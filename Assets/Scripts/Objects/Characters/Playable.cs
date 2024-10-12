using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;


public class Playable : Character, ICameraTarget
{
    private FixedJoint joint = null;   //앉기 기능때문에 추가
    [SerializeField] float sitRaycastDistance;

    //9크기
    private UniqueTool[] currentStoreUniqueTool;

    private UniqueTool currentTargetUniqueTool;
    private IOuterFuncInteraction currentTargetOuterFuncInteraction;

    private List<FuncInteractionData> originInputFuncInteractionList;
    /// <summary>
    /// 현재 들고 있는 도구의 기능들 모음
    /// </summary>
    private List<FuncInteractionData> currentHoldingFuncInteractionList;
    private List<FuncInteractionData> currentOuterFuncInteractionList;
    /// <summary>
    /// 플레이어블의 행동과 연관이 있는 기능들 모음
    /// </summary>
    private List<FuncInteractionData> playableActionFuncInteractionList;
    /// <summary>
    /// 플레이어블과 연관되지만, 마치 매니저처럼 어느 상황에서든 되었으면 하는 것
    /// </summary>
    private List<FuncInteractionData> playableAbsoluteFuncInteractionList;

    private bool isSit;
    private bool isPickUpTool = false;
    private int currentSightOrdinal;

    [SerializeField] float defaultRushPower;
    private float rushPower;

    [SerializeField] float defaultRushCoolTime;
    private float rushCoolTime;
    private bool isRush; //러쉬 상태 일 경우 적에게 상태이상을 부여하기 위해 추가함 + 쿨타임 체크용도

    //캐릭터 시선
    protected float xRot;
    protected float yRot;
    [SerializeField] float sensitivity;
    [SerializeField] float clampAngle;
    [SerializeField] float sightForwardLength;

    //캐릭터 인칭
    private CameraViewType playableView;
    private CameraViewType PlayableView
    {
        get
        {
            return playableView;
        }
        set
        {
            GameManager.Instance.CurrentWorld.WorldCamera.CameraBreak(this);
            playableView = value;
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, playableView);
        }
    }

    //플레이어블 입력 UI
    private PlayableInputUI inputUI;


    protected override void Initialize()
    {
        base.Initialize();

        rushPower = defaultRushPower;
        playableView = CameraViewType.ThirdView;

        GameManager.ObjectsUpdate -= PlayableManagerUpdate;
        GameManager.CharactersFixedUpdate -= PlayableManagerFixedUpdate;

        GameManager.ObjectsUpdate += PlayableManagerUpdate;
        GameManager.CharactersFixedUpdate += PlayableManagerFixedUpdate;

        //깔쌈한테스트//
        currentHoldingFuncInteractionList = new List<FuncInteractionData>();
        playableActionFuncInteractionList = new List<FuncInteractionData>();
        playableAbsoluteFuncInteractionList = new List<FuncInteractionData>();

        inputUI = GameManager.Instance.UI.GetFixedUI<PlayableInputUI>(FixedUIType.PlayableInputUI);
    }
    protected override void MyStart()
    {
        base.MyStart();
        
        //==Initialize() 끝난 후==//

        characterCollider = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<Animator>();

        //깔쌈한테스트//

        FuncInteractionData jump = new(OuterKeyCode.Jump, "점프", OnJump, null, null);
        //ControllerManager.AddInputFuncInteraction(jump);

        FuncInteractionData forward = new(OuterKeyCode.Forward, "앞으로 이동", null, OnMoveForward, null);
        //ControllerManager.AddInputFuncInteraction(forward);

        FuncInteractionData backward = new(OuterKeyCode.Backward, "뒤로 이동", null, OnMoveBackward, null);
        //ControllerManager.AddInputFuncInteraction(backward);

        FuncInteractionData left = new(OuterKeyCode.Leftward, "왼쪽으로 이동", null, OnMoveLeft, null);
        //ControllerManager.AddInputFuncInteraction(left);

        FuncInteractionData right = new(OuterKeyCode.Rightward, "오른쪽으로 이동", null, OnMoveRight, null);
        //ControllerManager.AddInputFuncInteraction(right);

        FuncInteractionData accel = new(OuterKeyCode.Dash, "대시 기능", null, OnRun, RunGetKeyUp);
        //ControllerManager.AddInputFuncInteraction(accel);

        FuncInteractionData sit = new(OuterKeyCode.Sit, "앉기 기능", null, OnSit, UnSit);
        //ControllerManager.AddInputFuncInteraction(sit);

        FuncInteractionData rush = new(OuterKeyCode.Rush, "돌진 기능", OnRush, null, null);
        //ControllerManager.AddInputFuncInteraction(rush);

        FuncInteractionData onOffPickupTool = new(OuterKeyCode.TakeTool, "도구 잡고 놓는 기능", OnOffPickUpTool, null, null);
        //ControllerManager.AddInputFuncInteraction(onOffPickupTool);

        FuncInteractionData clickSwitch = new(OuterKeyCode.OuterFunc, "레버 또는 버튼 누르는 기능", OnSwitchFuncInteraction, null, null);
        //ControllerManager.AddInputFuncInteraction(clickSwitch);

        AddPlayableActionInputFuncInteraction(jump);
        AddPlayableActionInputFuncInteraction(forward);
        AddPlayableActionInputFuncInteraction(backward);
        AddPlayableActionInputFuncInteraction(left);
        AddPlayableActionInputFuncInteraction(right);
        AddPlayableActionInputFuncInteraction(accel);
        AddPlayableActionInputFuncInteraction(sit);
        AddPlayableActionInputFuncInteraction(rush);
        AddPlayableActionInputFuncInteraction(onOffPickupTool);
        AddPlayableActionInputFuncInteraction(clickSwitch);

        AddInputFuncInteraction(playableActionFuncInteractionList);

        // 절대적 입력

        FuncInteractionData sightChange = new(OuterKeyCode.Sight, "1, 3인칭을 교체하는 기능", OnSightChange, null, null);
        AddInputFuncInteraction(sightChange);

        // 기능적 초기화 (Initialize는 다소 변수적 초기화)
        PlayableViewInitialize();
    }


    private void PlayableViewInitialize()
    {
        GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraViewType.ThirdView);
        PlayableView = CameraViewType.ThirdView;
    }


    protected void PlayableManagerUpdate(float deltaTime)
    {
        ApplicationGeneralState();
        RunUpdate();
        AnimatorUpdate();
        RushCoolTimeUpdate(deltaTime);
        RenewalCrowdControlRemainTimeUpdate(deltaTime);
    }

    protected void PlayableManagerFixedUpdate(float fixedDeltaTime)
    {
        FixedUpdate_Test();
        MoveHorizontalityFixedUpdate(fixedDeltaTime);
        ResetDirection();
        CharacterRotationSightFixedUpdate();
        if(currentGeneralState == GeneralState.Normal && !isSit && !isRush) MoveLookAt();
    }


    protected override void MyDestroy()
    {
        base.MyDestroy();
        GameManager.ObjectsUpdate -= PlayableManagerUpdate;
        GameManager.CharactersFixedUpdate -= PlayableManagerFixedUpdate;

        RemoveInputFuncInteraction(playableActionFuncInteractionList);

        RemoveInputFuncInteraction(playableAbsoluteFuncInteractionList);
    }

    protected override void Jump()
    {
        base.Jump();
        anim.SetTrigger("isJump");
    }
    private void OnSit()
    {
        if (currentGeneralState == GeneralState.Normal) Sit();
        else UnSit();
    }

    private void Sit()
    {
        RaycastHit hit;
        if (Physics.Raycast(currentRigidbody.transform.position, Vector3.down, out hit, sitRaycastDistance))
        {
            if (hit.collider.GetComponent<Rigidbody>())
            {
                if (joint == null) joint = gameObject.AddComponent<FixedJoint>();
                currentRigidbody.constraints = RigidbodyConstraints.None;
                joint.connectedBody = hit.collider.GetComponent<Rigidbody>();
                isSit = true;
            }
        }
    }

    private void UnSit()
    {
        Destroy(joint);
        currentRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        isSit = false;
    }


    private void OnRush()
    {
        Rush();
    }

    private void Rush()
    {
        if (IsGround && !isRush)
        {
            isRush = true;
            rushCoolTime = defaultRushCoolTime;
            currentRigidbody.AddForce(transform.forward * rushPower, ForceMode.Impulse);
        }
    }

    private void RushCoolTimeUpdate(float deltaTime)
    {
        if (isRush)
        {
            rushCoolTime -= deltaTime;
            if (rushCoolTime <= 0)
            {
                isRush = false;
            }
        }
    }

    private void ChangeSightOrdinal()
    {

    }

    private void TargetGameObjectUpdate()
    {

    }

    private void TargetUniqueTool()
    {
        RaycastHit hit;
        int layerMask = (1 << LayerMask.NameToLayer("Cast_UniqueTool")) + (1 << LayerMask.NameToLayer("Block"));
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), CurrentSightForward_Interaction, out hit, sightForwardLength, layerMask))
        {
            currentTargetUniqueTool = hit.collider.GetComponentInParent<UniqueTool>();
            PickUpTool(currentTargetUniqueTool);
            isPickUpTool = true;
        }
    }

    private void OnOffPickUpTool()
    {
        if (isPickUpTool)
        {
            PutTool();
            isPickUpTool = false;
        }
        else
        {
            TargetUniqueTool();
        }
    }


    private void TargetOuterFuncInteraction()
    {

    }

    private void DoTargetingUniqueTool()
    {

    }
    private void DoUnTargetingUniqueTool()
    {

    }

    private void DoTargetingOuterFuncInteraction()
    {

    }
    private void DoUnTargetingOuterFuncInteraction()
    {

    }

    private void OnSwitchFuncInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), CurrentSightForward_Interaction, out hit, sightForwardLength))
        {
            if (hit.collider.GetComponent<ITriggerFuncInteraction>() != null)
            {
                hit.collider.GetComponent<ITriggerFuncInteraction>().DoTrigger();
            }
        }
    }

    /// <summary>
    /// 1, 3인칭을 전환하는 함수
    /// </summary>
    private void OnSightChange()
    {
        if (PlayableView == CameraViewType.Length - 1)
        {
            PlayableView = CameraViewType.FirstView;
        }
        else if (PlayableView >= CameraViewType.Default)
        {
            PlayableView += 1;
        }
        else if (PlayableView >= CameraViewType.Length)
        {
            Debug.LogError("허용되지 않은 플레이어블의 카메라 타입입니다!");
            PlayableView = CameraViewType.FirstView;
        }
        else
        {
            Debug.LogError("규격외의 값입니다!");
            PlayableView = CameraViewType.FirstView;
        }
    }

   

    protected override void ApplicationGeneralState()
    {
        switch (currentGeneralState)
        {
            case GeneralState.Normal:
                if (isSit || isRush)
                {
                    currentGeneralState = GeneralState.Action;
                    break;
                }
                CheckWantMoveDirection();
                currentRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                break;
            case GeneralState.CrowdControl:
                ApplicationCrowdControl();
                break;
            case GeneralState.Action:
                if (!isSit && !isRush) currentGeneralState = GeneralState.Normal;
                break;
        }
    }

    private void AnimatorUpdate()
    {
        anim.SetBool("isGround", IsGround);
        anim.SetBool("isRush", isRush);
        anim.SetBool("isRun", isAccel);
        anim.SetBool("isSit", isSit);

        anim.SetFloat("MoveSpeed", currentSpeed);

        anim.SetInteger("GeneralState", (int)currentGeneralState);
        anim.SetInteger("CrowdState", (int)currentCrowdControlState);
    }

    private void MoveLookAt()
    {
        transform.eulerAngles = new Vector3(0, yRot, 0);
    }

    private void CharacterRotationSightFixedUpdate()
    {
        xRot += ControllerManager.MouseMovement.y * sensitivity;
        yRot += ControllerManager.MouseMovement.x * sensitivity;

        xRot = Mathf.Clamp(xRot, -clampAngle, clampAngle);

        //transform.eulerAngles = new Vector3(0, yRot, 0);
    }
    public FirstViewCameraData FirstViewCameraSet()
    {
        FirstViewCameraData tempt = new FirstViewCameraData();
        tempt.SetInfo(transform.position, Quaternion.Euler(-xRot, yRot, 0) * Vector3.forward);
        return tempt;
    }

    public ThirdViewCameraData ThirdViewCameraSet()
    {
        ThirdViewCameraData tempt = new ThirdViewCameraData();
        tempt.SetInfo(transform.position, -xRot, yRot, 1, 5, 1, 2.5f);
        return tempt;
    }

    public void CustomViewCameraSet(Camera main, float deltaTime)
    {

    }

    public override Vector3 CurrentSightEulerAngle_Origin
    {
        get
        {
            Vector3 result = Vector3.zero;
            result.x = -xRot;
            result.y = yRot;

            return result;
        }
    }


    //깔쌈한테스트//

    private void AddInputFuncInteraction(FuncInteractionData data)
    {
        ControllerManager.AddInputFuncInteraction(data);
    }
    private void AddInputFuncInteraction(List<FuncInteractionData> list)
    {
        ControllerManager.AddInputFuncInteraction(list);
    }
    private void RemoveInputFuncInteraction(FuncInteractionData data)
    {
        ControllerManager.RemoveInputFuncInteraction(data);
    }
    private void RemoveInputFuncInteraction(List<FuncInteractionData> list)
    {
        ControllerManager.RemoveInputFuncInteraction(list);
    }


    private void AddPlayableActionInputFuncInteraction(FuncInteractionData data)
    {
        playableActionFuncInteractionList.Add(data);
    }
    private void AddPlayableActionInputFuncInteraction(List<FuncInteractionData> list)
    {
        foreach(FuncInteractionData each in list)
        {
            playableActionFuncInteractionList.Add(each);
        }
    }
    private void RemovePlayableActionInputFuncInteraction(FuncInteractionData data)
    {
        playableActionFuncInteractionList.Remove(data);
    }
    private void RemovePlayableActionInputFuncInteraction(List<FuncInteractionData> list)
    {
        foreach (FuncInteractionData each in list)
        {
            playableActionFuncInteractionList.Remove(each);
        }
    }


    private void AddHoldingInputFuncInteraction(FuncInteractionData data)
    {
        currentHoldingFuncInteractionList.Add(data);
        inputUI.SetInputInfo(data);
    }
    private void AddHoldingInputFuncInteraction(List<FuncInteractionData> list)
    {
        foreach (FuncInteractionData each in list)
        {
            currentHoldingFuncInteractionList.Add(each);
            inputUI.SetInputInfo(each);
        }
    }
    private void RemoveHoldingInputFuncInteraction(FuncInteractionData data)
    {
        inputUI.UnSetInputInfo(data);
        currentHoldingFuncInteractionList.Remove(data);
    }
    private void RemoveHoldingInputFuncInteraction(List<FuncInteractionData> list)
    {
        foreach (FuncInteractionData each in list)
        {
            inputUI.UnSetInputInfo(each);
            currentHoldingFuncInteractionList.Remove(each);
        }
    }
    private void RemoveAllHoldingInputFuncInteraction()
    {
        inputUI.UnSetInputInfo(currentHoldingFuncInteractionList);
        currentHoldingFuncInteractionList.Clear();
    }

    //깔쌈한테스트//
    public void ExchangeHoldingInputFuncInteraction(List<FuncInteractionData> newList)
    {
        RemoveInputFuncInteraction(currentHoldingFuncInteractionList);
        RemovePlayableActionInputFuncInteraction(currentHoldingFuncInteractionList);
        RemoveAllHoldingInputFuncInteraction();

        AddHoldingInputFuncInteraction(newList);
        AddPlayableActionInputFuncInteraction(currentHoldingFuncInteractionList);
        AddInputFuncInteraction(currentHoldingFuncInteractionList);
    }


    protected override void AfterCompleteSetCrowdControl(CrowdControlState targetCC)
    {
        base.AfterCompleteSetCrowdControl(targetCC);
        if(targetCC == CrowdControlState.ElectricShcok || targetCC == CrowdControlState.Stun)
        {
            RemoveInputFuncInteraction(playableActionFuncInteractionList);
        }
    }

    protected override void AfterCompleteDeleteCrowdControlDict(CrowdControlState beforeCrowdControlState)
    {
        base.AfterCompleteDeleteCrowdControlDict(beforeCrowdControlState);

        if (beforeCrowdControlState == CrowdControlState.ElectricShcok || beforeCrowdControlState == CrowdControlState.Stun)
        {
            AddInputFuncInteraction(playableActionFuncInteractionList);
        }
    }




    protected override void PutTool()
    {
        //깔쌈한테스트//
        RemoveInputFuncInteraction(currentHoldingFuncInteractionList);
        RemovePlayableActionInputFuncInteraction(currentHoldingFuncInteractionList);
        RemoveAllHoldingInputFuncInteraction();

        base.PutTool();
    }

    public override void PickUpTool(UniqueTool target)
    {
        base.PickUpTool(target);

        //깔쌈한테스트//
        AddHoldingInputFuncInteraction(target.GetHoldingFuncInteractionDataList());
        AddPlayableActionInputFuncInteraction(currentHoldingFuncInteractionList);
        AddInputFuncInteraction(currentHoldingFuncInteractionList);
    }
}
