using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Timeline;

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
    /// �Ϲ�ȭ�� ���� �ִ� �Լ�. ��� ������� ������ ������ �����ȴ�.
    /// </summary>
    /// <param name="target">���� ���� ���</param>
    public void ApplyForce(PhysicsInteractionObject target)
    {

    }
}

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public abstract class PhysicsInteractionObject : MyComponent
{
    //�� ������Ʈ�� �����ۿ� ���� ������ٵ�
    protected Rigidbody initialRigidbody;
    //�� ������Ʈ�� �����ۿ��� �� ��, ��ġ �ڽ��� �� ó�� Ȱ���ؾ� �� ������ٵ�. �� ���������� �������� ������ �ǹ���.
    protected Rigidbody currentRigidbody;
    public Rigidbody CurrentRigidbody => currentRigidbody;
    protected Collider physicsInteractionObjectCollider;

    protected Queue<ForceInfo> receivedForceQueue;

    protected float initialMass;
    public float InitialMass => initialMass;
    protected float currentMass;
    public float CurrentMass => currentMass;

    const float massRatio = 0.05f;

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

    /// <summary>
    /// Main�� �Ǵ� FixedUpdate. �⺻������ ������� ���� �����ϴ� ����� �����Ǿ� �ִ�.
    /// </summary>
    /// <param name="fixedDeltaTime">1fixed frame�� �ð�</param>
    protected virtual void MainFixedUpdate(float fixedDeltaTime)
    {
        ApplyForceQueue();
    }

    public virtual void GetSpecialInteraction(SpecialInteraction.WindData source)
    {
        if(source.origin == this)
        {
            return;
        }

        AddForce(new ForceInfo(source.Direction * source.intensity, ForceType.DurationForce));
    }
    public virtual void GetSpecialInteraction(SpecialInteraction.WaterData source)
    {
        //����
        AddForce(new ForceInfo(source.Direction * source.intensity, ForceType.DurationForce));
        //�η�
        AddForce(new ForceInfo(Vector3.up * source.amount, ForceType.UnityDuration));
        //AddForce(new ForceInfo(Vector3.up * (source.amount / CurrentMass) * (GetDownSpeed() * -1 * 0.5f), ForceType.DurationForce));
    }
    public virtual void GetSpecialInteraction(SpecialInteraction.FireData source)
    {
        source.detectedCharacter.SetCrowdControl(CrowdControlState.Stun, 0.5f);
        source.detectedCharacter.AddForce((-source.detectedCharacter.transform.forward * source.backBounce + Vector3.up * source.upBounce), ForceType.VelocityForce);
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
            case ForceType.UnityDuration:
                AddForceUnityDuration(direction);
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
        currentRigidbody.velocity += direction * 2f;
    }
    private void AddForceDuration(Vector3 direction)
    {
        currentRigidbody.velocity += direction * 0.2f; 
    }
    private void AddForceUnityDuration(Vector3 direction)
    {
        currentRigidbody.AddForce(direction, ForceMode.Acceleration);
    }

    protected virtual void Initialize()
    {
        receivedForceQueue = new Queue<ForceInfo>();
        currentRigidbody = initialRigidbody;
        initialMass = initialRigidbody.mass;
        currentMass = currentRigidbody.mass;

        RegisterFuncInInitialize();
    }

    /// <summary>
    /// ��� ��� �۾��� �����մϴ�.
    /// </summary>
    protected abstract void RegisterFuncInInitialize();

    /// <summary>
    /// ��� ���� �۾��� �����մϴ�.
    /// </summary>
    protected abstract void RemoveFuncInDestroy();

    protected override void MyDestroy()
    {
        base.MyDestroy();

        RemoveFuncInDestroy();
    }

    public virtual Vector3 GetVelocity()
    {
        return currentRigidbody.velocity;
    }
}
