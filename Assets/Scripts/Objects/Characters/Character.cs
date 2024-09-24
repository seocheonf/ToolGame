using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

    protected CapsuleCollider characterCollider;
    protected UniqueTool currentHoldingUniqueTool;

    protected Vector3 wantMoveDirection;
    protected Vector3 currentMoveDirection;

    protected bool isMoveForward;
    protected bool isMoveBackward;
    protected bool isMoveLeft;
    protected bool isMoveRight;

    protected bool isAir;
    protected bool isJump;

    [SerializeField] protected Animator anim;

    [SerializeField]
    protected GeneralState currentGeneralState;
    protected CrowdControlState currentCrowdControlState;     //현재 걸려있는 CC기
    List<AffectedCrowdControl> affectedCrowdControlList;
    Dictionary<CrowdControlState, AffectedCrowdControl> affectedCrowdControlDict = new();   //List -> Dictionary 로 교체

    //달리기 속도 비율...? (일단 내가 달리기 구현했을 땐 비율을 안쓰긴했음)
    protected float defaultRunningRatio;
    protected float runningRatio;

    //현재 속도 (구현했을 땐 현재 속도를 안쓰긴 했음)
    protected float currentSpeed; //아마 rigidbody.velocity.magnitude 가 currentSpeed 랑 같을거임

    //기본 움직임 속도
    [SerializeField] protected float defaultMoveSpeed;
    protected float moveSpeed;

    //달리기 속도
    [SerializeField] protected float defaultAccelSpeed;
    protected float accelSpeed;
    protected bool isAccel = false;

    //최대속도 (추가)
    [SerializeField] float defaultMaxSpeed;
    private float maxSpeed;

    [SerializeField] protected float defaultJumpPower;
    protected float jumpPower;

    //시선 오일러각 - 근원의 시선
    protected Vector3 currentSightEulerAngle_Origin;
    //오일러각
    public virtual Vector3 CurrentSightEulerAngle_Origin
    {
        get
        {
            return currentSightEulerAngle_Origin;
        }
    }
    
    //쿼터니언각
    public virtual Quaternion CurrentSightQuaternionAngle_Origin
    {
        get
        {
            return Quaternion.Euler(CurrentSightEulerAngle_Origin);
        }
    }
    public virtual Vector3 CurrentSightForward_Origin
    {
        get
        {
            return Quaternion.Euler(CurrentSightEulerAngle_Origin) * Vector3.forward;
        }
    }

    //시선 오일러각 - 상호작용 시선
    protected Vector3 currentSightEulerAngle_Interaction;
    //오일러각
    public virtual Vector3 CurrentSightEulerAngle_Interaction
    {
        get
        {
            if (currentGeneralState == GeneralState.CrowdControl)
                return currentSightEulerAngle_Interaction;
            else
            {
                currentSightEulerAngle_Interaction = CurrentSightEulerAngle_Origin;
                return currentSightEulerAngle_Interaction;
            }
        }
    }
    //쿼터니언각
    public virtual Quaternion CurrentSightQuaternionAngle_Interaction
    {
        get
        {
            return Quaternion.Euler(CurrentSightEulerAngle_Interaction);
        }
    }
    public virtual Vector3 CurrentSightForward_Interaction
    {
        get
        {
            return Quaternion.Euler(CurrentSightEulerAngle_Interaction) * Vector3.forward;
        }
    }


    //경사로인지 확인하는 변수들 (추가)
    private GameObject ground;
    protected Ray moveRay;
    Dictionary<GameObject, Vector3> attachedCollision = new();

    private bool _isGround = false;
    public bool IsGround => _isGround;

    //잡을 지점
    [SerializeField]
    private Vector3 catchingLocalPosition;
    public Vector3 CatchingLocalPositionOrigin => catchingLocalPosition;
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

    //땅을 인식할 수 있는 경사로의 최대각도 설정 (추가)
    [SerializeField] float maxSlopeAngle;

    protected Vector3 _groundNormal = Vector3.down;
    protected Vector3 GroundNormal
    {
        get => _groundNormal;
        set
        {
            _groundNormal = value;
            _isGround = (value.y > 0 && Vector3.Angle(Vector3.up, value) < maxSlopeAngle);
            if (_isGround && isJump) isJump = false; 
        }
    }
    protected Vector3 planeMovementVector;
    protected Vector3 groundMovementVelocity;
    
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
        if (currentHoldingUniqueTool != null)
        {
            currentRigidbody.mass = initialMass;
            currentHoldingUniqueTool.PutTool();
            currentHoldingUniqueTool = null;
        }
    }

    public virtual void PickUpTool(UniqueTool target)
    {
        target.PickUpTool(this);
        currentHoldingUniqueTool = target;
        currentRigidbody.mass += currentHoldingUniqueTool.InitialMass;
        target.FakeCenterPosition = transform.position + CatchingLocalPosition;
    }



    #region 이동계열 함수 
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
        if (IsGround && attachedCollision.Count != 0)
        {
            Vector3 result = currentRigidbody.velocity;
            result.y = jumpPower;

            currentRigidbody.velocity = result;
            isJump = true;
        }
    }

    protected void OnRun()
    {
        Run();
    }
    protected virtual void Run()
    {
        isAccel = true;
    }

    protected void RunGetKeyUp()
    {
        isAccel = false;
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


    protected virtual void MoveHorizontalityFixedUpdate(float fixedDeltaTime)
    {
        //CheckWantMoveDirection();
        currentMoveDirection = (wantMoveDirection.x * transform.right + wantMoveDirection.z * transform.forward).normalized;
        //transform.position += FixedUpdate_Calculate_Move();
        currentRigidbody.MovePosition(transform.position + FixedUpdate_Calculate_Move());
    }

    #region 상태이상계열 함수

    private void Stun()
    {
        currentRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void ElectricShock()
    {
        currentRigidbody.constraints = RigidbodyConstraints.None;
    }

    protected virtual void ApplicationGeneralState()
    {
        switch (currentGeneralState)
        {
            case GeneralState.Normal:
                currentRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                break;
            case GeneralState.CrowdControl:
                ApplicationCrowdControl();
                break;
            case GeneralState.Action:
                break;
            default: 
                break;
        }
    }

    protected void ApplicationCrowdControl()
    {
        switch (currentCrowdControlState)
        {
            case CrowdControlState.Stun:
                Stun();
                break;
            case CrowdControlState.ElectricShcok: 
                ElectricShock(); 
                break;
            default:
                break;
        }
    }

    public virtual void SetCrowdControl(CrowdControlState targetCC, float duration)
    {
        if (!affectedCrowdControlDict.TryGetValue(targetCC, out AffectedCrowdControl value))
        {
            AffectedCrowdControl newCrowdControl = new(targetCC, duration);
            affectedCrowdControlDict.Add(targetCC, newCrowdControl);
            //TODO : 여기다가 상태이상 함 넣어보슈
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

    protected virtual void DeleteCrowdControlDict(float deltaTime)
    {
        affectedCrowdControlDict[currentCrowdControlState].remainTime = affectedCrowdControlDict[currentCrowdControlState].remainTime - deltaTime;
        if (affectedCrowdControlDict[currentCrowdControlState].remainTime <= 0)
        {
            affectedCrowdControlDict.Remove(currentCrowdControlState);
            currentGeneralState = GeneralState.Normal;
            //TODO : 여기다가도 상태이상 빠질때 함수 넣으슈
        }
    }

    #endregion

    protected virtual void ChangeAngleUpdate()
    {

    }

    bool isTest = false;
    private void OnCollisionEnter(Collision collision)
    {
        //땅이 아닌 레버나 버튼이나 가스레인지 장식물 (원형기둥) 에 닿을때마다 이미 들어있으니 가세요라 오류 수정해야됨 (해결함)
        Debug.Log(collision.relativeVelocity.sqrMagnitude);
        if (collision.relativeVelocity.sqrMagnitude > 15) 
        {
            Debug.Log("끼얏");
            isTest = true;
            return;
        } 
        if (attachedCollision.ContainsKey(collision.gameObject)) { }
        else attachedCollision.Add(collision.gameObject, collision.GetContact(0).normal);
        CalculateGround();
    }

    private void OnCollisionStay(Collision collision)
    {
        //명륜진사갈비급 오브젝트 콜라이더 Dictionary 무한 오류 제공 이벤트 수정해야됨 (근데 없다고 함 < ?????????) (해결함)
        Vector3 normal = collision.GetContact(0).normal;
        if (!attachedCollision.ContainsKey(collision.gameObject)) // <= if (attachedCollision[collision.gameObject] != null) 원래 문구
        {
            attachedCollision[collision.gameObject] = normal;
        }
        if (IsGround && isTest)
        {
            Debug.Log("호우");
            currentRigidbody.velocity = Vector3.zero;
            isTest = false;
        }
        CalculateGround();
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

    #region 경사로 관련 함수

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

    protected float FixedUpdate_Test()
    {
        Vector3 currentShiftDirection = currentMoveDirection - planeMovementVector;

        float currentMoveAmount = Mathf.Min(moveSpeed * Time.fixedDeltaTime, currentShiftDirection.magnitude);

        return currentMoveAmount;
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

        originDistance += characterCollider.radius;

        int layerMask = 1 << LayerMask.NameToLayer("Block");
        //여기 Raycast를 선이 아닌 Box로 바꿔야함
        //if (Physics.BoxCast(CalculateCenter(currentMoveAmount), CalculateHalfExtents(currentMoveAmount), CurrentSightForward_Interaction, out RaycastHit hit, transform.rotation, originDistance, layerMask)) // <= Physics.Raycast(moveRay, out RaycastHit hit, originDistance, -1 ,QueryTriggerInteraction.Ignore)
        if (Physics.Raycast(moveRay, out RaycastHit hit, originDistance, -1, QueryTriggerInteraction.Ignore))
        {
            float possibleDistance = hit.distance - characterCollider.radius;

            float impossibleDistance = originDistance - possibleDistance;

            Vector3 originVector = CurrentMovementVelocity.normalized;
            Vector3 slidingVector = Vector3.ProjectOnPlane(originVector, hit.normal.normalized);

            CurrentMovementVelocity = (originVector * possibleDistance) + (slidingVector * impossibleDistance);
        }
        
        return CurrentMovementVelocity;
    }

    private Vector3 CalculateCenter(float speed)
    {
        Vector3 capsuleCenter;
        float height = characterCollider.bounds.center.y + (characterCollider.radius / 2);
        Vector3 length = transform.position + currentMoveDirection.normalized * (characterCollider.radius + (speed / 2));
        capsuleCenter = new Vector3(length.x, height, length.z);

        return capsuleCenter;
    }

    private Vector3 CalculateHalfExtents(float speed)
    {
        float height;
        height = characterCollider.height + characterCollider.radius;
        Vector3 halfExtents = new Vector3(speed / 2, height / 2, speed / 2);
        
        return halfExtents; 
    }

    #endregion

    protected override void Initialize()
    {
        base.Initialize();
        accelSpeed = defaultAccelSpeed;
        moveSpeed = defaultMoveSpeed;
        jumpPower = defaultJumpPower;
        
    }


    protected override void RegisterFuncInInitialize()
    {
        GameManager.CharactersFixedUpdate -= MainFixedUpdate;
        GameManager.CharactersFixedUpdate += MainFixedUpdate;
    }
    protected override void RemoveFuncInDestroy()
    {
        GameManager.CharactersFixedUpdate -= MainFixedUpdate;
    }

}