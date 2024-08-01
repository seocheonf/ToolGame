using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabPool
{
    //대상 프리팹
    public ResourceEnum.Prefab myPrefab;
    //개수
    public int number;
}

public class PoolManager : Manager
{
    //Pooling하고 있는 데이터 정보
    private Dictionary<ResourceEnum.Prefab, Queue<GameObject>> prefabPoolDictionary = new Dictionary<ResourceEnum.Prefab, Queue<GameObject>>();

    /// <summary>
    /// Prefab 풀링 데이터를 List기반으로 설정하는 함수
    /// </summary>
    /// <param name="wantPoolData">대상 Prefab 풀링 데이터 List</param>
    /// <param name="dataCountPerFrame">프레임당 생성하여 풀링할 데이터의 개수</param>
    /// <returns></returns>
    public IEnumerator SetPool(List<PrefabPool> wantPoolData, int poolDataCountPerFrame = 75)
    {
        int currentPoolDataCount = 0;

        //디자인 전환용 트리거
        bool loadingInfoDesign = false;

        foreach (PrefabPool eachPoolData in wantPoolData)
        {
            ResourceEnum.Prefab currentPrefab = eachPoolData.myPrefab;
            int currentCount = eachPoolData.number;

            for(int i = 0; i < currentCount; i++)
            {
                //디자인 전환 작업
                loadingInfoDesign = (currentPoolDataCount % 75 == 0) ? !loadingInfoDesign : loadingInfoDesign;
                GameManager.TurnOnBasicLoadingCavnas($"<{currentPrefab}> Data Loading..{(loadingInfoDesign ? ".." : "  ")} [{i}/{currentCount}]");

                //하나의 오브젝트를 생성하여 풀링에 등록
                SetStock(currentPrefab);

                //지정 개수만큼 데이터 풀링을 완료하였다면 자신의 역할을 미룸. 프레임 마다 풀링할 데이터의 개수 제한하여 로딩 창 구현.
                if((++currentPoolDataCount % currentPoolDataCount) == 0)
                {
                    yield return null;
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// 풀링 데이터를 기반으로 풀링 객체를 만들고 풀링에 넣는 함수
    /// </summary>
    /// <param name="targetPrefab">대상 Prefab 풀링 데이터</param>
    private void SetStock(ResourceEnum.Prefab targetPrefab)
    {
        //오브젝트 생성 후 끄기
        GameObject instance = GameObject.Instantiate(ResourceManager.GetResource(targetPrefab));
        instance.SetActive(false);

        //풀링으로 태어난 존재 확인
        instance.AddComponent<PoolingInfo>().SetInfo(targetPrefab);

        //이미 Pool에 targetPrefab에 대한 key 정보가 없으면 큐와 함께 생성
        //만약 큐가 없는 예외가 발생하면 큐만 만들어서 넣어주기
        if(!prefabPoolDictionary.TryGetValue(targetPrefab, out Queue<GameObject> result))
        {
            //없다면
            result = new Queue<GameObject>();
            prefabPoolDictionary.Add(targetPrefab, result);
        }
        else if(result == null)
        {
            //있는데 Queue가 비워져 있다면
            result = new Queue<GameObject>();
            prefabPoolDictionary[targetPrefab] = result;
        }

        //result에는 대상 Prefab에 대한 Queue가 있음이 보장되고, 여기에 생성된 GameObject를 넣어준다.
        result.Enqueue(instance);
    }

    //현재 월드로 부터 풀링 데이터를 받아오도록하는 함수
    //쓰기 쉽게 static으로 하되, 현재 World의 Pool이라는게 보장되어야 하니 직접 가져오도록 함.
    /// <summary>
    /// 현재 월드의 풀링으로부터 풀링 객체를 받아오는 함수
    /// </summary>
    /// <param name="targetPrefab">대상 Prefab 풀링 데이터</param>
    /// <returns>사용 가능한 풀링 객체</returns>
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

        //새로 생성하는 과정X - 꺼져있는 것을 빼오면서 켜주는 과정
        if (!currentPool.prefabPoolDictionary.TryGetValue(targetPrefab, out Queue<GameObject> resultQueue))
        {
            //보관중인 풀링 객체가 없는데 풀링 객체를 요청한다면, 풀링 객체를 1개 만들어 보관해주기
            currentPool.SetStock(targetPrefab);
            //위의 결과로 인해 만들어 졌을 것이니, 값이 보장됨.
            currentPool.prefabPoolDictionary.TryGetValue(targetPrefab, out resultQueue);
        }
        else if (resultQueue.Count == 0) //key와 value가 있긴 있지만, Queue가 비어있을 때를 생각해야 함.
        {
            //풀링 객체 1개 만들고 풀링에 넣기. 이 함수의 결과로 queue에 데이터가 있음이 보장됨.
            currentPool.SetStock(targetPrefab);
        }

        GameObject result = resultQueue.Dequeue();
        result.SetActive(true);
        return result;
    }

    /// <summary>
    /// 현재 월드의 풀링으로부터 풀링 객체를 받아와 특정 월드좌표로 이동시키는 함수
    /// </summary>
    /// <param name="targetPrefab">대상 Prefab 풀링 데이터</param>
    /// <param name="position">풀링 객체의 이동 좌표</param>
    /// <returns>사용 가능한 풀링 객체</returns>
    public static GameObject TakeStock(ResourceEnum.Prefab targetPrefab, Vector3 position)
    {
        GameObject result = TakeStock(targetPrefab);
        result.transform.position = position;
        return result;
    }

    /// <summary>
    /// 현재 월드의 풀링으로부터 풀링 객체를 받아와 특정 월드좌표로 이동시키고 특정 회전 각으로 월드기준 회전시키는 함수
    /// </summary>
    /// <param name="targetPrefab">대상 Prefab 풀링 데이터</param>
    /// <param name="position">풀링 객체의 이동 좌표</param>
    /// <param name="rotation">풀링 객체의 회전 각도</param>
    /// <returns>사용 가능한 풀링 객체</returns>
    public static GameObject TakeStock(ResourceEnum.Prefab targetPrefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = TakeStock(targetPrefab, position);
        result.transform.rotation = rotation;
        return result;
    }

    /// <summary>
    /// 현재 월드의 풀링으로부터 풀링 객체를 받아와 특정 Transform의 'Unity 엔진 오브젝트 계층 상'의 자식으로 넣는 함수. 설정될 부모를 기준으로 자신의 초기 셋팅을 적용해 준다.
    /// </summary>
    /// <param name="targetPrefab">대상 Prefab 풀링 데이터</param>
    /// <param name="parent">부모로 설정할 게임 Transform 객체</param>
    /// <returns>사용 가능한 풀링 객체</returns>
    public static GameObject TakeStock(ResourceEnum.Prefab targetPrefab, Transform parent)
    {
        //실제 풀링 객체를 가져오고, 원본의 정보를 가져온다. 원본의 정보는 말 그대로 정보만으로서 의미를 가진다.
        GameObject result = TakeStock(targetPrefab);
        GameObject origin = ResourceManager.GetResource(targetPrefab);
        result.transform.parent = parent;
        result.transform.localPosition = origin.transform.position;
        result.transform.localRotation = origin.transform.rotation;
        result.transform.localScale = origin.transform.localScale;
        return result;
    }

    /// <summary>
    /// GameObject를 파괴/회수하는 함수.
    /// </summary>
    /// <param name="targetGameObject">대상 GameObject</param>
    public static void PutInStock(GameObject targetGameObject)
    {
        //풀링 객체였는지 확인해보기
        if (targetGameObject.TryGetComponent(out PoolingInfo info))
        {   
            PutInStock(info);
        }
        else
        {
            //풀링 아니라면, 그냥 대신 없애줌.
            GameObject.Destroy(targetGameObject);
        }
    }

    /// <summary>
    /// PoolingInfo Component를 보유한 GameObject를 파괴/회수하는 함수
    /// </summary>
    /// <param name="info">대상 PoolingInfo Component</param>
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
            //기본적인 정보를 초기화하여 넣어주는 과정
            targetGameObject.transform.parent = null;
            targetGameObject.transform.localPosition = origin.transform.position;
            targetGameObject.transform.localRotation = origin.transform.rotation;
            targetGameObject.transform.localScale = origin.transform.localScale;

            //끄는 과정
            targetGameObject.SetActive(false);

            //대응되는 key는 있지만 넣을 queue가 비어있다면
            if (result == null)
            {
                result = new Queue<GameObject>();
            }

            //집어 넣는 과정
            result.Enqueue(targetGameObject);
        }
        else
        {
            //대응되는 key가 없을 경우, 풀링이 아니므로 그냥 대신 없애줌
            GameObject.Destroy(targetGameObject);
        }
    }

}
