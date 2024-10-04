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
    //�� ���� ��ũ�� �ֻ���� ��ġ�� �����ϱ� ���� �����μ�, ���ϴ����� ���� uiBlock�� ��ĭ �ö󰡴� ���� �������� ��´�.
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
    /// �Է� ������ ���޹޾� UI�� �����ŵ�ϴ�.
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
    /// �Է� ������ ���޹޾� UI���� �����մϴ�.
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
    /// UIBlock�� ����� mask�� ����� backGround�� �ڽ����� ���� ��, ��ȯ�մϴ�.
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
    /// UIBlock�� Ǯ������ �����ɴϴ�. ���� Ǯ�� �����Ϳ� ���ٸ�, ���� ���� ��ȯ�մϴ�.
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
    /// UIBlock�� Ǯ���� �ݳ��մϴ�.
    /// </summary>
    private void PutBlock(PlayableInputUIBlock data)
    {
        uiBlockPool.Enqueue(data);
    }

    /// <summary>
    /// UIBlock�� �׾� �ø��ϴ�.
    /// </summary>
    /// <param name="data"></param>
    private void SetBlock(PlayableInputUIBlock data)
    {
        data.SetActive(true);
        uiBlockList.Add(data);
        RefreshBlockPos();
    }
    /// <summary>
    /// Ư�� ��ġ�� UIBlock�� �����ϴ�.
    /// </summary>
    private void UnSetBlock(int index)
    {
        uiBlockList[index].SetActive(false);
        uiBlockList.RemoveAt(index);
        RefreshBlockPos();
    }
    /// <summary>
    /// Ư�� UIBlock�� �����ϴ�.
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
