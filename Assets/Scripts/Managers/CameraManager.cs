using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Manager
{

    #region 변수부

    //--------------카메라 매니저 기본 변수부--------------//
    
    //카메라매니저가 관리하는 카메라
    private Camera mainCamera;

    //카메라가 해야할 일의 전환을 위한 델리게이트
    private FixedUpdateFunction CameraFixedUpdate;

    //카메라의 타입. 타입에 따라 행위가 달라짐
    private CameraType currentCameraType;
    //카메라 타입이 변함에 따라, 카메라가 수행해야 할 일도 전환되어야 함.
    private CameraType CurrentCameraType
    {
        get
        {
            return currentCameraType;
        }
        set
        {
            switch(value)
            {
                case CameraType.FirstView :
                    {
                        CameraFixedUpdate = FirstViewCameraFixedUpdate;
                        break;
                    }
                case CameraType.ThirdView:
                    {
                        CameraFixedUpdate = ThirdViewCameraFixedUpdate;
                        break;
                    }
                default :
                    {
                        CameraFixedUpdate = DefaultCameraFixedUpdate;
                        break;
                    }
            };
            currentCameraType = value;
        }
    }

    //-----------------카메라 조정 변수부-----------------//

    //카메라매니저가 관리하는 타겟들의 Stack
    private List<ICameraTarget> cameraTargetLStack;

    #endregion

    #region 함수부

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        GameManager.TurnOnBasicLoadingCavnas("Camera Loading..");

        //변수 초기화
        VarInitialize();

        //자신의 업데이트가 누구와 함께 살고 죽는가를 생각해보자.
        GameManager.Instance.CurrentWorld.WorldFixedUpdates -= ManagerFixedUpdate;
        GameManager.Instance.CurrentWorld.WorldFixedUpdates += ManagerFixedUpdate;

        GameManager.TurnOnBasicLoadingCavnas("Camera Loading Complete!!");

    }

    /// <summary>
    /// 카메라 매니저의 기본 변수 초기화를 담당한다.
    /// </summary>
    private void VarInitialize()
    {
        mainCamera = Camera.main;
        
        currentCameraType = CameraType.Default;

        cameraTargetLStack = new List<ICameraTarget>();
    }


    /// <summary>
    /// 카메라 타입을 설정하는 함수
    /// </summary>
    /// <param name="cameraType">설정하고자 하는 카메라 타입</param>
    public void CameraSet(CameraType cameraType)
    {
        CurrentCameraType = cameraType;
    }
    /// <summary>
    /// 카메라 타겟을 추가하고, 타입을 설정하는 함수
    /// </summary>
    /// <param name="cameraTarget">추가하고자 하는 카메라 타겟</param>
    /// <param name="cameraType">설정하고자 하는 카메라 타입</param>
    public void CameraSet(ICameraTarget cameraTarget, CameraType cameraType)
    {
        cameraTargetLStack.Insert(0, cameraTarget);
        CurrentCameraType = cameraType;
    }
    /// <summary>
    /// 카메라 타겟을 제거하는 함수
    /// </summary>
    /// <param name="cameraTarget">제거하고자 하는 카메라 타겟</param>
    public void CameraBreak(ICameraTarget cameraTarget)
    {
#if UNITY_EDITOR
        if(!cameraTargetLStack.Remove(cameraTarget))
        {
            Debug.LogError("CameraManager doesn't have [cameraTarget].");
            CurrentCameraType = CameraType.Default;
        }
#else
        cameraTargetLStack.Remove(cameraTarget);
#endif
    }


    /// <summary>
    /// 1인칭 카메라 상태에서 수행해야할 카메라 동작
    /// </summary>
    /// <param name="fixedDeltaTime">FixedUpdate 수행 시간 간격</param>
    private void FirstViewCameraFixedUpdate(float fixedDeltaTime)
    {
#if UNITY_EDITOR
        if(cameraTargetLStack.Count <= 0)
        {
            Debug.LogError("Not exist [cameraTaret]");
        }
#endif

        FirstViewCameraData targetData = cameraTargetLStack[0].FirstViewCameraSet();

        mainCamera.transform.position = targetData.targetPosition;
        mainCamera.transform.forward = targetData.targetForward;

    }
    /// <summary>
    /// 3인칭 카메라 상태에서 수행해야할 카메라 동작
    /// </summary>
    /// <param name="fixedDeltaTime">FixedUpdate 수행 시간 간격</param>
    private void ThirdViewCameraFixedUpdate(float fixedDeltaTime)
    {
#if UNITY_EDITOR
        if (cameraTargetLStack.Count <= 0)
        {
            Debug.LogError("Not exist [cameraTaret]");
        }
#endif

        ThirdViewCameraData targetData = cameraTargetLStack[0].ThirdViewCameraSet();

        #region 노환준

        Quaternion rot = Quaternion.Euler(targetData.Xrot, targetData.Yrot, 0);
        mainCamera.transform.position = targetData.targetPosition;
        mainCamera.transform.rotation = rot;

        RaycastHit hit;
        float finalDistance;
        Vector3 finalDir = (mainCamera.transform.up * targetData.TPPOffsetY) + (-mainCamera.transform.forward * targetData.TPPOffsetZ);
        finalDir *= targetData.maxDistance;


        if (Physics.Linecast(mainCamera.transform.position, targetData.targetPosition + finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, targetData.minDistance, targetData.maxDistance);
        }
        else
        {
            finalDistance = targetData.maxDistance;
        }

        mainCamera.transform.position = targetData.targetPosition + finalDir.normalized * finalDistance;

        #endregion

    }
    /// <summary>
    /// 기본 카메라 상태에서 수행해야할 카메라 동작
    /// </summary>
    /// <param name="fixedDeltaTime">FixedUpdate 수행 시간 간격</param>
    private void DefaultCameraFixedUpdate(float fixedDeltaTime)
    {

    }

    public override void ManagerFixedUpdate(float fixedDeltaTime)
    {
        base.ManagerFixedUpdate(fixedDeltaTime);

        CameraFixedUpdate?.Invoke(fixedDeltaTime);
    }

    #endregion
}

/// <summary>
/// 카메라 타겟이 되고 싶은 자라면 붙이면 좋을 인터페이스
/// </summary>
public interface ICameraTarget
{
    public FirstViewCameraData  FirstViewCameraSet();
    public ThirdViewCameraData  ThirdViewCameraSet();
    //public CustomViewCameraData CustomViewCameraSet(Camera mainCamera);
}

/// <summary>
/// 1안창 카메라 설정 정보
/// </summary>
public class FirstViewCameraData
{
    //타겟 위치
    public Vector3 targetPosition;
    //타겟의 정면
    public Vector3 targetForward;
}
/// <summary>
/// 3인칭 카메라 설정 정보
/// </summary>
public class ThirdViewCameraData
{
    #region 노환준

    public Vector3 targetPosition;
    public float Xrot;
    public float Yrot;
    public int minDistance;
    public int maxDistance;
    public float TPPOffsetY;
    public float TPPOffsetZ;

    #endregion

    #region 김형모
    /*
    //타겟 위치
    public Vector3 targetPosition;
    //타겟의 정면
    public Vector3 targetForward;

    //타겟의 수평각도
    public Vector3 targetRotation_Horizontal;
    //타겟의 수직각도
    public Vector3 targetRotation_Vertical;

    //타겟과 카메라 사이의 거리
    public int maxDistance;

    //전방에 벽이 있을 때 이를 고려하여 카메라를 당길 것인지, 말 것인지에 대한 판단 변수
    public bool blockingCheck;
    */
    #endregion
}
/*
public class CustomViewCameraData
{

}
*/