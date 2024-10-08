using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitPositionObject : PhysicsInteractionObject
{
    protected override void RegisterFuncInInitialize()
    {
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
        GameManager.ObjectsFixedUpdate += MainFixedUpdate;
    }
    protected override void RemoveFuncInDestroy()
    {
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
    }
}
