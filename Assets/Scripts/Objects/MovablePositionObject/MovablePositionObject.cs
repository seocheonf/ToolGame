using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovablePositionObject : PhysicsInteractionObject, ITeleportTarget_Portal
{
    /*
    protected float initialMass;
    protected float currentMass;
    */
    /*
    protected override void Initialize()
    {
        base.Initialize();

        initialMass = currentRigidbody.mass;
        currentMass = currentRigidbody.mass;
    }
    */
    public virtual void TP_Portal(Vector3 targetPosition)
    {
        transform.position = targetPosition;
        currentRigidbody.velocity = Vector3.zero;
    }
}
