using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;


class FloatingNode
{
    public FloatingUIComponent headBlockingUI;
    public List<FloatingUIComponent> nonBlockingStack;
}

public class UIManager : Manager
{

    private Stack<FloatingNode> floatingUIStack;

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        GameManager.TurnOnBasicLoadingCavnas("Essential UI Loading..");

        floatingUIStack = new Stack<FloatingNode>();

    }

    
    public T GetFixedUI<T>(FixedUIType uiType) where T : FixedUIComponent
    {
        GameObject result = null;
        T resultT = null;

        result = ResourceManager.GetResource(ResourceEnum.Prefab);
        resultT = result.GetComponent<T>();

        return resultT;
    }
    public bool TryGetFixedUI<T>(FixedUIType uiType, out T resultT) where T : FixedUIComponent
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

    public T GetFloatingUI<T>(FloatingUIType uIType) where T : FloatingUIComponent
    {

    }
    public bool TryGetFloatingUI<T>(FloatingUIType uiType, out T resultT) where T : FloatingUIComponent
    {

    }

    private void PushUIStack(Stack<FloatingNode> target, FloatingUIComponent data)
    {

    }
    private void PopUIStack(Stack<FloatingNode> target, FloatingUIComponent data)
    {

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