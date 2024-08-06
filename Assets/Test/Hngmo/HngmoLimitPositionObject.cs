using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HngmoLimitPositionObject : LimitPositionObject
{
    protected override void Initialize()
    {
        base.Initialize();

    }

    protected override void MyStart()
    {
        base.MyStart();
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
    }

    private void CustomFixedUpdate(float fixedDeltaTime)
    {

        while (receivedForceQueue.TryDequeue(out ForceInfo result))
        {
            AddForce(result);
        }

    }
}
