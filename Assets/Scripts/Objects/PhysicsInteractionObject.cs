using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ForceInfo
{
    public Vector3 direction;
    public ForceType forceType;

    public ForceInfo(Vector3 direction, ForceType forceType)
    {
        this.direction = direction;
        this.forceType = forceType;
    }

    /// <summary>
    /// 일반화된 힘을 주는 함수. 대상에 관계없이 일정한 로직이 제공된다.
    /// </summary>
    /// <param name="target">힘을 받을 대상</param>
    public void ApplyForce(PhysicsInteractionObject target)
    {

    }
}

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class PhysicsInteractionObject : MyComponent
{
    //본 오브젝트의 물리작용 원본 리지드바디
    protected Rigidbody initialRigidbody;
    //본 오브젝트가 물리작용을 할 때, 마치 자신의 것 처럼 활용해야 할 리지드바디. 즉 물리적으로 실질적인 본인을 의미함.
    protected Rigidbody currentRigidbody;
    public Rigidbody CurrentRigidbody => currentRigidbody;
    protected Collider physicsInteractionObjectCollider;

    protected Queue<ForceInfo> receivedForceQueue;

    protected float initialMass;
    public float InitialMass => initialMass;
    protected float currentMass;
    public float CurrentMass => currentMass;

    const float massRatio = 0.01f;

    private void Awake()
    {
        initialRigidbody = GetComponent<Rigidbody>();
        physicsInteractionObjectCollider = GetComponent<Collider>();
    }

    protected override void MyStart()
    {
        base.MyStart();
        Initialize();
    }

    protected virtual void MainFixedUpdate(float fixedDeltaTime)
    {
        ApplyForceQueue();
    }

    public virtual void GetSpecialInteraction(SpecialInteraction.WindData source)
    {
        AddForce(new ForceInfo(source.Direction * source.intensity, ForceType.VelocityForce));
    }
    public virtual void GetSpecialInteraction(SpecialInteraction.WaterData source)
    {

    }
    public virtual void GetSpecialInteraction(SpecialInteraction.FireData source)
    {

    }

    public virtual void AccelDownForce(float ratio)
    {
        Vector3 down = currentRigidbody.velocity;
        down.y *= ratio;
        currentRigidbody.velocity = down;
    }
    public virtual float GetDownSpeed()
    {
        return currentRigidbody.velocity.y;
    }

    public void AddForce(Vector3 direction, ForceType forceType)
    {
        AddForce(new ForceInfo(direction, forceType));
    }
    public void AddForce(ForceInfo info)
    {
        receivedForceQueue.Enqueue(info);
    }

    private void ApplyForce(ForceInfo info)
    {
        ApplyForce(info.direction, info.forceType);
    }
    private void ApplyForce(Vector3 direction, ForceType forceType)
    {
        direction /= (1 + currentRigidbody.mass * massRatio);
        switch (forceType)
        {
            case ForceType.VelocityForce:
                AddForceVelocity(direction);
                break;
            case ForceType.ImpulseForce:
                AddForceImpulse(direction);
                break;
            case ForceType.DurationForce:
                AddForceDuration(direction);
                break;
        }
    }
    protected void ApplyForceQueue()
    {
        while(receivedForceQueue.TryDequeue(out ForceInfo result))
        {
            ApplyForce(result.direction, result.forceType);
        }
    }

    private void AddForceVelocity(Vector3 direction)
    {
        currentRigidbody.velocity += direction;
    }
    private void AddForceImpulse(Vector3 direction)
    {

    }
    private void AddForceDuration(Vector3 direction)
    {

    }

    protected virtual void Initialize()
    {
        receivedForceQueue = new Queue<ForceInfo>();
        currentRigidbody = initialRigidbody;
        initialMass = initialRigidbody.mass;
        currentMass = currentRigidbody.mass;

        //기능 등록 작업
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
        GameManager.ObjectsFixedUpdate += MainFixedUpdate;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //기능 해제 작업
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
    }

    public virtual Vector3 GetVelocity()
    {
        return currentRigidbody.velocity;
    }
}
