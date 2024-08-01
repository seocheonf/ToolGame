using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabPool
{
    //��� ������
    public ResourceEnum.Prefab myPrefab;
    //����
    public int number;
}

public class PoolManager : Manager
{
    //Pooling�ϰ� �ִ� ������ ����
    private Dictionary<ResourceEnum.Prefab, Queue<GameObject>> prefabPoolDictionary = new Dictionary<ResourceEnum.Prefab, Queue<GameObject>>();

    /// <summary>
    /// Prefab Ǯ�� �����͸� List������� �����ϴ� �Լ�
    /// </summary>
    /// <param name="wantPoolData">��� Prefab Ǯ�� ������ List</param>
    /// <param name="dataCountPerFrame">�����Ӵ� �����Ͽ� Ǯ���� �������� ����</param>
    /// <returns></returns>
    public IEnumerator SetPool(List<PrefabPool> wantPoolData, int poolDataCountPerFrame = 75)
    {
        int currentPoolDataCount = 0;

        //������ ��ȯ�� Ʈ����
        bool loadingInfoDesign = false;

        foreach (PrefabPool eachPoolData in wantPoolData)
        {
            ResourceEnum.Prefab currentPrefab = eachPoolData.myPrefab;
            int currentCount = eachPoolData.number;

            for(int i = 0; i < currentCount; i++)
            {
                //������ ��ȯ �۾�
                loadingInfoDesign = (currentPoolDataCount % 75 == 0) ? !loadingInfoDesign : loadingInfoDesign;
                GameManager.TurnOnBasicLoadingCavnas($"<{currentPrefab}> Data Loading..{(loadingInfoDesign ? ".." : "  ")} [{i}/{currentCount}]");

                //�ϳ��� ������Ʈ�� �����Ͽ� Ǯ���� ���
                SetStock(currentPrefab);

                //���� ������ŭ ������ Ǯ���� �Ϸ��Ͽ��ٸ� �ڽ��� ������ �̷�. ������ ���� Ǯ���� �������� ���� �����Ͽ� �ε� â ����.
                if((++currentPoolDataCount % currentPoolDataCount) == 0)
                {
                    yield return null;
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// Ǯ�� �����͸� ������� Ǯ�� ��ü�� ����� Ǯ���� �ִ� �Լ�
    /// </summary>
    /// <param name="targetPrefab">��� Prefab Ǯ�� ������</param>
    private void SetStock(ResourceEnum.Prefab targetPrefab)
    {
        //������Ʈ ���� �� ����
        GameObject instance = GameObject.Instantiate(ResourceManager.GetResource(targetPrefab));
        instance.SetActive(false);

        //Ǯ������ �¾ ���� Ȯ��
        instance.AddComponent<PoolingInfo>().SetInfo(targetPrefab);

        //�̹� Pool�� targetPrefab�� ���� key ������ ������ ť�� �Բ� ����
        //���� ť�� ���� ���ܰ� �߻��ϸ� ť�� ���� �־��ֱ�
        if(!prefabPoolDictionary.TryGetValue(targetPrefab, out Queue<GameObject> result))
        {
            //���ٸ�
            result = new Queue<GameObject>();
            prefabPoolDictionary.Add(targetPrefab, result);
        }
        else if(result == null)
        {
            //�ִµ� Queue�� ����� �ִٸ�
            result = new Queue<GameObject>();
            prefabPoolDictionary[targetPrefab] = result;
        }

        //result���� ��� Prefab�� ���� Queue�� ������ ����ǰ�, ���⿡ ������ GameObject�� �־��ش�.
        result.Enqueue(instance);
    }

    //���� ����� ���� Ǯ�� �����͸� �޾ƿ������ϴ� �Լ�
    //���� ���� static���� �ϵ�, ���� World�� Pool�̶�°� ����Ǿ�� �ϴ� ���� ���������� ��.
    /// <summary>
    /// ���� ������ Ǯ�����κ��� Ǯ�� ��ü�� �޾ƿ��� �Լ�
    /// </summary>
    /// <param name="targetPrefab">��� Prefab Ǯ�� ������</param>
    /// <returns>��� ������ Ǯ�� ��ü</returns>
    public static GameObject TakeStock(ResourceEnum.Prefab targetPrefab)
    {
#if UNITY_EDITOR
        if (GameManager.Instance.CurrentWorld == null || GameManager.Instance.CurrentWorld.Pool == null)
        {
            Debug.LogAssertion($"You try to take {targetPrefab} in poolingStock before Manager or Pool Instantiate");
            return null;
        }
#endif

        PoolManager currentPool = GameManager.Instance.CurrentWorld.Pool;

        //���� �����ϴ� ����X - �����ִ� ���� �����鼭 ���ִ� ����
        if (!currentPool.prefabPoolDictionary.TryGetValue(targetPrefab, out Queue<GameObject> resultQueue))
        {
            //�������� Ǯ�� ��ü�� ���µ� Ǯ�� ��ü�� ��û�Ѵٸ�, Ǯ�� ��ü�� 1�� ����� �������ֱ�
            currentPool.SetStock(targetPrefab);
            //���� ����� ���� ����� ���� ���̴�, ���� �����.
            currentPool.prefabPoolDictionary.TryGetValue(targetPrefab, out resultQueue);
        }
        else if (resultQueue.Count == 0) //key�� value�� �ֱ� ������, Queue�� ������� ���� �����ؾ� ��.
        {
            //Ǯ�� ��ü 1�� ����� Ǯ���� �ֱ�. �� �Լ��� ����� queue�� �����Ͱ� ������ �����.
            currentPool.SetStock(targetPrefab);
        }

        GameObject result = resultQueue.Dequeue();
        result.SetActive(true);
        return result;
    }

    /// <summary>
    /// ���� ������ Ǯ�����κ��� Ǯ�� ��ü�� �޾ƿ� Ư�� ������ǥ�� �̵���Ű�� �Լ�
    /// </summary>
    /// <param name="targetPrefab">��� Prefab Ǯ�� ������</param>
    /// <param name="position">Ǯ�� ��ü�� �̵� ��ǥ</param>
    /// <returns>��� ������ Ǯ�� ��ü</returns>
    public static GameObject TakeStock(ResourceEnum.Prefab targetPrefab, Vector3 position)
    {
        GameObject result = TakeStock(targetPrefab);
        result.transform.position = position;
        return result;
    }

    /// <summary>
    /// ���� ������ Ǯ�����κ��� Ǯ�� ��ü�� �޾ƿ� Ư�� ������ǥ�� �̵���Ű�� Ư�� ȸ�� ������ ������� ȸ����Ű�� �Լ�
    /// </summary>
    /// <param name="targetPrefab">��� Prefab Ǯ�� ������</param>
    /// <param name="position">Ǯ�� ��ü�� �̵� ��ǥ</param>
    /// <param name="rotation">Ǯ�� ��ü�� ȸ�� ����</param>
    /// <returns>��� ������ Ǯ�� ��ü</returns>
    public static GameObject TakeStock(ResourceEnum.Prefab targetPrefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = TakeStock(targetPrefab, position);
        result.transform.rotation = rotation;
        return result;
    }

    /// <summary>
    /// ���� ������ Ǯ�����κ��� Ǯ�� ��ü�� �޾ƿ� Ư�� Transform�� 'Unity ���� ������Ʈ ���� ��'�� �ڽ����� �ִ� �Լ�. ������ �θ� �������� �ڽ��� �ʱ� ������ ������ �ش�.
    /// </summary>
    /// <param name="targetPrefab">��� Prefab Ǯ�� ������</param>
    /// <param name="parent">�θ�� ������ ���� Transform ��ü</param>
    /// <returns>��� ������ Ǯ�� ��ü</returns>
    public static GameObject TakeStock(ResourceEnum.Prefab targetPrefab, Transform parent)
    {
        //���� Ǯ�� ��ü�� ��������, ������ ������ �����´�. ������ ������ �� �״�� ���������μ� �ǹ̸� ������.
        GameObject result = TakeStock(targetPrefab);
        GameObject origin = ResourceManager.GetResource(targetPrefab);
        result.transform.parent = parent;
        result.transform.localPosition = origin.transform.position;
        result.transform.localRotation = origin.transform.rotation;
        result.transform.localScale = origin.transform.localScale;
        return result;
    }

    /// <summary>
    /// GameObject�� �ı�/ȸ���ϴ� �Լ�.
    /// </summary>
    /// <param name="targetGameObject">��� GameObject</param>
    public static void PutInStock(GameObject targetGameObject)
    {
        //Ǯ�� ��ü������ Ȯ���غ���
        if (targetGameObject.TryGetComponent(out PoolingInfo info))
        {   
            PutInStock(info);
        }
        else
        {
            //Ǯ�� �ƴ϶��, �׳� ��� ������.
            GameObject.Destroy(targetGameObject);
        }
    }

    /// <summary>
    /// PoolingInfo Component�� ������ GameObject�� �ı�/ȸ���ϴ� �Լ�
    /// </summary>
    /// <param name="info">��� PoolingInfo Component</param>
    public static void PutInStock(PoolingInfo info)
    {
#if UNITY_EDITOR
        if (GameManager.Instance.CurrentWorld == null || GameManager.Instance.CurrentWorld.Pool == null)
        {
            Debug.LogAssertion($"You try to put {info} in poolingStock before Manager or Pool Instantiate");
            return;
        }
#endif

        GameObject targetGameObject = info.gameObject;
        GameObject origin = ResourceManager.GetResource(info.MyPrefab);

        if (GameManager.Instance.CurrentWorld.Pool.prefabPoolDictionary.TryGetValue(info.MyPrefab, out Queue<GameObject> result))
        {
            //�⺻���� ������ �ʱ�ȭ�Ͽ� �־��ִ� ����
            targetGameObject.transform.parent = null;
            targetGameObject.transform.localPosition = origin.transform.position;
            targetGameObject.transform.localRotation = origin.transform.rotation;
            targetGameObject.transform.localScale = origin.transform.localScale;

            //���� ����
            targetGameObject.SetActive(false);

            //�����Ǵ� key�� ������ ���� queue�� ����ִٸ�
            if (result == null)
            {
                result = new Queue<GameObject>();
            }

            //���� �ִ� ����
            result.Enqueue(targetGameObject);
        }
        else
        {
            //�����Ǵ� key�� ���� ���, Ǯ���� �ƴϹǷ� �׳� ��� ������
            GameObject.Destroy(targetGameObject);
        }
    }

}
