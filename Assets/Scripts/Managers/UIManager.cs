using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToolGame;
using UnityEngine;


class FloatingNode
{
    private FloatingUIComponent headBlockingUI;
    private List<FloatingUIComponent> nonBlockingStack;

    public FloatingUIComponent Head => headBlockingUI;

    public FloatingNode(FloatingUIComponent head)
    {
        headBlockingUI = head;
        nonBlockingStack = new List<FloatingUIComponent>();
    }

    public void Push(FloatingUIComponent data)
    {
        nonBlockingStack.Add(data);
    }
    public bool Pop(FloatingUIComponent data)
    {
        if (nonBlockingStack.Count == 0)
        {
            Debug.LogError("NonBlockingUI Stack이 비워져 있어요!");
            return false;
        }
        else if (nonBlockingStack[^1] == data)
        {
            nonBlockingStack.RemoveAt(nonBlockingStack.Count - 1);
            return true;
        }
        else
        {
            Debug.LogError("UI Stack의 최상단과 제거하고자 하는 UI가 일치하지 않습니다!");
            return false;
        }
    }
    public void UpInStack(FloatingUIComponent data)
    {
        nonBlockingStack.Remove(data);
        nonBlockingStack.Add(data);
    }

    public void PopAll()
    {

        while(nonBlockingStack.Count - 1 >= 0)
        {
            nonBlockingStack.Last().SetActive(false);
        }

        nonBlockingStack.Clear();
    }
}

public class UIManager : Manager
{

    private Stack<FloatingNode> floatingUIStack;

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        GameManager.TurnOnBasicLoadingCavnas("Essential UI Loading..");

        floatingUIStack = new Stack<FloatingNode>();

        floatingUIStack.Push(new FloatingNode(null));

    }

    private bool TryGetFixedInstance<T>(FixedUIType uiType, out T resultT) where T : FixedUIComponent
    {

        resultT = null;
        GameObject result = null;

        string typeName = uiType.ToString();
        typeName = "UI_Fixed_" + typeName;
        
        if(!Enum.TryParse(typeName, out ResourceEnum.Prefab prefab))
        {
            Debug.LogError("해당하는 UI enum value에 대한 Prefab enum value가 없어요!");
            return false;
        }

        if(!ResourceManager.TryGetResource(prefab, out result))
        {
            Debug.LogError("해당하는 UI 리소스가 없어요!");
            return false;
        }

        result = GameObject.Instantiate(result);

        if(!result.TryGetComponent(out resultT))
        {
            Debug.LogError("해당하는 UI의 속성이 아니에요!");
            return false;
        }

        result.transform.SetParent(GameManager.Instance.CurrentWorld.WorldCanvas.transform);

        return true;

    }
    public T GetFixedUI<T>(FixedUIType uiType) where T : FixedUIComponent
    {
        T resultT;

        TryGetFixedInstance(uiType, out resultT);

        return resultT;
    }
    public bool TryGetFixedUI<T>(FixedUIType uiType, out T resultT) where T : FixedUIComponent
    {
        if(!TryGetFixedInstance(uiType, out resultT))
        {
            return false;
        }
        return true;
    }

    private bool TryGetFloatingInstance<T>(FloatingUIType uiType, out T resultT) where T : FloatingUIComponent
    {

        resultT = null;
        GameObject result = null;

        string typeName = uiType.ToString();
        typeName = "UI_Floating_" + typeName;

        if (!Enum.TryParse(typeName, out ResourceEnum.Prefab prefab))
        {
            Debug.LogError("해당하는 UI enum value에 대한 Prefab enum value가 없어요!");
            return false;
        }

        if (!ResourceManager.TryGetResource(prefab, out result))
        {
            Debug.LogError("해당하는 UI 리소스가 없어요!");
            return false;
        }

        result = GameObject.Instantiate(result);
        
        if (!result.TryGetComponent(out resultT))
        {
            Debug.LogError("해당하는 UI의 속성이 아니에요!");
            return false;
        }

        result.transform.SetParent(GameManager.Instance.CurrentWorld.WorldCanvas.transform);

        return true;

    }
    public T GetFloatingUI<T>(FloatingUIType uiType) where T : FloatingUIComponent
    {
        T resultT;

        TryGetFloatingInstance(uiType, out resultT);

        resultT.UIStart += () => { PushUIStack(resultT); };
        resultT.UIDestroy += () => { PopUIStack(resultT); };

        return resultT;
    }
    public bool TryGetFloatingUI<T>(FloatingUIType uiType, out T resultT) where T : FloatingUIComponent
    {

        if (!TryGetFloatingInstance(uiType, out resultT))
        {
            return false;
        }

        T lambda = resultT;

        lambda.UIStart += () => { PushUIStack(lambda); };
        lambda.UIDestroy += () => { PopUIStack(lambda); };

        return true;
    }

    //floatingUIStack을 위한 Push Pop함수
    private void PushUIStack(FloatingUIComponent data)
    {

        if(floatingUIStack.Count == 0)
        {
            floatingUIStack.Push(new FloatingNode(null));
        }

        if(data.IsBlocking)
        {
            FloatingNode node = new FloatingNode(data);
            floatingUIStack.Push(node);
        }
        else
        {
            floatingUIStack.TryPeek(out FloatingNode peek);
            peek.Push(data);
        }
    }
    private bool PopUIStack(FloatingUIComponent data)
    {
        FloatingNode peek = null;

        // 빼낼 UI 정보가 stack에 전혀 없다면?
        if(!floatingUIStack.TryPeek(out peek))
        {
            Debug.LogError("쌓여있는 UI가 없는데 UI를 닫으려고 했어요!");
            return false;
        }

        // 빼낼 UI 정보가 stack 최상단의 Head와 동일하다면?
        if (peek.Head == data)
        {
            peek.PopAll();
            floatingUIStack.Pop();
            return true;
        }

        return peek.Pop(data);
    }

    /*

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


    */
}