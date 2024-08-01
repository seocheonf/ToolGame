using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInfo
{
    public Vector3 direction;
    public ForceType forceType;

    /// <summary>
    /// �Ϲ�ȭ�� ���� �ִ� �Լ�. ��� ������� ������ ������ �����ȴ�.
    /// </summary>
    /// <param name="target">���� ���� ���</param>
    public void ApplyForce(PhysicsInteractionObject target)
    {

    }
}

public abstract class PhysicsInteractionObject : MyComponent
{
    private Queue<ForceInfo> receivedForceQueue;

    protected override void MyStart()
    {
        base.MyStart();
        Initialize();
    }

    public virtual void GetSpecialInteraction(SpecialInteraction.WindData source)
    {

    }
    public virtual void GetSpecialInteraction(SpecialInteraction.WaterData source)
    {

    }
    public virtual void GetSpecialInteraction(SpecialInteraction.FireData source)
    {

    }

    public void AddForce(ForceInfo info)
    {

    }
    public virtual void AddForce(Vector3 direction, ForceType forceType)
    {

    }
    private void AddForceVelocity(Vector3 direction)
    {

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

}
