using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UniqueTool : MovablePositionObject
{
    protected List<FuncInteractionData> holdingFuncInteractionList;

    protected float angle;
    protected Character holdingCharacter;

    //잡힐 지점
    [SerializeField]
    protected Vector3 catchedLocalPosition;
    protected Vector3 CatchedLocalPosition
    {
        get
        {
            return transform.rotation * catchedLocalPosition;
        }
    }


#if UNITY_EDITOR

    public virtual Vector3 CatchedLocalPositionEdit
    {
        get
        {
            return transform.position + catchedLocalPosition;
        }
        set
        {
            catchedLocalPosition = value - transform.position;
        }
    }

#endif

    protected override void Initialize()
    {
        base.Initialize();

        holdingFuncInteractionList = new List<FuncInteractionData>();

    }

    public virtual void PutTool()
    {
        //타겟 리지드바디 초기화
        currentRigidbody = initialRigidbody;

        //기존 본인 설정 초기화
        physicsInteractionObjectCollider.isTrigger = false;
        currentRigidbody.isKinematic = false;
        currentRigidbody.velocity = holdingCharacter.GetVelocity();
        ControllerManager.RemoveInputFuncInteraction(holdingFuncInteractionList);
        transform.parent = null;
        holdingCharacter = null;

    }
    protected void CheckPutToolUpdate(float deltaTime)
    {

    }
    protected void PutToolEnd()
    {
        
    }
    public virtual void PickUpTool(Character source)
    {
        //기존 본인 설정 세팅
        transform.parent = source.transform;
        holdingCharacter = source;
        currentRigidbody.isKinematic = true;
        physicsInteractionObjectCollider.isTrigger = true;
        FakeCenterPosition = holdingCharacter.GetCatchingPosition();
        ControllerManager.AddInputFuncInteraction(holdingFuncInteractionList);

        //타겟 리지드바디 설정
        currentRigidbody = holdingCharacter.CurrentRigidbody;
    }


    public List<FuncInteractionData> GetHoldingFuncInteractionList()
    {
        return holdingFuncInteractionList;
    }




    /// <summary>
    /// 거짓 월드 중심 좌표. 이 값을 바꾸면, 이 거짓 월드 중심 좌표를 기준으로하는 좌표로 위치가 옮겨진다.
    /// </summary>
    public Vector3 FakeCenterPosition
    {
        get
        {
            return transform.position + CatchedLocalPosition;
        }
        set
        {
            transform.position = value - CatchedLocalPosition;
        }
    }
    
    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 하는 캐릭터의 회전 각도. 값을 변경하면, 거짓 월드 중심 좌표를 기준으로 회전한다.
    /// </summary>
    public Quaternion FakeCenterRotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            Vector3 beforePosition = FakeCenterPosition;
            transform.rotation = value;
            FakeCenterPosition = beforePosition;
        }
    }

    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 하는 캐릭터의 Up 방향 벡터. 거짓 월드 중심 좌표를 기준으로 특정 방향을 바라본다.
    /// </summary>
    public Vector3 FakeCenterUp
    {
        get
        {
            return transform.up;
        }
        set
        {
            Vector3 beforePosition = FakeCenterPosition;
            transform.up = value;
            FakeCenterPosition = beforePosition;
        }
    }

    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 eulerAnlge을 설정한다.
    /// </summary>
    /// <param name="eulerAngle">기준 eulerAngle</param>
    public void SetFakeCenterEulerAngle(Vector3 eulerAngle)
    {
        Vector3 tempt = FakeCenterPosition;
        transform.eulerAngles = eulerAngle;
        FakeCenterPosition = tempt;
    }

    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로, 현재 회전 상태에서 주어진 쿼터니언 회전만큼을 추가한다.
    /// </summary>
    /// <param name="quaternionRot">쿼터니언 회전 값</param>
    public void SetFakeCenterQuaternionProductRotation(Quaternion quaternionRot)
    {
        Vector3 tempt = FakeCenterPosition;
        transform.rotation *= quaternionRot;
        FakeCenterPosition = tempt;
    }

    public delegate void ToolTrasnformTask();
    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 transform 변환을 주고 싶을 때, transform 변환 식을 매개변수로 받아, 거짓 월드 좌표를 기준으로 하는 작업을 진행한다.
    /// </summary>
    /// <param name="TransformTask">거짓 월드 중심 좌표를 기준으로 수행하고자 하는 transform 변환</param>
    public void SetFakeCenterFreeTransform(ToolTrasnformTask TransformTask)
    {
        Vector3 tempt = FakeCenterPosition;
        TransformTask();
        FakeCenterPosition = tempt;
    }

}
