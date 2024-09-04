using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rascal : Character
{
    [SerializeField] Vector2 center;
    [SerializeField] Collider playerDetectArea;
    [SerializeField] protected GameObject player;
    [SerializeField] float radius;
    [SerializeField] float maxCoolTime;

    protected bool isPlayerIn;
    protected bool isAction = false;
    protected Vector3 destination;
    float coolTime;
    //���� ��ǥ�� Random.insideUnitCircle �� �ϸ� �ȴٰ� ��
    public void GetRandomPos()
    {
        var circlePos = radius * Random.insideUnitCircle;
        var movePos = new Vector3(circlePos.x, 0, circlePos.y) + new Vector3(center.x, 0, center.y);
        destination = movePos;
    }

    protected override void MyStart()
    {
        base.MyStart();
        characterCollider = GetComponent<CapsuleCollider>();
        currentGeneralState = GeneralState.Normal;
        coolTime = maxCoolTime;
        currentRigidbody = gameObject.GetComponent<Rigidbody>();
        GetRandomPos();
    }

    protected override void MoveHorizontalityFixedUpdate(float fixedDeltaTime)
    {
        if(!isAction) transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.fixedDeltaTime);
    }
    protected void RascalManagerUpdate(float deltaTime)
    {
        ApplicationGeneralState();
        RenewalCrowdControlRemainTimeUpdate(deltaTime);
    }

    protected void RascalManagerFixedUpdate(float fixedDeltaTime)
    {
        MoveHorizontalityFixedUpdate(fixedDeltaTime);
    }

    protected override void Initialize()
    {
        base.Initialize();

        GameManager.ObjectsUpdate -= RascalManagerUpdate;
        GameManager.ObjectsFixedUpdate -= RascalManagerFixedUpdate;
        GameManager.ObjectsUpdate += RascalManagerUpdate;
        GameManager.ObjectsFixedUpdate += RascalManagerFixedUpdate;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        GameManager.ObjectsUpdate -= RascalManagerUpdate;
        GameManager.ObjectsFixedUpdate -= RascalManagerFixedUpdate;
    }

    protected override void ApplicationGeneralState()
    {
        switch (currentGeneralState)
        {
            case GeneralState.Normal:
                currentRigidbody.rotation = Quaternion.identity;
                currentRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                if ((destination - new Vector3(transform.position.x, 0, transform.position.z)).magnitude <= 0.25f) GetRandomPos();
                if (isPlayerIn) MoveForPlayer();
                break;
            case GeneralState.CrowdControl:
                ApplicationCrowdControl();
                break;
            case GeneralState.Action:
                break;
        }
    }

    void CalculateCoolTime(float deltaTime)
    {
        coolTime -= deltaTime;
        if (coolTime <= 0)
        {
            currentGeneralState = GeneralState.Normal;
            GetRandomPos();
            coolTime = maxCoolTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Playable>())
        {
            isPlayerIn = true;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Playable>())
        {
            isPlayerIn = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        isPlayerIn = false;
        moveSpeed = defaultMoveSpeed;
        currentGeneralState = GeneralState.Normal;
        GetRandomPos();
    }

    void MoveForPlayer()
    {
        if (isPlayerIn)
        {
            moveSpeed = defaultMoveSpeed + defaultAccelSpeed;
            destination = player.transform.position;
        }
    }

}
