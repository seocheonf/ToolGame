using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Manager
{

    #region ������

    //--------------ī�޶� �Ŵ��� �⺻ ������--------------//
    
    //ī�޶�Ŵ����� �����ϴ� ī�޶�
    private Camera mainCamera;

    //ī�޶� �ؾ��� ���� ��ȯ�� ���� ��������Ʈ
    private FixedUpdateFunction CameraFixedUpdate;

    //ī�޶��� Ÿ��. Ÿ�Կ� ���� ������ �޶���
    private CameraType currentCameraType;
    //ī�޶� Ÿ���� ���Կ� ����, ī�޶� �����ؾ� �� �ϵ� ��ȯ�Ǿ�� ��.
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

    //-----------------ī�޶� ���� ������-----------------//

    //ī�޶�Ŵ����� �����ϴ� Ÿ�ٵ��� Stack
    private List<CameraTargetInfo> cameraTargetLStack;


    #endregion

    #region �Լ���

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        GameManager.TurnOnBasicLoadingCavnas("Camera Loading..");

        //���� �ʱ�ȭ
        VarInitialize();

        //�ڽ��� ������Ʈ�� ������ �Բ� ��� �״°��� �����غ���.
        GameManager.Instance.CurrentWorld.WorldFixedUpdates -= ManagerFixedUpdate;
        GameManager.Instance.CurrentWorld.WorldFixedUpdates += ManagerFixedUpdate;

        GameManager.TurnOnBasicLoadingCavnas("Camera Loading Complete!!");

    }

    /// <summary>
    /// ī�޶� �Ŵ����� �⺻ ���� �ʱ�ȭ�� ����Ѵ�.
    /// </summary>
    private void VarInitialize()
    {
        mainCamera = Camera.main;
        
        currentCameraType = CameraType.Default;

        cameraTargetLStack = new List<CameraTargetInfo>();

    }


    /// <summary>
    /// ī�޶� Ÿ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="cameraType">�����ϰ��� �ϴ� ī�޶� Ÿ��</param>
    public void CameraSet(CameraType cameraType)
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
    /// ī�޶� Ÿ���� �߰��ϰ�, Ÿ���� �����ϴ� �Լ�.
    /// </summary>
    /// <param name="cameraTarget">�߰��ϰ��� �ϴ� ī�޶� Ÿ��</param>
    /// <param name="cameraType">�����ϰ��� �ϴ� ī�޶� Ÿ��</param>
    public void CameraSet(ICameraTarget cameraTarget, CameraType cameraType)
    {
        CameraTargetInfo info = new CameraTargetInfo(cameraTarget, cameraType);

        cameraTargetLStack.Insert(0, info);

        CurrentCameraType = cameraType;
    }
    /// <summary>
    /// ���� �ֱ���, ����� �Ǵ� ī�޶� Ÿ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="cameraTarget">�����ϰ��� �ϴ� ī�޶� Ÿ��</param>
    public void CameraBreak(ICameraTarget cameraTarget)
    {

        #region ����� ���� �۾�

        CameraTargetInfo target = null;

        //�ִ��� ã��. 0���� ���� �ֽ��̶� foreach�� ������ �ȴ�.
        foreach (CameraTargetInfo each in cameraTargetLStack)
        {
            if(each.cameraTarget == cameraTarget)
            {
                target = each;
                break;
            }
        }

#if UNITY_EDITOR
        //���ٸ�
        if(target == null)
        {
            Debug.LogError("������� ����� �����!");
            CurrentCameraType = CameraType.Default;
            return;
        }
#endif

        cameraTargetLStack.Remove(target);

        #endregion

        #region ����� �� �� ����������� Ȯ���� ��, ���� ī�޶� Ÿ���� ������ �°� ����
        int tempt = 0;
        foreach(CameraTargetInfo each in cameraTargetLStack)
        {
            //�ϳ��� �ִٸ�
            tempt++;
            break;
        }
        if (tempt == 0)
        {
            //�ϳ��� ���ٸ�
            CurrentCameraType = CameraType.Default;
        }
        else
        {
            //�ϳ��� �ִٸ�
            CurrentCameraType = cameraTargetLStack[0].cameraType;
        }
        #endregion

    }
    /// <summary>
    /// ����� �Ǵ� ī�޶� Ÿ���� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="cameraTarget">�����ϰ��� �ϴ� ī�޶� Ÿ��</param>
    public void CameraBreakAll(ICameraTarget cameraTarget)
    {

        cameraTargetLStack.RemoveAll((each) => each.cameraTarget == cameraTarget);

        #region ����� �� �� ����������� Ȯ���� ��, ���� ī�޶� Ÿ���� ������ �°� ����
        int tempt = 0;
        foreach (CameraTargetInfo each in cameraTargetLStack)
        {
            //�ϳ��� �ִٸ�
            tempt++;
            break;
        }
        if (tempt == 0)
        {
            //�ϳ��� ���ٸ�
            CurrentCameraType = CameraType.Default;
        }
        else
        {
            //�ϳ��� �ִٸ�
            CurrentCameraType = cameraTargetLStack[0].cameraType;
        }
        #endregion

    }


    /// <summary>
    /// 1��Ī ī�޶� ���¿��� �����ؾ��� ī�޶� ����
    /// </summary>
    /// <param name="fixedDeltaTime">FixedUpdate ���� �ð� ����</param>
    private void FirstViewCameraFixedUpdate(float fixedDeltaTime)
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
    /// 3��Ī ī�޶� ���¿��� �����ؾ��� ī�޶� ����
    /// </summary>
    /// <param name="fixedDeltaTime">FixedUpdate ���� �ð� ����</param>
    private void ThirdViewCameraFixedUpdate(float fixedDeltaTime)
    {
#if UNITY_EDITOR
        if (cameraTargetLStack.Count <= 0)
        {
            Debug.LogError("Not exist [cameraTaret]");
        }
#endif

        ThirdViewCameraData targetData = cameraTargetLStack[0].cameraTarget.ThirdViewCameraSet();

        #region ��ȯ��

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
    /// �⺻ ī�޶� ���¿��� �����ؾ��� ī�޶� ����
    /// </summary>
    /// <param name="fixedDeltaTime">FixedUpdate ���� �ð� ����</param>
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
/// ī�޶� Ÿ���� �ǰ� ���� �ڶ�� ���̸� ���� �������̽�
/// </summary>
public interface ICameraTarget
{
    public FirstViewCameraData FirstViewCameraSet();
    public ThirdViewCameraData ThirdViewCameraSet();
    //public CustomViewCameraData CustomViewCameraSet(Camera mainCamera);
}

/// <summary>
/// 1��â ī�޶� ���� ����
/// </summary>
public class FirstViewCameraData
{
    //Ÿ�� ��ġ
    public Vector3 targetPosition;
    //Ÿ���� ����
    public Vector3 targetForward;

    public FirstViewCameraData(Vector3 targetPosition, Vector3 targetForward)
    {
        this.targetPosition = targetPosition;
        this.targetForward = targetForward;
    }
}
/// <summary>
/// 3��Ī ī�޶� ���� ����
/// </summary>
public class ThirdViewCameraData
{
    #region ��ȯ��

    public Vector3 targetPosition;
    public float Xrot;
    public float Yrot;
    public int minDistance;
    public int maxDistance;
    public float TPPOffsetY;
    public float TPPOffsetZ;

    public ThirdViewCameraData(Vector3 targetPosition, float Xrot, float Yrot, int minDistance, int maxDistance, float TPPOffsetY, float TPPOffsetZ)
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

    #region ������
    /*
    //Ÿ�� ��ġ
    public Vector3 targetPosition;
    //Ÿ���� ����
    public Vector3 targetForward;

    //Ÿ���� ���򰢵�
    public Vector3 targetRotation_Horizontal;
    //Ÿ���� ��������
    public Vector3 targetRotation_Vertical;

    //Ÿ�ٰ� ī�޶� ������ �Ÿ�
    public int maxDistance;

    //���濡 ���� ���� �� �̸� ����Ͽ� ī�޶� ��� ������, �� �������� ���� �Ǵ� ����
    public bool blockingCheck;
    */
    #endregion
}
/*
public class CustomViewCameraData
{

}
*/

/// <summary>
/// ī�޶� Ÿ�ٰ�, �� Ÿ�ٿ� �����ϴ� ���� ī�޶� Ÿ���� �����ϴ� Ŭ����
/// </summary>
public class CameraTargetInfo
{
    public ICameraTarget cameraTarget;
    public CameraType cameraType;

    public CameraTargetInfo(ICameraTarget cameraTarget, CameraType cameraType)
    {
        this.cameraTarget = cameraTarget;
        this.cameraType = cameraType;
    }
}