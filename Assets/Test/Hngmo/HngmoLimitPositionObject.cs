using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HngmoLimitPositionObject : LimitPositionObject
{
    public Umbrella umbrellaTest;

    protected override void Initialize()
    {
        base.Initialize();

    }

    protected override void MyStart()
    {
        base.MyStart();
        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsUpdate += CustomUpdate;
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
    }

    private void CustomUpdate(float deltaTime)
    {

        /*
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            umbrellaTest.PickUpTool(null);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            umbrellaTest.PutTool();
        }
        */

    }

    private void CustomFixedUpdate(float fixedDeltaTime)
    {

        while (receivedForceQueue.TryDequeue(out ForceInfo result))
        {
            AddForce(result);
        }

    }
}
