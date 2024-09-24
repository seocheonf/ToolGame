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
            Debug.LogError("�ش��ϴ� UI enum value�� ���� Prefab enum value�� �����!");
            return false;
        }

        if(!ResourceManager.TryGetResource(prefab, out result))
        {
            Debug.LogError("�ش��ϴ� UI ���ҽ��� �����!");
            return false;
        }

        result = GameObject.Instantiate(result);

        if(!result.TryGetComponent(out resultT))
        {
            Debug.LogError("�ش��ϴ� UI�� �Ӽ��� �ƴϿ���!");
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
            Debug.LogError("�ش��ϴ� UI enum value�� ���� Prefab enum value�� �����!");
            return false;
        }

        if (!ResourceManager.TryGetResource(prefab, out result))
        {
            Debug.LogError("�ش��ϴ� UI ���ҽ��� �����!");
            return false;
        }

        result = GameObject.Instantiate(result);
        
        if (!result.TryGetComponent(out resultT))
        {
            Debug.LogError("�ش��ϴ� UI�� �Ӽ��� �ƴϿ���!");
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

    /*

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


    */
}