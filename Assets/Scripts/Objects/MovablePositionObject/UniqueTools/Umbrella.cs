using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
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
        Free
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

        ChangeCurrentCondition(initialCondition);

        conditionFuncInteractionDictionary = new Dictionary<UmbrellaCondition, List<FuncInteractionData>>();

        //기능 등록 작업
        GameManager.ObjectsFixedUpdate -= CustomUpdate;
        GameManager.ObjectsFixedUpdate += CustomUpdate;
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;


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

        //걸려 있을 때 할 일 대기
        //conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode., "우산", null, null, null));

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
    private void TryHookUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse)
            return;

        

        HookUmbrella();
    }
    private void HookUmbrella()
    {
        // 우산을 접고
        SetUmbrellaMode(false);
        // 우산 잡는 위치를 바꾸고
        SetUmbrellaJointPoint(false);

        // 플레이어가 있다면 우산 잡는 위치와 플레이어 잡는 위치를 일치 시키고
        if (holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // 우산의 Angle조정을 자유로 결정
        SetUmbrellaDirection(UmbrellaDirection.Free);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Hook, conditionFuncInteractionDictionary);
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

    private void ChangeCurrentCondition(UmbrellaCondition newCondition)
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
                HookUmbrella();
                break;
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
    /// 우산을 들고 있는 여부에 따라 collider isTrigger on/off 설정
    /// </summary>
    private void ChangeUmbrellStatus()
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
        SetFakeCenterEulerAngle(holdingCharacter.CurrentSightAngle);
        //대상의 방향을 추가로 조정하는 과정. 기본 우산의 forward를 고려하는 것과, 추가 회전을 통해 눈에 잘 보이게 하기 위한 작업
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(-120, 22.5f, 0));

        /*
        Vector3 tempt = FakeCenterPosition;
        transform.eulerAngles = holdingCharacter.CurrentSightAngle;
        transform.rotation *= Quaternion.Euler(-90, 0, 0);
        FakeCenterPosition = tempt;
        */
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
                    ChangeUmbrellaDirectionSight();
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
            receivedForceQueue.Enqueue(new ForceInfo(transform.up * dotValue * 5f * source.intensity, ForceType.VelocityForce));
        }

    }

    private void CustomUpdate(float deltaTime)
    { 
        //ChangeUmbrellaDirectionUpdate(deltaTime);

    }

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
            if (physicsInteractionObjectRigidbody.velocity.y <= 0 && umbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                Vector3 downVelocity = physicsInteractionObjectRigidbody.velocity;
                downVelocity.y *= (1 - dotValue * 0.05f);
                physicsInteractionObjectRigidbody.velocity = downVelocity;
            }

            while (receivedForceQueue.TryDequeue(out ForceInfo result))
            {
                AddForce(result);
            }
        }
        else
        {
            if (holdingCharacter.GetDownSpeed() <= 0 && umbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                holdingCharacter.AccelDownForce((1 - dotValue * 0.05f));
            }

            while (receivedForceQueue.TryDequeue(out ForceInfo result))
            {
                holdingCharacter.AddForce(result);
            }
        }
    }

    public override void PutTool()
    {
        physicsInteractionObjectCollider.isTrigger = false;
        physicsInteractionObjectRigidbody.isKinematic = false;
        physicsInteractionObjectRigidbody.velocity = holdingCharacter.GetVelocity();
        ControllerManager.RemoveInputFuncInteraction(conditionFuncInteractionDictionary[currentCondition]);
        transform.parent = null;
        holdingCharacter = null;
        ChangeUmbrellStatus();
    }

    public override void PickUpTool(Character source)
    {
        transform.parent = source.transform;
        holdingCharacter = source;
        physicsInteractionObjectRigidbody.isKinematic = true;
        physicsInteractionObjectCollider.isTrigger = true;
        ChangeUmbrellStatus();
        FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        SetUmbrellaDirection(currentStandardaAngle);

        ControllerManager.AddInputFuncInteraction(conditionFuncInteractionDictionary[currentCondition]);
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //기능 해제 작업

        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        

    }
}
