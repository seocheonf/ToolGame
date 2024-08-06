using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : UniqueTool
{
    protected override void Initialize()
    {
        base.Initialize();

        //기능 등록 작업

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;




        //기능 준비 작업




    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //기능 해제 작업

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;

    }

    public override void GetSpecialInteraction(WindData source)
    {


        //힘 보정 필요

        
        float dotValue = Vector3.Dot(transform.up, source.Direction);

        if(holdingCharacter == null)
        {
            receivedForceQueue.Enqueue(new ForceInfo(transform.up * dotValue * 5f * source.intensity, ForceType.VelocityForce));
            receivedForceQueue.Enqueue(new ForceInfo(source.Direction * source.intensity, ForceType.VelocityForce));
        }




    }

    private void CustomFixedUpdate(float fixedDeltaTime)
    {
        //하강 중일 때, 하강 속도 감소
        if(physicsInteractionObjectRigidbody.velocity.y <= 0)
        {
            float dotValue = Vector3.Dot(Vector3.up, transform.up);
            Vector3 downVelocity = physicsInteractionObjectRigidbody.velocity;
            downVelocity.y *= (1 - dotValue * 0.05f);
            physicsInteractionObjectRigidbody.velocity = downVelocity;
        }

        while (receivedForceQueue.TryDequeue(out ForceInfo result))
        {
            AddForce(result);
        }

        Debug.Log(physicsInteractionObjectRigidbody.velocity.magnitude);
    }


}
