using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToolGame;
using UnityEngine;
using UnityEngine.EventSystems;


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
    public void PopAll()
    {
        int stackCount = nonBlockingStack.Count - 1;
        for(int i = stackCount; i >= 0; i--)
        {
            nonBlockingStack.Last().SetActive(false);
            nonBlockingStack.RemoveAt(i);
        }

        //while(nonBlockingStack.Count - 1 >= 0)
        //{
        //    nonBlockingStack.Last().SetActive(false);
        //}
        //nonBlockingStack.Clear();
    }

    public bool TryGetTop(out FloatingUIComponent result)
    {
        result = null;
        if(nonBlockingStack.Count > 0)
        {
            result = nonBlockingStack[^1];
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpInStack(FloatingUIComponent data)
    {
        nonBlockingStack.Remove(data);
        nonBlockingStack.Add(data);
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

        //==

        FuncInteractionData closeUI = new FuncInteractionData(OuterKeyCode.Esc, "떠있는 UI를 끄거나, 떠있는 UI가 없을 때 해야할 일을 합니다.", FloatingOff, null, null);
        ControllerManager.AddInputFuncInteraction(closeUI);

        //==


        yield return null;

        GameManager.TurnOffBasicLoadingCanvas();

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

        result = GameObject.Instantiate(result, GameManager.Instance.CurrentWorld.WorldCanvas.transform);

        if(!result.TryGetComponent(out resultT))
        {
            Debug.LogError("해당하는 UI의 속성이 아니에요!");
            return false;
        }

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

        result = GameObject.Instantiate(result, GameManager.Instance.CurrentWorld.WorldCanvas.transform);
        
        if (!result.TryGetComponent(out resultT))
        {
            Debug.LogError("해당하는 UI의 속성이 아니에요!");
            return false;
        }

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
    private bool TryGetTopUIStack(out FloatingUIComponent result)
    {
        FloatingNode peek = null;
        result = null;

        if (!floatingUIStack.TryPeek(out peek))
        {
            Debug.LogError("UI Stack에 빼낼 정보가 없어요!");
            return false;
        }

        if(!peek.TryGetTop(out result))
            result = peek.Head;

        if (result == null)
            return false;

        return true;
    }

    //===========

    private Action DoNonFloating;

    private void FloatingOff()
    {
        if(TryGetTopUIStack(out FloatingUIComponent result))
        {

            result.SetActive(false);
        }
        else
        {
            DoNonFloating?.Invoke();
        }
    }

    public void SetNonFloating(Action action)
    {
        if(DoNonFloating != null)
        {
            Debug.LogError("이미 떠 있지 않을 때 해야할 일이 등록되어 있어요! 우선 이전의 모든 기능을 지우고, 새 기능을 등록하겠습니다. 이전에 문제가 없었는지 확인해주세요!");
            DoNonFloating = null;
        }
        DoNonFloating = action;
    }

    public void UnSetNonFloating()
    {
        int number = DoNonFloating.GetInvocationList().Length;
        if (number != 1)
        {
            Debug.LogError("이미 떠 있지 않을 때 해야할 일이 1개가 있지않아요. 즉 0개거나 1개보다 많아요! 이 상태에서 매개변수의 기능을 제거합니다. 이전에 문제가 없었는지 확인해주세요!");
            foreach(Delegate each in DoNonFloating.GetInvocationList())
            {
                Debug.LogError(each.Target);
            }
        }
        DoNonFloating = null;
    }



    //=============

    public static void SetSelectedNull()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

}