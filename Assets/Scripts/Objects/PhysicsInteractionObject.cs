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

//[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class PhysicsInteractionObject : MyComponent
{

    protected Rigidbody physicsInteractionObjectRigidbody;
    protected Collider physicsInteractionObjectCollider;

    protected Queue<ForceInfo> receivedForceQueue;

    private void Awake()
    {
        physicsInteractionObjectRigidbody = GetComponent<Rigidbody>();
        physicsInteractionObjectCollider = GetComponent<Collider>();
    }

    protected override void MyStart()
    {
        base.MyStart();
        Initialize();
    }

    public virtual void GetSpecialInteraction(SpecialInteraction.WindData source)
    {
        receivedForceQueue.Enqueue(new ForceInfo(source.Direction * source.intensity, ForceType.VelocityForce));
    }
    public virtual void GetSpecialInteraction(SpecialInteraction.WaterData source)
    {

    }
    public virtual void GetSpecialInteraction(SpecialInteraction.FireData source)
    {

    }

    public virtual void AccelDownForce(float ratio)
    {
        Vector3 down = physicsInteractionObjectRigidbody.velocity;
        down.y *= ratio;
        physicsInteractionObjectRigidbody.velocity = down;
    }
    public float GetDownSpeed()
    {
        return physicsInteractionObjectRigidbody.velocity.y;
    }

    public virtual void AddForce(ForceInfo info)
    {
        AddForce(info.direction, info.forceType);
    }
    public virtual void AddForce(Vector3 direction, ForceType forceType)
    {
        switch(forceType)
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
    private void AddForceVelocity(Vector3 direction)
    {
        physicsInteractionObjectRigidbody.velocity += direction;
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
    }



    public virtual Vector3 GetVelocity()
    {
        return physicsInteractionObjectRigidbody.velocity;
    }
}
