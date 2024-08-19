using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적용중인 상태이상에 대한 정보
public class AffectedCrowdControl
{
    public readonly CrowdControlState crowdControlState;
    public float remainTime;

    public AffectedCrowdControl(CrowdControlState crowdControlState, float remainTime)
    {
        this.crowdControlState = crowdControlState;
        this.remainTime = remainTime;
    }
}

public class Character : MovablePositionObject
{
    protected UniqueTool currentHoldingUniqueTool;

    protected Vector3 wantMoveDirection;
    protected Vector3 currentMoveDirection;

    protected bool isMoveForward;
    protected bool isMoveBackward;
    protected bool isMoveLeft;
    protected bool isMoveRight;

    protected bool isAir;

    protected GeneralState currentGeneralState;

    CrowdControlState currentCrowdControlState;
    List<AffectedCrowdControl> affectedCrowdControlList;

    protected float defaultRunningRatio;
    protected float runningRatio;

    protected float currentSpeed;

    protected float defaultMoveSpeed;
    protected float moveSpeed;

    protected float defaultAccelSpeed;
    protected float accelSpeed;

    protected float defaultJumpPower;
    protected float jumpPower;

    protected Vector3 currentSightEulerAngle;
    //오일러각
    public virtual Vector3 CurrentSightEulerAngle
    {
        get
        {
            return currentSightEulerAngle;
        }
    }
    //쿼터니언각
    public virtual Quaternion CurrentSightQuaternionAngle
    {
        get
        {
            return Quaternion.Euler(CurrentSightEulerAngle);
        }
    }
    public virtual Vector3 CurrentSightForward
    {
        get
        {
            return Quaternion.Euler(CurrentSightEulerAngle) * Vector3.forward;
        }
    }

    //잡을 지점
    [SerializeField]
    private Vector3 catchingLocalPosition;
    protected Vector3 CatchingLocalPosition
    {
        get
        {
            return transform.rotation * catchingLocalPosition;
        }
    }
    public Vector3 GetCatchingPosition()
    {
        return transform.position + CatchingLocalPosition;
    }


#if UNITY_EDITOR

    public virtual Vector3 CatchingLocalPositionEdit
    {
        get
        {
            return transform.position + catchingLocalPosition;
        }
        set
        {
            catchingLocalPosition = value - transform.position;
        }
    }

#endif


    protected virtual void PutTool()
    {
        currentRigidbody.mass = initialMass;
        currentHoldingUniqueTool.PutTool();
        currentHoldingUniqueTool = null;
    }
    public virtual void PickUpTool(UniqueTool target)
    {
        target.PickUpTool(this);
        currentHoldingUniqueTool = target;
        currentRigidbody.mass += currentHoldingUniqueTool.InitialMass;
    }

    protected void OnMoveForward()
    {
    }
    protected void OnMoveBackward()
    {
    }
    protected void OnMoveLeft()
    {
    }
    protected void OnMoveRight()
    {
    }

    protected void OnJump()
    {
        Jump();
    }
    protected virtual void Jump()
    {
    }

    protected void OnRun()
    {
    }
    protected virtual void Run()
    {
    }

    protected virtual void MoveHorizontalityFixedUpdate(float fixedDeltaTime)
    {

    }

    protected void ApplicationCrowdControl()
    {

    }
    public void SetCrowdControl(CrowdControlState targetCC, float duration)
    {

    }
    protected void RenewalCrowdControlRemainTimeUpdate(float deltaTime)
    {

    }

    protected virtual void ChangeAngleUpdate()
    {

    }

}