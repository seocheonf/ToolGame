using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Playable : Character, ICameraTarget
{
    private FixedJoint joint = null;   //앉기 기능때문에 추가
    [SerializeField] float sitRaycastDistance;

    //9크기
    private UniqueTool[] currentStoreUniqueTool;

    private UniqueTool currentTargetUniqueTool;
    private IOuterFuncInteraction currentTargetOuterFuncInteraction;

    private List<FuncInteractionData> originInputFuncInteractionList;
    private List<FuncInteractionData> currentHoldingFuncInteractionList;
    private List<FuncInteractionData> currentOuterFuncInteractionList;

    private bool isSit;

    private int currentSightOrdinal;

    private bool isJump;

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

    protected override void MyStart()
    {
        base.MyStart();
        characterCollider = GetComponent<CapsuleCollider>();

        FuncInteractionData jump = new(KeyCode.Space, "점프", OnJump, null, null);
        ControllerManager.AddInputFuncInteraction(jump);

        FuncInteractionData forward = new(KeyCode.W, "앞으로 이동", null, OnMoveForward, null);
        ControllerManager.AddInputFuncInteraction(forward);

        FuncInteractionData backward = new(KeyCode.S, "뒤로 이동", null, OnMoveBackward, null);
        ControllerManager.AddInputFuncInteraction(backward);

        FuncInteractionData left = new(KeyCode.A, "왼쪽으로 이동", null, OnMoveLeft, null);
        ControllerManager.AddInputFuncInteraction(left);

        FuncInteractionData right = new(KeyCode.D, "오른쪽으로 이동", null, OnMoveRight, null);
        ControllerManager.AddInputFuncInteraction(right);

        FuncInteractionData accel = new(KeyCode.LeftShift, "대시 기능", null, OnRun, RunGetKeyUp);
        ControllerManager.AddInputFuncInteraction(accel);

        FuncInteractionData sit = new(KeyCode.LeftControl, "앉기 기능", null, OnSit, UnSit);
        ControllerManager.AddInputFuncInteraction(sit);

        FuncInteractionData rush = new(KeyCode.R, "돌진 기능", OnRush, null, null);
        ControllerManager.AddInputFuncInteraction(rush);

        FuncInteractionData pickupTools = new(KeyCode.Mouse0, "도구 집는 기능", TargetUniqueTool, null, null);
        ControllerManager.AddInputFuncInteraction(pickupTools);

        FuncInteractionData putTools = new(KeyCode.Mouse1, "도구 놓는 기능", PutTool, null, null);
        ControllerManager.AddInputFuncInteraction(putTools);

        FuncInteractionData clickSwitch = new(KeyCode.E, "레버 또는 버튼 누르는 기능", OnSwitchFuncInteraction, null, null);
        ControllerManager.AddInputFuncInteraction(clickSwitch);

        GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraViewType.ThirdView);
    }
    protected void PlayableManagerUpdate(float deltaTime)
    {
        ApplicationGeneralState();
        RunUpdate();
        RushCoolTimeUpdate(deltaTime);
        RenewalCrowdControlRemainTimeUpdate(deltaTime);
    }

    protected void PlayableManagerFixedUpdate(float fixedDeltaTime)
    {
        MoveHorizontalityFixedUpdate(fixedDeltaTime);
        ResetDirection();
        CharacterRotationSightFixedUpdate();
    }

    protected override void Initialize()
    {
        base.Initialize();

        rushPower = defaultRushPower;

        GameManager.ObjectsUpdate -= PlayableManagerUpdate;
        GameManager.CharactersFixedUpdate -= PlayableManagerFixedUpdate;

        GameManager.ObjectsUpdate += PlayableManagerUpdate;
        GameManager.CharactersFixedUpdate += PlayableManagerFixedUpdate;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        GameManager.ObjectsUpdate -= PlayableManagerUpdate;
        GameManager.CharactersFixedUpdate -= PlayableManagerFixedUpdate;
    }

    private void OnSit()
    {
        if (currentGeneralState == GeneralState.Normal) Sit();
        else UnSit();
    }

    private void Sit()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 2, UnityEngine.Color.magenta);
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
        //Boxcast 로 바꿔야 함
        //해봤자 땅에 닿아도 Raycast는 쏠 것이고 쏜 곳에 Boxcast를 만든 뒤에
        //Boxcast안에 들어있는 오브젝트가 hit.collider.GetComponent<UniqueTool>() 인 경우에만 PickupTool() 하면 되기때문에
        //그렇게 큰 어려움은 아닐듯 함 (아님 공룡박치기하고)
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), CurrentSightForward_Interaction, out hit, sightForwardLength))
        {
            if (hit.collider.GetComponent<UniqueTool>())
            {
                currentTargetUniqueTool = hit.collider.GetComponent<UniqueTool>();
            }
        }
    }
    private void TargetUniqueTool()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), CurrentSightForward_Interaction, out hit, sightForwardLength))
        {
            if (hit.collider.GetComponent<UniqueTool>())
            {
                currentTargetUniqueTool = hit.collider.GetComponent<UniqueTool>();
                PickUpTool(currentTargetUniqueTool);
            }
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
            if (hit.collider.GetComponent<LimitPositionObject>())
            {
                hit.collider.GetComponent<LimitPositionObject>().ObjectClick();
            }
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
                MoveLookAt();
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
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), CurrentSightForward_Interaction * sightForwardLength, UnityEngine.Color.red);

    }

#endif
}
