using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableInputUI : FixedUIComponent
{
    private List<PlayableInputUIBlock> uiBlockList;
    //private Dictionary<FuncInteractionData, int> inputUIIndex;
    private Dictionary<FuncInteractionData, PlayableInputUIBlock> inputUIDictionary;
    private Queue<PlayableInputUIBlock> uiBlockPool;
    [SerializeField]
    int uiBlockDistance;
    [SerializeField]
    int startPosition;
    [SerializeField]
    RectTransform backGround;

    [SerializeField]
    //이 값은 스크롤 최상단의 위치를 조절하기 위한 값으로서, 최하단으로 부터 uiBlock이 몇칸 올라가는 것을 기준으로 삼는다.
    int upperBoundaryCount;

    float initialHeight;


    protected override void MyStart()
    {
        base.MyStart();

        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsUpdate += CustomUpdate;

        if(!temptTrigger)
        {
            temptTrigger = true;
            InitializeOnce();
        }
    }

    protected override void MyDestroy()
    {
        GameManager.ObjectsUpdate -= CustomUpdate;

        base.MyDestroy();
    }

    bool temptTrigger = false;
    private void InitializeOnce()
    {
        uiBlockList = new List<PlayableInputUIBlock>();
        //inputUIIndex = new Dictionary<FuncInteractionData, int>();
        inputUIDictionary = new Dictionary<FuncInteractionData, PlayableInputUIBlock>();
        uiBlockPool = new Queue<PlayableInputUIBlock>();

        initialHeight = startPosition;
    }

    /// <summary>
    /// 입력 정보를 전달받아 UI에 노출시킵니다.
    /// </summary>
    /// <param name="data"></param>
    public void SetInputInfo(FuncInteractionData data)
    {
        PlayableInputUIBlock block = GetBlock();
        block.SetInfo(ControllerManager.GetUnityKeyCode(data.keyCode).ToString(), data.description);
        //block.SetInfo(data.keyCode.ToString(), data.description);
        inputUIDictionary.Add(data, block);

        SetBlock(block);
    }
    public void SetInputInfo(List<FuncInteractionData> list)
    {
        foreach (FuncInteractionData each in list)
        {
            SetInputInfo(each);
        }
    }

    /// <summary>
    /// 입력 정보를 전달받아 UI에서 제거합니다.
    /// </summary>
    /// <param name="data"></param>
    public void UnSetInputInfo(FuncInteractionData data)
    {
        PlayableInputUIBlock block = inputUIDictionary[data];

        UnSetBlock(block);

        PutBlock(block);

        inputUIDictionary.Remove(data);
    }
    public void UnSetInputInfo(List<FuncInteractionData> list)
    {
        foreach(FuncInteractionData each in list)
        {
            UnSetInputInfo(each);
        }
    }

    /// <summary>
    /// UIBlock을 만들고 mask가 적용된 backGround의 자식으로 넣은 뒤, 반환합니다.
    /// </summary>
    /// <returns>UIBlock</returns>
    private PlayableInputUIBlock MakeBlock()
    {
        PlayableInputUIBlock instance = GameManager.Instance.UI.GetFixedUI<PlayableInputUIBlock>(ToolGame.FixedUIType.PlayableInputUIBlock);
        instance.transform.SetParent(backGround);
        instance.SetActive(false);
        return instance;
    }

    /// <summary>
    /// UIBlock을 풀링에서 가져옵니다. 만약 풀링 데이터에 없다면, 새로 만들어서 반환합니다.
    /// </summary>
    /// <returns></returns>
    private PlayableInputUIBlock GetBlock()
    {
        PlayableInputUIBlock data;
        if (!uiBlockPool.TryDequeue(out data))
        {
            data = MakeBlock();
        }
        return data;
    }
    /// <summary>
    /// UIBlock을 풀링에 반납합니다.
    /// </summary>
    private void PutBlock(PlayableInputUIBlock data)
    {
        uiBlockPool.Enqueue(data);
    }

    /// <summary>
    /// UIBlock을 쌓아 올립니다.
    /// </summary>
    /// <param name="data"></param>
    private void SetBlock(PlayableInputUIBlock data)
    {
        data.SetActive(true);
        uiBlockList.Add(data);
        RefreshBlockPos();
    }
    /// <summary>
    /// 특정 위치의 UIBlock을 빼냅니다.
    /// </summary>
    private void UnSetBlock(int index)
    {
        uiBlockList[index].SetActive(false);
        uiBlockList.RemoveAt(index);
        RefreshBlockPos();
    }
    /// <summary>
    /// 특정 UIBlock을 빼냅니다.
    /// </summary>
    private void UnSetBlock(PlayableInputUIBlock data)
    {
        data.SetActive(false);
        uiBlockList.Remove(data);
        RefreshBlockPos();
    }

    private void RefreshBlockPos()
    {
        Vector3 pos = Vector3.zero;
        for(int i = 0; i<uiBlockList.Count; i++)
        {
            pos.y = initialHeight + uiBlockDistance * i;
            uiBlockList[i].RectInfo.localPosition = pos;
        }
    }






    private void CustomUpdate(float deltaTime)
    {
        SetScrollPosition(-ControllerManager.MouseScrollDelta * 10f);
    }

    public void SetScrollPosition(float scrollPosition)
    {

        if(uiBlockList.Count > 6)
        {
            float boundary = scrollPosition;

            if (uiBlockList[0].RectInfo.localPosition.y + scrollPosition > initialHeight)
            {
                boundary = initialHeight - uiBlockList[0].RectInfo.localPosition.y;
            }
            else if (uiBlockList[^1].RectInfo.localPosition.y + scrollPosition < (initialHeight + uiBlockDistance * upperBoundaryCount))
            {
                boundary = (initialHeight + uiBlockDistance * upperBoundaryCount) - uiBlockList[^1].RectInfo.localPosition.y;
            }

            Vector3 next = Vector3.zero;
            foreach(PlayableInputUIBlock each in uiBlockList)
            {
                next = each.RectInfo.localPosition;
                next.y += boundary;
                each.RectInfo.localPosition = next;
            }
        }


        /*
        Vector3 next = TextsArea.localPosition;
        next.y += scrollPosition;
        next.y = Mathf.Clamp(next.y, -initialHeight, initialHeight);
        Debug.Log(next.y);
        TextsArea.localPosition = next;*/
    }


}
