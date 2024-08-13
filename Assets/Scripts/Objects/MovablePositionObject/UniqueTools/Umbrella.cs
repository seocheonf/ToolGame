using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class UmbrellaConditionData
{
    //대상이 되는 우산
    private Umbrella targetUmbrella;
    //현재 우산 상황에서 할 수 있는 기능 모음
    private List<FuncInteractionData> holdingFuncInteractionList;

    public UmbrellaConditionData(Umbrella targetUmbrella, List<FuncInteractionData> holdingFuncInteractionList)
    {
        this.targetUmbrella = targetUmbrella;
        this.holdingFuncInteractionList = holdingFuncInteractionList;
    }

    //현재 우산 상황이 되었을 때 해야할 일
    public void SetCondition()
    {
        
    }

    //현재 우산 상황에서 빠져 나올 때 해야할 일
    public void UnsetCondition()
    {
        
    }





}

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

    private void ChangeCondition<T>(T currentCondition, T newCondition, Dictionary<T, List<FuncInteractionData>> conditionFunc) where T : System.Enum
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

    /// <summary>
    /// 우산의 펼침과 닫힘을 설정하는 함수
    /// </summary>
    /// <param name="mode">true면 펼침, false면 닫힘</param>
    private void SetUmbrellaMode(bool mode)
    {
        if (mode)
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
    /// 우산을 잡을 지점을 설정하는 함수
    /// </summary>
    /// <param name="jointPoint">true면 손잡이, false면 꽁다리</param>
    private void SetUmbrellaJointPoint(bool jointPoint)
    {
        if(jointPoint)
        {
            catchedLocalPosition = catchedLocalPositionKnob;
        }
        else
        {
            catchedLocalPosition = catchedLocalPositionKnobReverse;
        }
    }
    //손잡이 위치
    protected Vector3 catchedLocalPositionKnob;
    //꽁다리 위치
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

        currentCondition = initialCondition;

        //기본 Joint 지점은 손잡이
        catchedLocalPosition = catchedLocalPositionKnob;

        switch(currentCondition)
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

        conditionFuncInteractionDictionary = new Dictionary<UmbrellaCondition, List<FuncInteractionData>>();

        //기능 등록 작업

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;




        //기능 준비 작업
        for(UmbrellaCondition i = 0; i<UmbrellaCondition.Length; i++)
        {
            conditionFuncInteractionDictionary[i] = new List<FuncInteractionData>();
        }

        //접혀져 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.Tab, "우산 펼치기", UmbrellaModeChange, null, null));

        //펼쳐져 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Open]  .Add(new FuncInteractionData(KeyCode.Tab, "우산 접기", null, null, null));

        //뒤집혀 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.Tab, "우산", null, null, null));

        //걸려 있을 때 할 일 대기
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.Tab, "우산", null, null, null));

    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////// 상태변화도 하나의 기능으로 보는 것!
    /// </summary>
    /// 

    //우산을 열기
    private void OpenUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Closed)
            return;

        SetUmbrellaMode(true);
        SetUmbrellaJointPoint(true);

        currentCondition = UmbrellaCondition.Open;
    }
    //우산을 닫기
    private void CloseUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse && currentCondition != UmbrellaCondition.Open)
            return;

        SetUmbrellaMode(false);
        SetUmbrellaJointPoint(true);
        /////////////////////////////////////////손잡이에 맞추는 함수를 캐릭터 쪽에 필요로 할 듯.
        FakeCenterPosition = holdingCharacter.transform.position + holdingCharacter.CatchingLocalPosition;

        currentCondition = UmbrellaCondition.Closed;
    }
    //우산을 뒤집기
    private void ReverseUmbrella()
    {
        SetUmbrellaMode(false);
        SetUmbrellaJointPoint(false);

        currentCondition = UmbrellaCondition.Reverse;
    }
    //우산을 걸기
    private void HookUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse)
            return;

        SetUmbrellaMode(false);
        SetUmbrellaJointPoint(false);

        currentCondition = UmbrellaCondition.Hook;
    }


    protected override void MyDestroy()
    {
        base.MyDestroy();

        //기능 해제 작업

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;

    }

    public override void GetSpecialInteraction(WindData source)
    {

        //힘 보정 필요
      

        //일반적인 힘 작용
        base.GetSpecialInteraction(source);

        //특수작용 - 펼쳐져 있을 때만
        if (UmbrellaMode)
        {
            float dotValue = Vector3.Dot(transform.up, source.Direction);
            receivedForceQueue.Enqueue(new ForceInfo(transform.up * dotValue * 5f * source.intensity, ForceType.VelocityForce));
        }

    }

    private void CustomFixedUpdate(float fixedDeltaTime)
    {

        //FakeCenterRotation *= Quaternion.Euler(1, 0, 0);
        
        ApplyForce();

    }

    private void UmbrellaModeChange()
    {
        UmbrellaMode = !UmbrellaMode;
    }

    private void ApplyForce()
    {
        if(holdingCharacter == null)
        {
            //하강 중일 때, 하강 속도 감소
            if (physicsInteractionObjectRigidbody.velocity.y <= 0 && UmbrellaMode)
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
            if (holdingCharacter.GetDownSpeed() <= 0 && UmbrellaMode)
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

    int umbDir = 0;

    public override void PickUpTool(Character source)
    {
        base.PickUpTool(source);
        //UmbrellaDirectionChange(umbDir);
    }

    //0 - 앞, 1 - 뒤, 2 - 위, 3 - 좌, 4 - 우
    private void UmbrellaDirectionChange(int dir)
    {
        switch (dir)
        {
            case 0:
                transform.up = holdingCharacter.transform.forward;
                break;
            case 1:
                transform.up = -holdingCharacter.transform.forward;
                break;
            case 2:
                transform.up = holdingCharacter.transform.up;
                break;
            case 3:
                transform.up = -holdingCharacter.transform.right;
                break;
            case 4:
                transform.up = holdingCharacter.transform.right;
                break;

        }
    }

}
