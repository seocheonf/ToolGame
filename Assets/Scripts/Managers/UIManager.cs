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