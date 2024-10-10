using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;

public class CameraManager : Manager
{

    #region 변수부

    //--------------카메라 매니저 기본 변수부--------------//
    
    //카메라매니저가 관리하는 카메라
    private Camera mainCamera;

    //카메라가 해야할 일의 전환을 위한 델리게이트
    private UpdateFunction CameraLateUpdate;

    //카메라의 타입. 타입에 따라 행위가 달라짐
    private CameraViewType currentCameraType;
    //카메라 타입이 변함에 따라, 카메라가 수행해야 할 일도 전환되어야 함.
    private CameraViewType CurrentCameraType
    {
        get
        {
            return currentCameraType;
        }
        set
        {
            switch(value)
            {
                case CameraViewType.FirstView :
                    {
                        CameraLateUpdate = FirstViewCameraLateUpdate;
                        break;
                    }
                case CameraViewType.ThirdView:
                    {
                        CameraLateUpdate = ThirdViewCameraLateUpdate;
                        break;
                    }
                case CameraViewType.Custom:
                    {
                        CameraLateUpdate = CustomViewCameraLateUpdate;
                        break;
                    }
                default :
                    {
                        CameraLateUpdate = DefaultCameraLateUpdate;
                        break;
                    }
            };
            currentCameraType = value;
        }
    }

    //-----------------카메라 조정 변수부-----------------//

    //카메라매니저가 관리하는 타겟들의 Stack
    private List<CameraTargetInfo> cameraTargetLStack;


    #endregion

    #region 함수부

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        GameManager.TurnOnBasicLoadingCavnas("Camera Loading..");

        //변수 초기화
        VarInitialize();

        //자신의 업데이트가 누구와 함께 살고 죽는가를 생각해보자.
        GameManager.Instance.CurrentWorld.WorldLateUpdates -= ManagerLateUpdate;
        GameManager.Instance.CurrentWorld.WorldLateUpdates += ManagerLateUpdate;

        //GameManager.Instance.CurrentWorld.WorldUpdates -= ManagerFixedUpdate;
        //GameManager.Instance.CurrentWorld.WorldUpdates += ManagerFixedUpdate;
        yield return null;
        GameManager.TurnOffBasicLoadingCanvas();



        GameManager.TurnOnBasicLoadingCavnas("Camera Loading Complete!!");
        yield return null;
        GameManager.TurnOffBasicLoadingCanvas();


    }

    /// <summary>
    /// 카메라 매니저의 기본 변수 초기화를 담당한다.
    /// </summary>
    private void VarInitialize()
    {
        mainCamera = Camera.main;
        
        currentCameraType = CameraViewType.Default;

        cameraTargetLStack = new List<CameraTargetInfo>();

    }


    /// <summary>
    /// 카메라 타입을 설정하는 함수
    /// </summary>
    /// <param name="cameraType">설정하고자 하는 카메라 타입</param>
    public void CameraSet(CameraViewType cameraType)
    {
        CurrentCameraType = cameraType;

#if UNITY_EDITOR
        if (cameraTargetLStack.Count <= 0)
        {
            Debug.LogError("CameraManager doesn't have [cameraTarget].");
            return;
        }
#endif

        cameraTargetLStack[0].cameraType = cameraType;

    }
    /// <summary>
    /// 카메라 타겟을 추가하고, 타입을 설정하는 함수.
    /// </summary>
    /// <param name="cameraTarget">추가하고자 하는 카메라 타겟</param>
    /// <param name="cameraType">설정하고자 하는 카메라 타입</param>
    public void CameraSet(ICameraTarget cameraTarget, CameraViewType cameraType)
    {
        CameraTargetInfo info = new CameraTargetInfo(cameraTarget, cameraType);

        cameraTargetLStack.Insert(0, info);

        CurrentCameraType = cameraType;
    }
    /// <summary>
    /// 가장 최근의, 대상이 되는 카메라 타겟을 제거하는 함수
    /// </summary>
    /// <param name="cameraTarget">제거하고자 하는 카메라 타겟</param>
    public void CameraBreak(ICameraTarget cameraTarget)
    {

        #region 지우기 위한 작업

        CameraTargetInfo target = null;

        //있는지 찾기. 0번이 가장 최신이라 foreach로 돌려도 된다.
        foreach (CameraTargetInfo each in cameraTargetLStack)
        {
            if(each.cameraTarget == cameraTarget)
            {
                target = each;
                break;
            }
        }

#if UNITY_EDITOR
        //없다면
        if(target == null)
        {
            Debug.LogError("지우려는 대상이 없어요!");
            CurrentCameraType = CameraViewType.Default;
            return;
        }
#endif

        cameraTargetLStack.Remove(target);

        #endregion

        #region 지우고 난 후 비워졌는지를 확인한 후, 현재 카메라 타입을 정보에 맞게 설정
        int tempt = 0;
        foreach(CameraTargetInfo each in cameraTargetLStack)
        {
            //하나라도 있다면
            tempt++;
            break;
        }
        if (tempt == 0)
        {
            //하나라도 없다면
            CurrentCameraType = CameraViewType.Default;
        }
        else
        {
            //하나라도 있다면
            CurrentCameraType = cameraTargetLStack[0].cameraType;
        }
        #endregion

    }
    /// <summary>
    /// 대상이 되는 카메라 타겟을 전부 제거하는 함수
    /// </summary>
    /// <param name="cameraTarget">제거하고자 하는 카메라 타겟</param>
    public void CameraBreakAll(ICameraTarget cameraTarget)
    {

        cameraTargetLStack.RemoveAll((each) => each.cameraTarget == cameraTarget);

        #region 지우고 난 후 비워졌는지를 확인한 후, 현재 카메라 타입을 정보에 맞게 설정
        int tempt = 0;
        foreach (CameraTargetInfo each in cameraTargetLStack)
        {
            //하나라도 있다면
            tempt++;
            break;
        }
        if (tempt == 0)
        {
            //하나라도 없다면
            CurrentCameraType = CameraViewType.Default;
        }
        else
        {
            //하나라도 있다면
            CurrentCameraType = cameraTargetLStack[0].cameraType;
        }
        #endregion

    }


    /// <summary>
    /// 1인칭 카메라 상태에서 수행해야할 카메라 동작
    /// </summary>
    /// <param name="deltaTime">LateUpdate 수행 시간 간격</param>
    private void FirstViewCameraLateUpdate(float deltaTime)
    {
#if UNITY_EDITOR
        if(cameraTargetLStack.Count <= 0)
        {
            Debug.LogError("Not exist [cameraTaret]");
        }
#endif

        FirstViewCameraData targetData = cameraTargetLStack[0].cameraTarget.FirstViewCameraSet();

        mainCamera.transform.position = targetData.targetPosition;
        mainCamera.transform.forward = targetData.targetForward;

    }
    /// <summary>
    /// 3인칭 카메라 상태에서 수행해야할 카메라 동작
    /// </summary>
    /// <param name="deltaTime">LateUdpate 수행 시간 간격</param>
    private void ThirdViewCameraLateUpdate(float deltaTime)
    {
#if UNITY_EDITOR
        if (cameraTargetLStack.Count <= 0)
        {
            Debug.LogError("Not exist [cameraTaret]");
        }
#endif

        ThirdViewCameraData targetData = cameraTargetLStack[0].cameraTarget.ThirdViewCameraSet();

        #region 노환준

        Quaternion rot = Quaternion.Euler(targetData.Xrot, targetData.Yrot, 0);
        mainCamera.transform.position = targetData.targetPosition;
        mainCamera.transform.rotation = rot;

        RaycastHit hit;
        float finalDistance;
        Vector3 finalDir = (mainCamera.transform.up * targetData.TPPOffsetY) + (-mainCamera.transform.forward * targetData.TPPOffsetZ);
        finalDir *= targetData.maxDistance;


        if (Physics.Linecast(mainCamera.transform.position, targetData.targetPosition + finalDir, out hit) && !hit.collider.isTrigger)
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
    /// <param name="deltaTime">LateUpdate 수행 시간 간격</param>
    private void DefaultCameraLateUpdate(float deltaTime)
    {

    }

    /// <summary>
    /// Custom 카메라 동작
    /// </summary>
    /// <param name="deltaTime"></param>
    private void CustomViewCameraLateUpdate(float deltaTime)
    {
        cameraTargetLStack[0].cameraTarget.CustomViewCameraSet(mainCamera, deltaTime);
    }

    public override void ManagerLateUpdate(float deltaTime)
    {
        base.ManagerLateUpdate(deltaTime);

        CameraLateUpdate?.Invoke(deltaTime);
    }
    /*
    public override void ManagerFixedUpdate(float fixedDeltaTime)
    {
        base.ManagerFixedUpdate(fixedDeltaTime);

        CameraLateUpdate?.Invoke(fixedDeltaTime);
    }
    */
    #endregion
}

/// <summary>
/// 카메라 타겟이 되고 싶은 자라면 붙이면 좋을 인터페이스
/// </summary>
public interface ICameraTarget
{
    public FirstViewCameraData FirstViewCameraSet();
    public ThirdViewCameraData ThirdViewCameraSet();
    public void CustomViewCameraSet(Camera main, float deltaTime);
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

    public void SetInfo(Vector3 targetPosition, Vector3 targetForward)
    {
        this.targetPosition = targetPosition;
        this.targetForward = targetForward;
    }
}
/// <summary>
/// 3인칭 카메라 설정 정보
/// </summary>
public class ThirdViewCameraData
{
    #region 노환준

    public Vector3 targetPosition;
    public Vector3 targetForward;
    public float Xrot;
    public float Yrot;
    public int minDistance;
    public int maxDistance;
    public float TPPOffsetY;
    public float TPPOffsetZ;


    public void SetInfo(Vector3 targetPosition, float Xrot, float Yrot, int minDistance, int maxDistance, float TPPOffsetY, float TPPOffsetZ)
    {
        this.targetPosition = targetPosition;
        this.Xrot = Xrot;
        this.Yrot = Yrot;
        this.minDistance = minDistance;
        this.maxDistance = maxDistance;
        this.TPPOffsetY = TPPOffsetY;
        this.TPPOffsetZ = TPPOffsetZ;
    }

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



//public class CustomViewCameraData
//{

//}


/// <summary>
/// 카메라 타겟과, 그 타겟에 대응하는 현재 카메라 타입을 보관하는 클래스
/// </summary>
public class CameraTargetInfo
{
    public ICameraTarget cameraTarget;
    public CameraViewType cameraType;

    public CameraTargetInfo(ICameraTarget cameraTarget, CameraViewType cameraType)
    {
        this.cameraTarget = cameraTarget;
        this.cameraType = cameraType;
    }
}