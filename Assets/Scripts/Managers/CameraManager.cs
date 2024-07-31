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
    private List<CameraTarget> cameraTargetLStack;


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

        cameraTargetLStack = new List<CameraTarget>();
    }


    /// <summary>
    /// ī�޶� Ÿ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="cameraType">�����ϰ��� �ϴ� ī�޶� Ÿ��</param>
    public void CameraSet(CameraType cameraType)
    {
        CurrentCameraType = cameraType;
    }
    /// <summary>
    /// ī�޶� Ÿ���� �߰��ϰ�, Ÿ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="cameraTarget">�߰��ϰ��� �ϴ� ī�޶� Ÿ��</param>
    /// <param name="cameraType">�����ϰ��� �ϴ� ī�޶� Ÿ��</param>
    public void CameraSet(CameraTarget cameraTarget, CameraType cameraType)
    {
        cameraTargetLStack.Insert(0, cameraTarget);
        CurrentCameraType = cameraType;
    }
    /// <summary>
    /// ī�޶� Ÿ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="cameraTarget">�����ϰ��� �ϴ� ī�޶� Ÿ��</param>
    public void CameraBreak(CameraTarget cameraTarget)
    {
#if UNITY_EDITOR
        if(!cameraTargetLStack.Remove(cameraTarget))
        {
            Debug.LogError("CameraManager doesn't have [cameraTarget].");
        }
#else
        cameraTargetLStack.Remove(cameraTarget);
#endif
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

        FirstViewCameraData targetData = cameraTargetLStack[0].FirstViewCameraSet();
        mainCamera.transform.position = targetData.cameraPosition;
        mainCamera.transform.forward = targetData.cameraForward;


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

        ThirdViewCameraData targetData = cameraTargetLStack[0].ThirdViewCameraSet();
        


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
public interface CameraTarget
{
    public FirstViewCameraData  FirstViewCameraSet();
    public ThirdViewCameraData  ThirdViewCameraSet();
}

/// <summary>
/// 1��â ī�޶� ���� ����
/// </summary>
public class FirstViewCameraData
{
    //ī�޶� ��ġ
    public Vector3 cameraPosition;
    //ī�޶� ����
    public Vector3 cameraForward;
}
/// <summary>
/// 3��Ī ī�޶� ���� ����
/// </summary>
public class ThirdViewCameraData
{
    public Vector3 cameraPosition; //ī�޶� ��ġ
    public Vector3 cameraRotation_Horizontal;
    public Vector3 cameraRotation_Vertical;
    
    public int maxDistance; //3��Ī �信��, Ÿ�ٰ� ī�޶� ������ �Ÿ�
    
    public bool blockingCheck; //3��Ī �信��, ���濡 ���� ���� �� �̸� ����Ͽ� ī�޶� ��� ������, �� �������� ���� �Ǵ� ����

}