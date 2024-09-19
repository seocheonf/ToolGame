using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;

[System.Serializable]
class SingleUIInfo
{
    public SingleUIType uiType;
    public ResourceEnum.Prefab prefab;
}

class MultipleUIInfo
{
    public MultipleUIType uiType;
    public ResourceEnum.Prefab prefab;
}

class SingleUICount
{
    public GameObject instance;
    public int count = 0;
    public HashSet<object> user = new HashSet<object>();

    public SingleUICount(GameObject newInstance)
    {
        instance = newInstance;
    }
}

[System.Serializable]
class FixedUIInfo
{
    public FixedUIType uiType;
    public ResourceEnum.Prefab prefab;
}

class FixedUICount
{
    public GameObject instance;
    public bool count = false;
}







public class UIManager : Manager
{
    //인스펙터용 데이터
    [SerializeField]
    private List<SingleUIInfo> singleInfoList;
    [SerializeField]
    private List<MultipleUIInfo> multipleInfoList;
    //

    private Dictionary<SingleUIType, SingleUICount> singleInstanceDictionary;

    private Dictionary<MultipleUIType, ResourceEnum.Prefab> multipleInfoDictionary;
    private Dictionary<MultipleUIType, Queue<GameObject>> multiplePoolDictionary;




    //-------------------------------

    //인스펙터용 데이터
    [SerializeField]
    private List<FixedUIInfo> fixedInfoList;
    private Dictionary<FixedUIType, GameObject> fixedPrefabDictionary;
    private Dictionary<FixedUIType, FixedUICount> fixedUIDictionary;
    
    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        GameManager.TurnOnBasicLoadingCavnas("Essential UI Loading..");

        //

        singleInstanceDictionary = new Dictionary<SingleUIType, SingleUICount>();

        for(int i = 0; i<singleInfoList.Count; i++)
        {
            singleInstanceDictionary.Add(singleInfoList[i].uiType, new(GameObject.Instantiate(ResourceManager.GetResource(singleInfoList[i].prefab))));
            singleInfoList[i] = null;
        }

        singleInfoList = null;

        //

        multipleInfoDictionary = new Dictionary<MultipleUIType, ResourceEnum.Prefab>();
        multiplePoolDictionary = new Dictionary<MultipleUIType, Queue<GameObject>>();

        for (int i = 0; i < multipleInfoList.Count; i++)
        {
            multipleInfoDictionary.Add(multipleInfoList[i].uiType, multipleInfoList[i].prefab);
            multipleInfoList[i] = null;
        }

        multipleInfoList = null;

        //


    }


    private T GetFixedUI<T>(FixedUIType uiType) where T : FixedUIComponent
    {
        T resultT = null;

        FixedUICount result = fixedUIDictionary[uiType];

        result.count = true;
        resultT = result.instance.GetComponent<T>();

        return resultT;
    }
    private bool TryGetFixedUI<T>(FixedUIType uiType, out T resultT) where T : FixedUIComponent
    {
        resultT = null;

        //생성된 인스턴스 확인
        if (!fixedUIDictionary.TryGetValue(uiType, out FixedUICount result) || result == null)
        //생성된 인스턴스가 없다면?
        {
            //새로 인스턴스를 생성한다.
            result.instance = GetFixedPrefab(uiType);
            result.count = true;
        }
        if(result.instance == null)
        {
            Debug.LogError("Fixed UI가 준비되지 않았아요. Fixed UI용 프리팹 설정과 프리팹 데이터를 확인해주세요!");
            return false;
        }

        resultT = result.instance.GetComponent<T>();
        return true;
    }





    private GameObject GetFixedPrefab(FixedUIType uiType)
    {
        return fixedPrefabDictionary[uiType];
    }
    private bool TryGetFixedPrefab(FixedUIType uIType, out GameObject result)
    {
        if (!fixedPrefabDictionary.TryGetValue(uIType, out result))
        {
            //리소스 정보 확인 불가.
            Debug.LogError("Fixed UI가 준비되지 않았아요. Fixed UI용 프리팹 설정과 프리팹 데이터를 확인해주세요!");
            return false;
        }
        else if (result == null)
        {
            Debug.LogError("Fixed UI가 준비되지 않았아요. Fixed UI용 프리팹 설정과 프리팹 데이터를 확인해주세요!");
            return false;
        }
        return true;
    }



















    public T GetSingleUI<T>(SingleUIType uiType, object user) where T : SingleUIComponent
    {
        if(singleInstanceDictionary.TryGetValue(uiType, out SingleUICount result) && result != null && result.instance != null)
        {
            result.count++;

            result.user.Add(user);

            T resultT = result.instance.GetComponent<T>();
            resultT.SetActive(true);

            return resultT;
        }
        else
        {
            Debug.LogError("유일 UI가 준비되지 않았아요. 유일 UI용 프리팹 설정과 프리팹 데이터를 확인해주세요!");
            return null;
        }
    }
    public void PutSingleUI<T>(ref T target) where T : SingleUIComponent
    {
#if UNITY_EDITOR
        if(target == null)
        {
            Debug.LogError("Null Ref Error!!!!!!!");
            return;
        }
#endif

        if (singleInstanceDictionary.TryGetValue(target.GetUIType(), out SingleUICount result) && result != null && result.instance != null)
        {
            result.count--;

            if (result.count == 0)
            {
                target.SetActive(false);
            }
            else if(result.count < 0)
            {
                Debug.LogError("잘못된 refcount 입니다! 값이 UIManager를 통하지 않고 사용되었을 가능성이 있습니다!");
            }
        }
        else
        {
            Debug.LogError("유일 UI가 준비되지 않았아요. 유일 UI용 프리팹 설정과 프리팹 데이터를 확인해주세요!");
        }

        target = null;
    }

    public T GetSingleUI<T>(SingleUIType uiType) where T : SingleUIComponent
    {
        T target = null;

        #region 코드 중복 줄이기
        if (!singleInstanceDictionary.TryGetValue(uiType, out GameObject resultG) || resultG == null)
        {
            if (singleInfoDictionary.TryGetValue(uiType, out ResourceEnum.Prefab resultP))
            {
                resultG = ResourceManager.GetResource(resultP);
                resultG = GameObject.Instantiate(resultG);
            }
            else
            {
                Debug.LogError("UI에 대한 리소스를 찾을 수 없어요!");
                return null;
            }

            if(!singleInstanceDictionary.TryAdd(uiType, resultG))
            {
                singleInstanceDictionary[uiType] = resultG;
            }

        }

        target = resultG.GetComponent<T>();

        if(target == null)
        {
            Debug.LogError("UI에 대한 리소스를 찾을 수 없어요!");
            return null;
        }

        target.transform.parent = GameManager.Instance.MainCanvas.transform;
        return target;
        #endregion
        //vs
        #region 직관적으로 작성하기
        //우선 생성되어 있는 유일 UI가 있는 지 확인
        if (singleInstanceDictionary.TryGetValue(uiType, out GameObject resultG))
        {
            //Dictionary구조는 있는데 비어있다면
            if (resultG == null)
            {
                //Resource를 찾아와 생성
                if (singleInfoDictionary.TryGetValue(uiType, out ResourceEnum.Prefab resultP))
                {
                    GameObject prefab = ResourceManager.GetResource(resultP);
                    target = GameObject.Instantiate(prefab).GetComponent<T>();
                    singleInstanceDictionary[uiType] = target.gameObject;
                }
                //ResourceManager에도 그 유일 UI정보가 없다면
                else
                {
                    Debug.LogError("UI에 대한 리소스를 찾을 수 없어요!");
                }
            }
            else
            {
                //이미 있다면
                target = resultG.GetComponent<T>();
            }
        }
        //유일 UI가 생성되어 있지 않은 상태라면
        else
        {
            //Resource를 찾아와 생성
            if (singleInfoDictionary.TryGetValue(uiType, out ResourceEnum.Prefab resultP))
            {
                GameObject prefab = ResourceManager.GetResource(resultP);
                target = GameObject.Instantiate(prefab).GetComponent<T>();
                singleInstanceDictionary.Add(uiType, target.gameObject);
            }
            //ResourceManager에도 그 유일 UI정보가 없다면
            else
            {
                Debug.LogError("UI에 대한 리소스를 찾을 수 없어요!");
            }
        }

        target.transform.parent = GameManager.Instance.MainCanvas.transform;
        return target;
        #endregion
    }
    public SingleUIComponent GetSingleUI(SingleUIType uiType)
    {
        return default;
    }

    public T GetMultipleUI<T>(MultipleUIType uiType) where T : MultipleUIComponent
    {
        return default;
    }
    public MultipleUIComponent GetMultipleUI(MultipleUIType uiType)
    {
        return default;
    }



}