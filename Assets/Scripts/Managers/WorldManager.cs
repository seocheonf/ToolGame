using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    //각 월드가 관리할 카메라 정보
    private CameraManager worldCamera;
    public CameraManager WorldCamera => worldCamera;

    //각 월드가 관리할 풀링 정보
    private PoolManager pool;
    public PoolManager Pool => pool;

    //인스펙터창에서 미리 설정할 풀링 데이터 정보들
    [SerializeField]
    private List<PrefabPool> prefabPoolList;

    //각 월드가 Update상에서 해야할 일들의 모음
    //나중에 중간에 월드를 빠져나갈 때, 한꺼번에 있던거 다 뺄 수 있게 애초에 이를 통해서 넣도록 함.
    public UpdateFunction WorldUpdates;
    public FixedUpdateFunction WorldFixedUpdates;


    protected virtual IEnumerator Start()
    {
#if UNITY_EDITOR
        //에디터 상에선 개발 편의를 위해, 게임 매니저가 없으면 최초 씬으로 이동하는 기능
        //빌드 상에선 그 상황 자체가 에러이기에 (물론 문제 없을 수 있으나, 최초 씬 고치기도 쉽고, 안전하게 가는 것이 좋음)
        if (GameManager.Instance == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            yield break;
        }
#endif

        //GameManager와 WorldManager가 하나의 씬에 모두 존재할 때만 필요한(유의미한) 기능
        //예외 처리
        yield return new WaitUntil(() => { return GameManager.Instance != null; });

        //본인이 태어났을 때 해야할 일들을 게임 매니저에 전달하여 미뤄 실행
        GameManager.ManagersStart += WorldManagerStart;

    }
    
    /// <summary>
    /// 월드 매니저가 태어났을 때 할 일
    /// </summary>
    protected virtual void WorldManagerStart()
    {
        StartCoroutine(Initiate());
    }

    /// <summary>
    /// 월드 매니저를 초기화하면서 해야할 일
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Initiate()
    {
        GameManager.TurnOnBasicLoadingCavnas("World Loading...");

        //우선 주민등록 신고하자.
        GameManager.Instance.SetCurrentWorld(this);

        pool = new PoolManager();
        yield return pool.Initiate();
        yield return pool.SetPool(prefabPoolList);

        worldCamera = new CameraManager();
        yield return worldCamera.Initiate();

        //업데이트 등록
        GameManager.ManagersUpdate -= WorldUpdates;
        GameManager.ManagersUpdate += WorldUpdates;
        GameManager.ManagersFixedUpdate -= WorldFixedUpdates;
        GameManager.ManagersFixedUpdate += WorldFixedUpdates;

        GameManager.TurnOffBasicLoadingCanvas();
    }

    /// <summary>
    /// 월드 매니저가 죽을 때 해야할 일
    /// </summary>
    public virtual void WorldManagerDestroy()
    {
        //업데이트 해체
        GameManager.ManagersUpdate -= WorldUpdates;
        GameManager.ManagersFixedUpdate -= WorldFixedUpdates;
    }

}