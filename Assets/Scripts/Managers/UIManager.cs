using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;

public class UIManager : Manager
{
    private Dictionary<SingleUIType, ResourceEnum.Prefab> singleInfoDictionary;
    private Dictionary<SingleUIType, GameObject> singleInstanceDictionary;

    private Dictionary<MultipleUIType, ResourceEnum.Prefab> multipleInfoDictionary;
    
    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        singleInfoDictionary = new Dictionary<SingleUIType, ResourceEnum.Prefab>();
        singleInstanceDictionary = new Dictionary<SingleUIType, GameObject>();

        multipleInfoDictionary = new Dictionary<MultipleUIType, ResourceEnum.Prefab>();


    }

    private void SetSingleUIPrefabPair(SingleUIType uiType, ResourceEnum.Prefab prefab)
    {
        singleInfoDictionary.Add(uiType, prefab);
    }
    private void SetMultipleUIPrefabPair(MultipleUIType uiType, ResourceEnum.Prefab prefab)
    {
        multipleInfoDictionary.Add(uiType, prefab);
    }

    public T GetSingleUI<T>(SingleUIType uiType) where T : SingleUIComponent
    {
        T target = null;
        //�켱 �����Ǿ� �ִ� ���� UI�� �ִ� �� Ȯ��
        if(singleInstanceDictionary.TryGetValue(uiType, out GameObject resultG))
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
