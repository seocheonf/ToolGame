using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Playable : Character, ICameraTarget
{
    private FixedJoint joint = null;   //�ɱ� ��ɶ����� �߰�
    //9ũ��
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
    private bool isRush; //���� ���� �� ��� ������ �����̻��� �ο��ϱ� ���� �߰��� + ��Ÿ�� üũ�뵵

    //ĳ���� �ü�
    private float xRot;
    private float yRot;
    [SerializeField] float sensitivity;
    [SerializeField] float clampAngle;

    protected override void MyStart()
    {
        base.MyStart();
        rb = GetComponent<Rigidbody>();
        characterCollider = GetComponent<CapsuleCollider>();

        FuncInteractionData jump = new();
        jump.keyCode = KeyCode.Space;
        jump.description = "����";
        jump.OnFuncInteraction = OnJump;
        ControllerManager.AddInputFuncInteraction(jump);

        FuncInteractionData forward = new();
        forward.keyCode = KeyCode.W;
        forward.description = "������ �̵�";
        forward.OnFuncInteraction = OnMoveForward;
        ControllerManager.AddInputFuncInteraction(forward);

        FuncInteractionData backward = new();
        backward.keyCode = KeyCode.S;
        backward.description = "�ڷ� �̵�";
        backward.DurationFuncInteraction = OnMoveBackward;
        ControllerManager.AddInputFuncInteraction(backward);

        FuncInteractionData left = new();
        left.keyCode = KeyCode.A;
        left.description = "�������� �̵�";
        left.DurationFuncInteraction = OnMoveLeft;
        ControllerManager.AddInputFuncInteraction(left);

        FuncInteractionData right = new();
        right.keyCode = KeyCode.D;
        right.description = "���������� �̵�";
        right.DurationFuncInteraction = OnMoveRight;
        ControllerManager.AddInputFuncInteraction(right);

        FuncInteractionData accel = new();
        accel.keyCode = KeyCode.LeftShift;
        accel.description = "��� ���";
        accel.DurationFuncInteraction = OnRun;
        accel.OffFuncInteraction = RunGetKeyUp;
        ControllerManager.AddInputFuncInteraction(accel);

        FuncInteractionData sit = new();
        sit.keyCode = KeyCode.LeftControl;
        sit.description = "�ɱ� ���";
        accel.DurationFuncInteraction = OnSit;
        accel.OffFuncInteraction = UnSit;
        ControllerManager.AddInputFuncInteraction(sit);

        FuncInteractionData rush = new();
        rush.keyCode = KeyCode.R;
        rush.description = "���� ���";
        rush.OnFuncInteraction = OnRush;
        ControllerManager.AddInputFuncInteraction(rush);
    }

    protected void PlayableManagerUpdate(float deltaTime)
    {
        RunUpdate();
        RushCoolTimeUpdate(deltaTime);
        RenewalCrowdControlRemainTimeUpdate(deltaTime);
    }

    protected void PlayableManagerFixedUpdate(float fixedDeltaTime)
    {
        MoveHorizontalityFixedUpdate(fixedDeltaTime);
        ResetDirection();

    }

    protected override void Initialize()
    {
        base.Initialize();
        GameManager.ObjectsUpdate -= PlayableManagerUpdate;
        GameManager.ObjectsFixedUpdate -= PlayableManagerFixedUpdate;

        GameManager.ObjectsUpdate += PlayableManagerUpdate;
        GameManager.ObjectsFixedUpdate += PlayableManagerFixedUpdate;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        GameManager.ObjectsUpdate -= PlayableManagerUpdate;
        GameManager.ObjectsFixedUpdate -= PlayableManagerFixedUpdate;
    }

    protected override void Jump()
    {
        isJump = true;
    }

    private void OnSit()
    {
        Sit();
    }

    private void Sit()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 2, UnityEngine.Color.magenta);
        if (Physics.Raycast(rb.transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.collider.GetComponent<Rigidbody>())
            {
                if (joint == null) joint = gameObject.AddComponent<FixedJoint>();
                rb.constraints = RigidbodyConstraints.None;
                joint.connectedBody = hit.collider.GetComponent<Rigidbody>();
            }
        }
    }

    private void UnSit()
    {
        Destroy(joint);
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
            rb.AddForce(transform.forward * rushPower, ForceMode.Impulse);
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

    }

    private void CharacterRotationSightFixedUpdate()
    {
        xRot += ControllerManager.MouseMovement.y * sensitivity;
        yRot += ControllerManager.MouseMovement.x * sensitivity;

        xRot = Mathf.Clamp(xRot, -clampAngle, clampAngle);

        transform.eulerAngles = new Vector3(0, yRot, 0);
    }
    public FirstViewCameraData FirstViewCameraSet()
    {
        FirstViewCameraData tempt = new FirstViewCameraData();
        tempt.SetInfo(transform.position, transform.forward);
        return tempt;
    }

    public ThirdViewCameraData ThirdViewCameraSet()
    {
        ThirdViewCameraData tempt = new ThirdViewCameraData();
        tempt.SetInfo(transform.position, -xRot, yRot, 1, 5, 1, 2.5f);
        return tempt;
    }
}
