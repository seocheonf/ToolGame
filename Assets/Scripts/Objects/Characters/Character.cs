using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������� �����̻� ���� ����
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
    // TODO : ���߿� �ڵ� ������ �� RigidBody �ٲ���� ��
    private UniqueTool currentHoldingUniqueTool;
    protected CapsuleCollider characterCollider;
    protected Rigidbody rb;   

    private Vector3 wantMoveDirection;
    private Vector3 currentMoveDirection;

    private bool isMoveForward;
    private bool isMoveBackward;
    private bool isMoveLeft;
    private bool isMoveRight;

    private bool isAir;

    private GeneralState currentGeneralState;   //���� ����

    CrowdControlState currentCrowdControlState;     //���� �ɷ��ִ� CC��
    List<AffectedCrowdControl> affectedCrowdControlList;
    Dictionary<CrowdControlState, AffectedCrowdControl> affectedCrowdControlDict = new();   //List -> Dictionary �� ��ü

    //�޸��� �ӵ� ����...? (�ϴ� ���� �޸��� �������� �� ������ �Ⱦ�������)
    private float defaultRunningRatio;
    private float runningRatio;

    //���� �ӵ� (�������� �� ���� �ӵ��� �Ⱦ��� ����)
    private float currentSpeed; //�Ƹ� rigidbody.velocity.magnitude �� currentSpeed �� ��������

    //�⺻ ������ �ӵ�
    [SerializeField] float defaultMoveSpeed;
    private float moveSpeed;

    //�޸��� �ӵ�
    [SerializeField] float defaultAccelSpeed;
    private float accelSpeed;
    protected bool isAccel = false;

    //�ִ�ӵ� (�߰�)
    [SerializeField] float defaultMaxSpeed;
    private float maxSpeed;

    [SerializeField] float defaultJumpPower;
    private float jumpPower;

    private Vector3 currentSightAngle;

    //�������� Ȯ���ϴ� ������ (�߰�)
    private GameObject ground;
    protected Ray moveRay;
    Dictionary<GameObject, Vector3> attachedCollision = new();

    private bool _isGround = false;
    public bool IsGround => _isGround;

    //���� �ν��� �� �ִ� ������ �ִ밢�� ���� (�߰�)
    [SerializeField] float maxSlopeAngle;

    protected Vector3 _groundNormal = Vector3.down;
    protected Vector3 GroundNormal
    {
        get => _groundNormal;
        set
        {
            _groundNormal = value;
            _isGround = (value.y > 0 && Vector3.Angle(Vector3.up, value) < maxSlopeAngle);
        }
    }
    protected Vector3 planeMovementVector;
    protected Vector3 groundMovementVelocity;

    private void PutTool()
    {

    }
    private void PickUpTool(UniqueTool target)
    {

    }

    #region �̵��迭 �Լ� 
    protected void OnMoveForward()
    {
        isMoveForward = true;
    }
    protected void OnMoveBackward()
    {
        isMoveBackward = true;
    }
    protected void OnMoveLeft()
    {
        isMoveLeft = true;
    }
    protected void OnMoveRight()
    {
        isMoveRight = true;
    }

    protected void CheckWantMoveDirection()
    {
        if (isMoveForward)
        {
            wantMoveDirection.z += 1;
        }
        if (isMoveBackward)
        {
            wantMoveDirection.z += -1;
        }
        if (isMoveLeft)
        {
            wantMoveDirection.x += -1;
        }
        if (isMoveRight)
        {
            wantMoveDirection.x += 1;
        }
    }

    protected void ResetDirection()
    {
        wantMoveDirection = Vector3.zero;
        isMoveForward = false;
        isMoveBackward = false;
        isMoveLeft = false;
        isMoveRight = false;
    }
    #endregion

    protected void OnJump()
    {
        Jump();
    }

    protected virtual void Jump()
    {
        jumpPower = defaultJumpPower;
        if (IsGround)
        {
            Vector3 result = rb.velocity;
            result.y = jumpPower;

            rb.velocity = result;
        }
    }

    protected void OnRun()
    {
        Run();
    }

    protected void RunUpdate()
    {
        if (isAccel)
        {
            moveSpeed = defaultMoveSpeed + accelSpeed;
            maxSpeed = defaultMaxSpeed + 20f;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            maxSpeed = defaultMaxSpeed;
        }

    }
    protected virtual void Run()
    {
        isAccel = true;
    }

    protected void RunGetKeyUp()
    {
        isAccel = false;
    }

    protected void MoveHorizontalityFixedUpdate(float fixedDeltaTime)
    {
        CheckWantMoveDirection();
        currentMoveDirection = (wantMoveDirection.x * transform.right + wantMoveDirection.z * transform.forward).normalized;
        if (currentMoveDirection.magnitude == 0)
        {
            Vector3 speedLimit = rb.velocity;
            speedLimit.x = Mathf.Lerp(speedLimit.x, 0, 0.1f);
            speedLimit.z = Mathf.Lerp(speedLimit.z, 0, 0.1f);
            rb.velocity = speedLimit;
        }
        else
        {
            rb.MovePosition(transform.position + FixedUpdate_Calculate_Move());
            
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            }
        }
    }

    private void Stun()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void ElectricShock()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    private void ApplicationCrowdControl()
    {
        switch (currentGeneralState)
        {
            case GeneralState.Normal:

                break;
            case GeneralState.CrowdControl:
                break;
            case GeneralState.Action:
                break;
            default: 
                break;
        }
    }

    public void SetCrowdControl(CrowdControlState targetCC, float duration)
    {
        if (!affectedCrowdControlDict.TryGetValue(targetCC, out AffectedCrowdControl value))
        {
            AffectedCrowdControl newCrowdControl = new(targetCC, duration);
            affectedCrowdControlDict.Add(targetCC, newCrowdControl);
        }
    }

    protected void RenewalCrowdControlRemainTimeUpdate(float deltaTime)
    {
        if (affectedCrowdControlDict.Count == 0)
        {
            currentGeneralState = GeneralState.Normal;
            return;
        }
        
        foreach (var crowdControl in affectedCrowdControlDict)
        {
            currentGeneralState = GeneralState.CrowdControl;
            currentCrowdControlState = crowdControl.Key;
            for (CrowdControlState i = 0; i < CrowdControlState.Length; i++)
            {
                if (crowdControl.Key <= i)
                {
                    currentCrowdControlState = crowdControl.Key;
                }
            }
        }
        DeleteCrowdControlDict(deltaTime);
    }

    private void DeleteCrowdControlDict(float deltaTime)
    {
        affectedCrowdControlDict[currentCrowdControlState].remainTime = affectedCrowdControlDict[currentCrowdControlState].remainTime - deltaTime;
        if (affectedCrowdControlDict[currentCrowdControlState].remainTime <= 0)
        {
            affectedCrowdControlDict.Remove(currentCrowdControlState);
            currentGeneralState = GeneralState.Normal;
        }
    }

    protected virtual void ChangeAngleUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        attachedCollision.Add(collision.gameObject, collision.GetContact(0).normal);
        CalculateGround();
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 normal = collision.GetContact(0).normal;
        if (attachedCollision[collision.gameObject] != null)
        {
            attachedCollision[collision.gameObject] = normal;
            CalculateGround();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        attachedCollision.Remove(collision.gameObject);
        if (collision.gameObject == ground)
        {
            ground = null;
            GroundNormal = Vector3.down;
        }
    }

    #region ���� ���� �Լ�

    protected void CalculateGround()
    {
        if (attachedCollision.Count == 0)
        {
            if (ground != null)
            {
                ground = null;
                GroundNormal = Vector3.down;
            }
        }
        else
        {
            GameObject mostGroundObject = ground;
            Vector3 mostGroundNormal;

            if (ground) mostGroundNormal = GroundNormal;
            else mostGroundNormal = Vector3.down;

            foreach (var currentTarget in attachedCollision)
            {
                if (mostGroundNormal.y < currentTarget.Value.y)
                {
                    mostGroundNormal = currentTarget.Value;
                    mostGroundObject = currentTarget.Key;
                }
            }

            ground = mostGroundObject;
            GroundNormal = mostGroundNormal;
        }
    }

    protected Vector3 FixedUpdate_Calculate_Move()
    {
        Vector3 currentShiftDirection = currentMoveDirection - planeMovementVector;

        float currentMoveAmount = Mathf.Min(moveSpeed * Time.fixedDeltaTime, currentShiftDirection.magnitude);

        planeMovementVector += currentShiftDirection.normalized * currentMoveAmount;
        groundMovementVelocity = Vector3.ProjectOnPlane(planeMovementVector * moveSpeed, GroundNormal);

        Vector3 CurrentMovementVelocity = groundMovementVelocity * Time.fixedDeltaTime;
        Vector3 capsuleBottom = characterCollider.bounds.center + (Vector3.down * (characterCollider.bounds.extents.y - characterCollider.radius));

        moveRay.origin = capsuleBottom;
        moveRay.direction = CurrentMovementVelocity;

        float originDistance = CurrentMovementVelocity.magnitude;

        if (Physics.Raycast(moveRay, out RaycastHit hit, originDistance))
        {
            float possibleDistance = hit.distance - characterCollider.radius;

            float impossibleDistance = originDistance - possibleDistance;

            Vector3 originVector = CurrentMovementVelocity.normalized;
            Vector3 slidingVector = Vector3.ProjectOnPlane(originVector, hit.normal);

            CurrentMovementVelocity = (originVector * possibleDistance) + (slidingVector * impossibleDistance);
        }

        return CurrentMovementVelocity;
    }

    #endregion
}