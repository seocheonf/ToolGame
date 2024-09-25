using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rascal : Character
{
    [SerializeField] Vector2 center;
    [SerializeField] Collider playerDetectArea;
    [SerializeField] float radius;
    [SerializeField] float maxCoolTime;

    protected bool isPlayerIn;
    protected bool isAction = false;
    protected Vector3 destination;
    float coolTime;
    //랜덤 좌표는 Random.insideUnitCircle 로 하면 된다고 함
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
        anim = GetComponent<Animator>();
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
        UpdateLookTarget();
        AnimatorUpdate();
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Playable>())
        {
            isPlayerIn = true;
            MoveForPlayer(other.GetComponent<Playable>());
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Playable>())
        {
            isPlayerIn = true;
            MoveForPlayer(other.GetComponent<Playable>());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        isPlayerIn = false;
        moveSpeed = defaultMoveSpeed;
        currentGeneralState = GeneralState.Normal;
        GetRandomPos();
    }

    void MoveForPlayer(Playable player)
    {
        if (isPlayerIn)
        {
            moveSpeed = defaultMoveSpeed + defaultAccelSpeed;
            destination = player.transform.position;
            
        }
    }

    void UpdateLookTarget()
    {
        Vector3 dir = destination - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.02f);
    }

    protected virtual void AnimatorUpdate()
    {
        anim.SetBool("isPlayerIn", isPlayerIn);
    }
}
