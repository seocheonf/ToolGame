using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적용중인 상태이상에 대한 정보
public class AffectedCrowdControl
{
    public readonly CrowdControlState crowdControlState;
    public float remainTime;
    public int tolerance;

    public AffectedCrowdControl(CrowdControlState crowdControlState, float remainTime, int tolerance = 0)
    {
        this.crowdControlState = crowdControlState;
        this.remainTime = remainTime;
        this.tolerance = tolerance;
    }
}

public class Character : MovablePositionObject
{
    private UniqueTool currentHoldingUniqueTool;

    private Vector3 wantMoveDirection;
    private Vector3 currentMoveDirection;

    private bool isMoveForward;
    private bool isMoveBackward;
    private bool isMoveLeft;
    private bool isMoveRight;

    private bool isAir;

    private GeneralState currentGeneralState;

    CrowdControlState currentCrowdControlState;
    List<AffectedCrowdControl> affectedCrowdControlList;

    private float defaultRunningRatio;
    private float runningRatio;

    private float currentSpeed;

    private float defaultMoveSpeed;
    private float moveSpeed;

    private float defaultAccelSpeed;
    private float accelSpeed;

    private float defaultJumpPower;
    private float jumpPower;

    private Vector3 currentSightAngle;




    private void PutTool()
    {

    }
    private void PickUpTool(UniqueTool target)
    {

    }

    private void OnMoveForward()
    {
    }
    private void OnMoveBackward()
    {
    }
    private void OnMoveLeft()
    {
    }
    private void OnMoveRight()
    {
    }

    private void OnJump()
    {
        Jump();
    }
    private void Jump()
    {
    }

    private void OnRun()
    {
    }
    private void Run()
    {
    }

    private void MoveHorizontalityFixedUpdate(float fixedDeltaTime)
    {

    }

    private void ApplicationCrowdControl()
    {

    }
    public void SetCrowdControl(CrowdControlState targetCC, float duration)
    {

    }
    private void RenewalCrowdControlRemainTimeUpdate(float deltaTime)
    {

    }

    protected virtual void ChangeAngleUpdate()
    {

    }
}