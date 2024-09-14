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
        //우선 생성되어 있는 유일 UI가 있는 지 확인
        if(singleInstanceDictionary.TryGetValue(uiType, out GameObject resultG))
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
