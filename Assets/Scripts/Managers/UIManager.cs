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
            Debug.LogError("NonBlockingUI Stack�� ����� �־��!");
            return false;
        }
        else if (nonBlockingStack[^1] == data)
        {
            nonBlockingStack.RemoveAt(nonBlockingStack.Count - 1);
            return true;
        }
        else
        {
            Debug.LogError("UI Stack�� �ֻ�ܰ� �����ϰ��� �ϴ� UI�� ��ġ���� �ʽ��ϴ�!");
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

        FuncInteractionData closeUI = new FuncInteractionData(OuterKeyCode.Esc, "���ִ� UI�� ���ų�, ���ִ� UI�� ���� �� �ؾ��� ���� �մϴ�.", FloatingOff, null, null);
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
            Debug.LogError("�ش��ϴ� UI enum value�� ���� Prefab enum value�� �����!");
            return false;
        }

        if(!ResourceManager.TryGetResource(prefab, out result))
        {
            Debug.LogError("�ش��ϴ� UI ���ҽ��� �����!");
            return false;
        }

        result = GameObject.Instantiate(result, GameManager.Instance.CurrentWorld.WorldCanvas.transform);

        if(!result.TryGetComponent(out resultT))
        {
            Debug.LogError("�ش��ϴ� UI�� �Ӽ��� �ƴϿ���!");
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
            Debug.LogError("�ش��ϴ� UI enum value�� ���� Prefab enum value�� �����!");
            return false;
        }

        if (!ResourceManager.TryGetResource(prefab, out result))
        {
            Debug.LogError("�ش��ϴ� UI ���ҽ��� �����!");
            return false;
        }

        result = GameObject.Instantiate(result, GameManager.Instance.CurrentWorld.WorldCanvas.transform);
        
        if (!result.TryGetComponent(out resultT))
        {
            Debug.LogError("�ش��ϴ� UI�� �Ӽ��� �ƴϿ���!");
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

    //floatingUIStack�� ���� Push Pop�Լ�
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

        // ���� UI ������ stack�� ���� ���ٸ�?
        if(!floatingUIStack.TryPeek(out peek))
        {
            Debug.LogError("�׿��ִ� UI�� ���µ� UI�� �������� �߾��!");
            return false;
        }

        // ���� UI ������ stack �ֻ���� Head�� �����ϴٸ�?
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
            Debug.LogError("UI Stack�� ���� ������ �����!");
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
            Debug.LogError("�̹� �� ���� ���� �� �ؾ��� ���� ��ϵǾ� �־��! �켱 ������ ��� ����� �����, �� ����� ����ϰڽ��ϴ�. ������ ������ �������� Ȯ�����ּ���!");
            DoNonFloating = null;
        }
        DoNonFloating = action;
    }

    public void UnSetNonFloating()
    {
        int number = DoNonFloating.GetInvocationList().Length;
        if (number != 1)
        {
            Debug.LogError("�̹� �� ���� ���� �� �ؾ��� ���� 1���� �����ʾƿ�. �� 0���ų� 1������ ���ƿ�! �� ���¿��� �Ű������� ����� �����մϴ�. ������ ������ �������� Ȯ�����ּ���!");
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