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
    //�ν����Ϳ� ������
    [SerializeField]
    private List<SingleUIInfo> singleInfoList;
    [SerializeField]
    private List<MultipleUIInfo> multipleInfoList;
    //

    private Dictionary<SingleUIType, SingleUICount> singleInstanceDictionary;

    private Dictionary<MultipleUIType, ResourceEnum.Prefab> multipleInfoDictionary;
    private Dictionary<MultipleUIType, Queue<GameObject>> multiplePoolDictionary;




    //-------------------------------

    //�ν����Ϳ� ������
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

        //������ �ν��Ͻ� Ȯ��
        if (!fixedUIDictionary.TryGetValue(uiType, out FixedUICount result) || result == null)
        //������ �ν��Ͻ��� ���ٸ�?
        {
            //���� �ν��Ͻ��� �����Ѵ�.
            result.instance = GetFixedPrefab(uiType);
            result.count = true;
        }
        if(result.instance == null)
        {
            Debug.LogError("Fixed UI�� �غ���� �ʾҾƿ�. Fixed UI�� ������ ������ ������ �����͸� Ȯ�����ּ���!");
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
            //���ҽ� ���� Ȯ�� �Ұ�.
            Debug.LogError("Fixed UI�� �غ���� �ʾҾƿ�. Fixed UI�� ������ ������ ������ �����͸� Ȯ�����ּ���!");
            return false;
        }
        else if (result == null)
        {
            Debug.LogError("Fixed UI�� �غ���� �ʾҾƿ�. Fixed UI�� ������ ������ ������ �����͸� Ȯ�����ּ���!");
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
            Debug.LogError("���� UI�� �غ���� �ʾҾƿ�. ���� UI�� ������ ������ ������ �����͸� Ȯ�����ּ���!");
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
                Debug.LogError("�߸��� refcount �Դϴ�! ���� UIManager�� ������ �ʰ� ���Ǿ��� ���ɼ��� �ֽ��ϴ�!");
            }
        }
        else
        {
            Debug.LogError("���� UI�� �غ���� �ʾҾƿ�. ���� UI�� ������ ������ ������ �����͸� Ȯ�����ּ���!");
        }

        target = null;
    }

    public T GetSingleUI<T>(SingleUIType uiType) where T : SingleUIComponent
    {
        T target = null;

        #region �ڵ� �ߺ� ���̱�
        if (!singleInstanceDictionary.TryGetValue(uiType, out GameObject resultG) || resultG == null)
        {
            if (singleInfoDictionary.TryGetValue(uiType, out ResourceEnum.Prefab resultP))
            {
                resultG = ResourceManager.GetResource(resultP);
                resultG = GameObject.Instantiate(resultG);
            }
            else
            {
                Debug.LogError("UI�� ���� ���ҽ��� ã�� �� �����!");
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
            Debug.LogError("UI�� ���� ���ҽ��� ã�� �� �����!");
            return null;
        }

        target.transform.parent = GameManager.Instance.MainCanvas.transform;
        return target;
        #endregion
        //vs
        #region ���������� �ۼ��ϱ�
        //�켱 �����Ǿ� �ִ� ���� UI�� �ִ� �� Ȯ��
        if (singleInstanceDictionary.TryGetValue(uiType, out GameObject resultG))
        {
            //Dictionary������ �ִµ� ����ִٸ�
            if (resultG == null)
            {
                //Resource�� ã�ƿ� ����
                if (singleInfoDictionary.TryGetValue(uiType, out ResourceEnum.Prefab resultP))
                {
                    GameObject prefab = ResourceManager.GetResource(resultP);
                    target = GameObject.Instantiate(prefab).GetComponent<T>();
                    singleInstanceDictionary[uiType] = target.gameObject;
                }
                //ResourceManager���� �� ���� UI������ ���ٸ�
                else
                {
                    Debug.LogError("UI�� ���� ���ҽ��� ã�� �� �����!");
                }
            }
            else
            {
                //�̹� �ִٸ�
                target = resultG.GetComponent<T>();
            }
        }
        //���� UI�� �����Ǿ� ���� ���� ���¶��
        else
        {
            //Resource�� ã�ƿ� ����
            if (singleInfoDictionary.TryGetValue(uiType, out ResourceEnum.Prefab resultP))
            {
                GameObject prefab = ResourceManager.GetResource(resultP);
                target = GameObject.Instantiate(prefab).GetComponent<T>();
                singleInstanceDictionary.Add(uiType, target.gameObject);
            }
            //ResourceManager���� �� ���� UI������ ���ٸ�
            else
            {
                Debug.LogError("UI�� ���� ���ҽ��� ã�� �� �����!");
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