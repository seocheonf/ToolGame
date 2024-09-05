using SpecialInteraction;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Umbrella : UniqueTool
{
    enum UmbrellaCondition
    {
        Open,
        Closed,
        Reverse,
        Hook,
        Length
    }

    #region 컨디션 변수

    enum UmbrellaDirection
    {
        Sight,
        Fixed,
        Hook
    }
    private UmbrellaDirection currentStandardaAngle;


    #endregion


    [SerializeField]
    UmbrellaCondition initialCondition;
    UmbrellaCondition currentCondition;

    Dictionary<UmbrellaCondition, List<FuncInteractionData>> conditionFuncInteractionDictionary;

    [SerializeField]
    private ResourceEnum.Material umbrellaMaterial;

    private Mesh umbrellaOpen;
    private Mesh umbrellaClosed;

    private MeshFilter umbrellaMeshfilter;

    [SerializeField]
    private MeshRenderer umbrellaMeshRenderer;

    [SerializeField]
    private MeshCollider umbrellaOpenCollider;
    [SerializeField]
    private MeshCollider umbrellaClosedCollider;

    // 우산의 펼침과 닫힘 여부. true면 펼침, false면 닫힘.
    private bool umbrellaMode;


    //손잡이 위치
    [SerializeField]
    protected Vector3 catchedLocalPositionKnob;
    //꽁다리 위치
    [SerializeField]
    protected Vector3 catchedLocalPositionKnobReverse;

#if UNITY_EDITOR
    enum JointPoint
    {
        Knob,
        KnobReverse
    }
    [SerializeField]
    JointPoint settingJointPoint;
    public override Vector3 CatchedLocalPositionEdit
    {
        get
        {
            if(settingJointPoint == JointPoint.Knob)
                return transform.position + catchedLocalPositionKnob;
            else
                return transform.position + catchedLocalPositionKnobReverse;
        }
        set
        {
            if (settingJointPoint == JointPoint.Knob)
                catchedLocalPositionKnob = value - transform.position;
            else
                catchedLocalPositionKnobReverse = value - transform.position;
        }
    }
#endif

    protected override void Initialize()
    {
        base.Initialize();

        //변수 초기화
        umbrellaOpen = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaOpen);
        umbrellaClosed = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaClosed);

        umbrellaMeshfilter = GetComponent<MeshFilter>();

        umbrellaMeshRenderer.material = ResourceManager.GetResource(umbrellaMaterial);

        physicsInteractionObjectCollider = umbrellaOpenCollider;

        umbrellaOpenCollider.enabled = true;
        umbrellaClosedCollider.enabled = false;
        umbrellaMeshfilter.mesh = umbrellaOpenCollider.sharedMesh;

        //기본 Joint 지점은 손잡이
        catchedLocalPosition = catchedLocalPositionKnob;

        ChangeInitialCondition(initialCondition);

        conditionFuncInteractionDictionary = new Dictionary<UmbrellaCondition, List<FuncInteractionData>>();

        //기능 등록 작업
        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsUpdate += CustomUpdate;


        //기능 준비 작업
        for(UmbrellaCondition i = 0; i<UmbrellaCondition.Length; i++)
        {
            conditionFuncInteractionDictionary[i] = new List<FuncInteractionData>();
        }

        //접혀져 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.Q, "우산 펼치기", TryOpenUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.Tab, "우산 뒤집기", TryReverseUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.F, "방향 전환", SwitchUmbrellaDirectionFixed, null, null));

        //펼쳐져 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.Q, "우산 접기", TryCloseUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.F, "방향 전환", SwitchUmbrellaDirectionFixed, null, null));

        //뒤집혀 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Reverse].Add(new FuncInteractionData(KeyCode.Tab, "우산 뒤집기", TryCloseUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Reverse].Add(new FuncInteractionData(KeyCode.CapsLock, "갈고리 걸기", TryHookOnUmbrella, null, null));

        //걸려 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Hook].Add(new FuncInteractionData(KeyCode.CapsLock, "갈고리 풀기", TryReverseUmbrellaInHook, null, null));

    }


    //우산을 열기
    private void TryOpenUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Closed)
            return;
        OpenUmbrella();
    }
    private void OpenUmbrella()
    {
        // 우산을 펼치고
        SetUmbrellaMode(true);
        // 우산 잡는 위치를 바꾸고
        SetUmbrellaJointPoint(true);

        // 플레이어가 있다면 우산 잡는 위치와 플레이어 잡는 위치를 일치 시키고
        if(holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // 우산의 Angle조정을 고정으로 설정
        SetUmbrellaDirection(UmbrellaDirection.Fixed);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Open, conditionFuncInteractionDictionary);
    }

    //우산을 닫기
    private void TryCloseUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse && currentCondition != UmbrellaCondition.Open)
            return;
        CloseUmbrella();
    }
    private void CloseUmbrella()
    {
        // 우산을 접고
        SetUmbrellaMode(false);
        // 우산 잡는 위치를 바꾸고
        SetUmbrellaJointPoint(true);

        // 플레이어가 있다면 우산 잡는 위치와 플레이어 잡는 위치를 일치 시키고
        if (holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // 우산의 Angle조정을 고정으로 설정
        SetUmbrellaDirection(UmbrellaDirection.Fixed);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Closed, conditionFuncInteractionDictionary);
    }

    //우산을 뒤집기
    private void TryReverseUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Closed && currentCondition != UmbrellaCondition.Hook)
            return;
        ReverseUmbrella();
    }
    private void ReverseUmbrella()
    {
        // 우산을 접고
        SetUmbrellaMode(false);
        // 우산 잡는 위치를 바꾸고
        SetUmbrellaJointPoint(false);

        // 플레이어가 있다면 우산 잡는 위치와 플레이어 잡는 위치를 일치 시키고
        if (holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // 우산의 Angle조정을 시선으로 설정
        SetUmbrellaDirection(UmbrellaDirection.Sight);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Reverse, conditionFuncInteractionDictionary);
    }

    //우산을 걸기
    private void TryHookOnUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse)
            return;

        HookOnUmbrella();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (holdingCharacter == null)
            return;

        //holdingCharacter.CurrentSightEulerAngle <= 이놈은 시야각을 나타내는 오일러 각
        Vector3 sightForward = holdingCharacter.CurrentSightForward;

        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(FakeCenterPosition, FakeCenterPosition + sightForward * 5f);

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(FakeCenterPosition + sightForward * 5f, Quaternion.Euler(holdingCharacter.CurrentSightEulerAngle), Vector3.one);
            Gizmos.matrix = rotationMatrix;

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 4);

        }

    }

    private void HookOnUmbrella()
    {
        //이 작업은 잡고 있는 캐릭터가 있을 때 실행되긴 하나, 혹시모르니 null체크
        if (holdingCharacter == null)
            return;
        
        Vector3 sightForward = holdingCharacter.CurrentSightForward;
        Collider[] hithit = Physics.OverlapBox(FakeCenterPosition + sightForward * 5f, Vector3.one * 2, holdingCharacter.CurrentSightQuaternionAngle);

        //가장 가까운 놈 캐칭
        Vector3 dir = Vector3.one * float.MaxValue;
        UmbrellaHookTarget resultTarget = null;
        foreach(Collider each in hithit)
        {
            if(each.TryGetComponent(out UmbrellaHookTarget result))
            {
                Vector3 dirSub = FakeCenterPosition - each.transform.position;
                float magSub = dirSub.magnitude;
                if (magSub < dir.magnitude)
                {
                    dir = dirSub;
                    resultTarget = result;
                }
            }
        }
        if (resultTarget == null)
        {
            return;
        }
        
        // 우산을 접고
        SetUmbrellaMode(false);
        // 우산 잡는 위치를 바꾸고
        SetUmbrellaJointPoint(false);

        // 플레이어가 있다면 우산 잡는 위치와 플레이어 잡는 위치를 일치 시키고
        if (holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // 우산의 Angle조정을 자유로 결정
        SetUmbrellaDirection(UmbrellaDirection.Hook);

        // Joint를 거는 과정
        SetSpringJoint(resultTarget);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Hook, conditionFuncInteractionDictionary);
    }

    //Hook상태에서 Reverse로 전환할 때 실행할 함수
    private void TryReverseUmbrellaInHook()
    {
        if (currentCondition != UmbrellaCondition.Hook)
            return;

        ReverseUmbrellaInHook();
    }
    private void ReverseUmbrellaInHook()
    {
        HookOffUmbrella();
        ReverseUmbrella();
    }

    //Hook상태를 풀고 싶을 때 실행할 함수
    private void HookOffUmbrella()
    {
        UnSetSpringJoint();
        hookTarget = null;
    }

    //hook을 설정하는 함수
    //우산이 연결시킨 힌지 정보
    private HingeJoint hookHinge;
    //내가 걸려있는 hookTarget정보
    private UmbrellaHookTarget hookTarget;
    private void SetSpringJoint(UmbrellaHookTarget target)
    {
        hookHinge = holdingCharacter.CurrentRigidbody.AddComponent<HingeJoint>();
        hookHinge.autoConfigureConnectedAnchor = false;
        hookHinge.connectedAnchor = Vector3.zero;
        hookHinge.connectedBody = target.HookRigid;
        hookHinge.anchor = holdingCharacter.CatchingLocalPositionOrigin;
        float umbLength = (catchedLocalPositionKnobReverse - catchedLocalPositionKnob).magnitude;
        target.hinge.anchor = Vector3.up * umbLength;
        hookTarget = target;
    }
    //hook을 푸는 함수
    private void UnSetSpringJoint()
    {
        Destroy(hookHinge);
        hookHinge = null;
    }
    

    private void SetSpringJointAlone(UmbrellaHookTarget target)
    {
        hookHinge = gameObject.AddComponent<HingeJoint>();
        hookHinge.autoConfigureConnectedAnchor = false;
        hookHinge.connectedAnchor = Vector3.zero;
        hookHinge.connectedBody = target.HookRigid;
        hookHinge.anchor = catchedLocalPositionKnob;
        target.hinge.anchor = Vector3.zero;
        hookTarget = target;
    }
    private void UnSetSpringJointAlone()
    {
        UnSetSpringJoint();
    }

    //우산안에서 들거나 놓을 때 해야할 일들
    private void PickUpToolTask()
    {
        if(currentStandardaAngle == UmbrellaDirection.Hook)
        {
            UnSetSpringJointAlone();
            SetSpringJoint(hookTarget);
        }
    }
    private void PutToolTask()
    {
        if (currentStandardaAngle == UmbrellaDirection.Hook)
        {
            UnSetSpringJoint();
            SetSpringJointAlone(hookTarget);
        }
    }


    //우산의 방향성 설정
    private void SetUmbrellaDirection(UmbrellaDirection dir)
    {
        currentStandardaAngle = dir;
        if(currentStandardaAngle == UmbrellaDirection.Fixed)
        {
            ChangeUmbrellaDirectionFixed(currentFixedAngle);
        }
    }

    enum UmbrellaDirectionFixed
    {
        Forward,
        Up,
        Backward,
        Down,
        Length
    }

    private UmbrellaDirectionFixed currentFixedAngle;

    private void SwitchUmbrellaDirectionFixed()
    {
        if (currentFixedAngle == (UmbrellaDirectionFixed.Length - 1))
            currentFixedAngle = 0;
        else
            currentFixedAngle += 1;

        ChangeUmbrellaDirectionFixed(currentFixedAngle);
    }

    private void ChangeUmbrellaDirectionFixed(UmbrellaDirectionFixed dir)
    {
        currentFixedAngle = dir;
        if (holdingCharacter != null)
        {
            switch (currentFixedAngle)
            {
                case UmbrellaDirectionFixed.Forward:
                    //FakeCenterRotation = Quaternion.LookRotation(-holdingCharacter.transform.up);
                    //FakeCenterForward = -holdingCharacter.transform.up;
                    //FakeCenterRotation = Quaternion.Euler(90,0,0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(holdingCharacter.transform.forward);
                    //beforePosition = FakeCenterPosition;
                    //transform.up = holdingCharacter.transform.forward;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = holdingCharacter.transform.forward;
                    break;
                case UmbrellaDirectionFixed.Backward:
                    //FakeCenterRotation = Quaternion.LookRotation(-holdingCharacter.transform.up);
                    //FakeCenterForward = holdingCharacter.transform.up;
                    //FakeCenterRotation = Quaternion.Euler(-90, 0, 0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(-holdingCharacter.transform.forward);
                    //beforePosition = FakeCenterPosition;
                    //transform.up = -holdingCharacter.transform.forward;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = -holdingCharacter.transform.forward;
                    break;
                case UmbrellaDirectionFixed.Up:
                    //FakeCenterRotation = Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterForward = holdingCharacter.transform.forward;
                    //FakeCenterRotation = Quaternion.Euler(0, 0, 0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(holdingCharacter.transform.up);
                    //beforePosition = FakeCenterPosition;
                    //transform.up = holdingCharacter.transform.up;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = holdingCharacter.transform.up;
                    break;
                case UmbrellaDirectionFixed.Down:
                    //FakeCenterRotation = Quaternion.LookRotation(-holdingCharacter.transform.forward);
                    //FakeCenterForward = -holdingCharacter.transform.forward;
                    //FakeCenterRotation = Quaternion.Euler(180, 0, 0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(-holdingCharacter.transform.up);
                    //beforePosition = FakeCenter   Position;
                    //transform.up = -holdingCharacter.transform.up;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = -holdingCharacter.transform.up;
                    break;
            }
        }
    }

    private void ChangeInitialCondition(UmbrellaCondition newCondition)
    {
        currentCondition = newCondition;
        
        switch (currentCondition)
        {
            case UmbrellaCondition.Open:
                OpenUmbrella();
                break;
            case UmbrellaCondition.Closed:
                CloseUmbrella();
                break;
            case UmbrellaCondition.Reverse:
                ReverseUmbrella();
                break;
            case UmbrellaCondition.Hook:
            default:
                currentCondition = UmbrellaCondition.Open;
                OpenUmbrella();
                break;
        }
    }

    /// <summary>
    /// 우산의 펼침과 닫힘을 설정하는 함수
    /// </summary>
    /// <param name="mode">true면 펼침, false면 닫힘</param>
    private void SetUmbrellaMode(bool mode)
    {
        umbrellaMode = mode;
        //정보설정
        if (umbrellaMode)
        {
            umbrellaOpenCollider.enabled = true;
            umbrellaClosedCollider.enabled = false;
            umbrellaMeshfilter.mesh = umbrellaOpen;
            physicsInteractionObjectCollider = umbrellaOpenCollider;
        }
        else
        {
            umbrellaClosedCollider.enabled = true;
            umbrellaOpenCollider.enabled = false;
            umbrellaMeshfilter.mesh = umbrellaClosed;
            physicsInteractionObjectCollider = umbrellaClosedCollider;
        }
    }

    /// <summary>
    /// 우산을 들고 있는 여부에 따라 collider isTrigger on/off 설정.\n
    /// 추후 다른 곳에서 사용시 null체크 관련해서 수정할 것.
    /// </summary>
    private void ChangeUmbrellaStatus()
    {
        //들고 있냐에 따라 바꿈
        if(holdingCharacter != null)
        {
            umbrellaOpenCollider.isTrigger = true;
            umbrellaClosedCollider.isTrigger = true;
        }
        else
        {
            umbrellaOpenCollider.isTrigger = false;
            umbrellaClosedCollider.isTrigger = false;
        }
    }

    /// <summary>
    /// 우산을 잡을 지점을 설정하는 함수
    /// </summary>
    /// <param name="jointPoint">true면 손잡이, false면 꽁다리</param>
    private void SetUmbrellaJointPoint(bool jointPoint)
    {
        if (jointPoint)
        {
            catchedLocalPosition = catchedLocalPositionKnob;
        }
        else
        {
            catchedLocalPosition = catchedLocalPositionKnobReverse;
        }
    }

    private void ChangeCondition<T>(ref T currentCondition, T newCondition, Dictionary<T, List<FuncInteractionData>> conditionFunc) where T : System.Enum
    {
        //기능 넣고 빼기는 들고 있을 때만.
        if (holdingCharacter != null)
        {
            //current의 List를 remove요청, change의 List를 add요청
            ControllerManager.RemoveInputFuncInteraction(conditionFunc[currentCondition]);
            //바뀐 놈 List로 갱신
            ControllerManager.AddInputFuncInteraction(conditionFunc[newCondition]);
        }
        //마지막에 식별해주는 친구 바꾸기
        currentCondition = newCondition;
    }

    private void ChangeUmbrellaDirectionSight()
    {
        //캐릭터의 시야를 바라보게 설정
        SetFakeCenterEulerAngle(holdingCharacter.CurrentSightEulerAngle);
        //대상의 방향을 추가로 조정하는 과정. 기본 우산의 forward를 고려하는 것과, 추가 회전을 통해 눈에 잘 보이게 하기 위한 작업
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(-120, 22.5f, 0));

        /*
        Vector3 tempt = FakeCenterPosition;
        transform.eulerAngles = holdingCharacter.CurrentSightEulerAngle;
        transform.rotation *= Quaternion.Euler(-90, 0, 0);
        FakeCenterPosition = tempt;
        */
    }
    
    private void ChangeUmbrellaDirectionHook()
    {
        Vector3 dir = FakeCenterPosition - hookTarget.transform.parent.position;
        FakeCenterUp = dir;
    }

    private void ChangeUmbrellaDirectionFixedUpdate(float fixedDeltaTime)
    {
        if(holdingCharacter != null)
        {
            switch(currentStandardaAngle)
            {
                case UmbrellaDirection.Fixed:
                    //ChangeUmbrellaDirectionFixed(currentFixedAngle);
                    break;
                case UmbrellaDirection.Sight:
                    //이것도 한번만 하게 할 수 있을 듯.
                    ChangeUmbrellaDirectionSight();
                    break;
                case UmbrellaDirection.Hook:
                    ChangeUmbrellaDirectionHook();
                    break;
            }
        }

    }




    //--------------------


    public override void GetSpecialInteraction(WindData source)
    {

        //힘 보정 필요
      

        //일반적인 힘 작용
        base.GetSpecialInteraction(source);

        //특수작용 - 펼쳐져 있을 때만
        if (umbrellaMode)
        {
            float dotValue = Vector3.Dot(transform.up, source.Direction);
            AddForce(new ForceInfo(transform.up * dotValue * 5f * source.intensity, ForceType.VelocityForce));
        }

    }

    private void CustomUpdate(float deltaTime)
    {
        /*
        //ChangeUmbrellaDirectionUpdate(deltaTime);
        Vector3 wantDirection = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //wantDirection += Vector3.left;

            //holdingCharacter.CurrentRigidbody.MovePosition(transform.position + Vector3.left);
            holdingCharacter.AddForce(Vector3.left * 5f, ForceType.VelocityForce);
            //holdingCharacter.CurrentRigidbody.AddForce(Vector3.left * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //wantDirection += Vector3.right;

            //holdingCharacter.CurrentRigidbody.MovePosition(transform.position + Vector3.right);
            holdingCharacter.AddForce(Vector3.right * 5f, ForceType.VelocityForce);
            //holdingCharacter.CurrentRigidbody.AddForce(Vector3.right * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //wantDirection += Vector3.forward;

            //holdingCharacter.CurrentRigidbody.MovePosition(transform.position + Vector3.forward);
            holdingCharacter.AddForce(Vector3.forward * 5f, ForceType.VelocityForce);
            //holdingCharacter.CurrentRigidbody.AddForce(Vector3.forward * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //wantDirection += Vector3.back;

            //holdingCharacter.CurrentRigidbody.MovePosition(transform.position + Vector3.back);
            holdingCharacter.AddForce(Vector3.back * 5f, ForceType.VelocityForce);
            //holdingCharacter.CurrentRigidbody.AddForce(Vector3.back * 5f, ForceMode.Impulse);
        }
        */
    }

    protected override void MainFixedUpdate(float fixedDeltaTime)
    {
        ChangeUmbrellaDirectionFixedUpdate(fixedDeltaTime);
        if(GetDownSpeed() <= 0 && umbrellaMode)
        {
            float dotValue = Vector3.Dot(Vector3.up, transform.up);
            AccelDownForce(1 - dotValue * 0.05f);
        }
        base.MainFixedUpdate(fixedDeltaTime);
    }


    /* Legacy
    private void CustomFixedUpdate(float fixedDeltaTime)
    {
        ChangeUmbrellaDirectionFixedUpdate(fixedDeltaTime);
        ApplyForce();
    }

    private void ApplyForce()
    {
        if(holdingCharacter == null)
        {
            //하강 중일 때, 하강 속도 감소
            if (currentRigidbody.velocity.y <= 0 && umbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                Vector3 downVelocity = currentRigidbody.velocity;
                downVelocity.y *= (1 - dotValue * 0.05f);
                currentRigidbody.velocity = downVelocity;
            }
        }
        else
        {
            if (holdingCharacter.GetDownSpeed() <= 0 && umbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                holdingCharacter.AccelDownForce((1 - dotValue * 0.05f));
            }
        }

        while (receivedForceQueue.TryDequeue(out ForceInfo result))
        {
            AddForce(result);
        }
    }
    */

    public override void PutTool()
    {
        if (holdingCharacter == null)
        {
            Debug.LogError("도구가 들려있지 않은데 놓지 마세요!!");
            return;
        }

        PutToolTask();

        //타겟 리지드바디 초기화
        currentRigidbody = initialRigidbody;

        //기존 본인 설정 초기화
        physicsInteractionObjectCollider.isTrigger = false;
        currentRigidbody.isKinematic = false;
        currentRigidbody.velocity = holdingCharacter.GetVelocity();
        ControllerManager.RemoveInputFuncInteraction(conditionFuncInteractionDictionary[currentCondition]);
        transform.parent = null;
        holdingCharacter = null;
        ChangeUmbrellaStatus();
    }

    public override void PickUpTool(Character source)
    {
        if (holdingCharacter != null)
        {
            Debug.LogError("도구가 이미 들려있는데 들려고 하지 마세요!!");
            return;
        }

        //기존 본인 설정 세팅
        transform.parent = source.transform;
        holdingCharacter = source;
        currentRigidbody.isKinematic = true;
        physicsInteractionObjectCollider.isTrigger = true;
        ChangeUmbrellaStatus();
        FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        SetUmbrellaDirection(currentStandardaAngle);

        ControllerManager.AddInputFuncInteraction(conditionFuncInteractionDictionary[currentCondition]);

        //타겟 리지드바디 설정
        currentRigidbody = holdingCharacter.CurrentRigidbody;

        PickUpToolTask();
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //기능 해제 작업

        GameManager.ObjectsUpdate -= CustomUpdate;
        

    }
}
