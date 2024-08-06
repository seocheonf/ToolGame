using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : UniqueTool
{
    protected override void Initialize()
    {
        base.Initialize();

        //��� ��� �۾�

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;




        //��� �غ� �۾�




    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //��� ���� �۾�

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;

    }

    public override void GetSpecialInteraction(WindData source)
    {


        //�� ���� �ʿ�

        
        float dotValue = Vector3.Dot(transform.up, source.Direction);

        if(holdingCharacter == null)
        {
            receivedForceQueue.Enqueue(new ForceInfo(transform.up * dotValue * 5f * source.intensity, ForceType.VelocityForce));
            receivedForceQueue.Enqueue(new ForceInfo(source.Direction * source.intensity, ForceType.VelocityForce));
        }




    }

    private void CustomFixedUpdate(float fixedDeltaTime)
    {
        //�ϰ� ���� ��, �ϰ� �ӵ� ����
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
