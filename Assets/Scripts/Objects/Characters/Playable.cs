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
    protected float xRot;
    protected float yRot;
    [SerializeField] float sensitivity;
    [SerializeField] float clampAngle;

    protected override void MyStart()
    {
        base.MyStart();
        characterCollider = GetComponent<CapsuleCollider>();

        FuncInteractionData jump = new(KeyCode.Space, "����", OnJump, null, null);
        ControllerManager.AddInputFuncInteraction(jump);

        FuncInteractionData forward = new(KeyCode.W, "������ �̵�", null, OnMoveForward, null);
        ControllerManager.AddInputFuncInteraction(forward);

        FuncInteractionData backward = new(KeyCode.S, "�ڷ� �̵�", null, OnMoveBackward, null);
        ControllerManager.AddInputFuncInteraction(backward);

        FuncInteractionData left = new(KeyCode.A, "�������� �̵�", null, OnMoveLeft, null);
        ControllerManager.AddInputFuncInteraction(left);

        FuncInteractionData right = new(KeyCode.D, "���������� �̵�", null, OnMoveRight, null);
        ControllerManager.AddInputFuncInteraction(right);

        FuncInteractionData accel = new(KeyCode.LeftShift, "��� ���", null, OnRun, RunGetKeyUp);
        ControllerManager.AddInputFuncInteraction(accel);

        FuncInteractionData sit = new(KeyCode.LeftControl, "�ɱ� ���", null, OnSit, UnSit);
        ControllerManager.AddInputFuncInteraction(sit);

        FuncInteractionData rush = new(KeyCode.R, "���� ���", OnRush, null, null);
        ControllerManager.AddInputFuncInteraction(rush);

        GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraType.ThirdView);

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
        CharacterRotationSightFixedUpdate();
    }

    protected override void Initialize()
    {
        base.Initialize();


        rushPower = defaultRushPower;




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

    private void OnSit()
    {
        Sit();
    }

    private void Sit()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 2, UnityEngine.Color.magenta);
        if (Physics.Raycast(currentRigidbody.transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.collider.GetComponent<Rigidbody>())
            {
                if (joint == null) joint = gameObject.AddComponent<FixedJoint>();
                currentRigidbody.constraints = RigidbodyConstraints.None;
                joint.connectedBody = hit.collider.GetComponent<Rigidbody>();
            }
        }
    }

    private void UnSit()
    {
        Destroy(joint);
        //�ӽÿ�
        currentRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }


    private void OnRush()
    {
        Rush();
    }

    private void Rush()
    {
        Debug.Log("rush");
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
        tempt.SetInfo(transform.position, Quaternion.Euler(-xRot, yRot, 0) * Vector3.forward);
        return tempt;
    }

    public ThirdViewCameraData ThirdViewCameraSet()
    {
        ThirdViewCameraData tempt = new ThirdViewCameraData();
        tempt.SetInfo(transform.position, -xRot, yRot, 1, 5, 1, 2.5f);
        return tempt;
    }


    
}
