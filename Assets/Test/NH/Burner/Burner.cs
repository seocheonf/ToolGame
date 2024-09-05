using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : LimitPositionObject
{
    [SerializeField] GameObject fire;
    
    public override void ObjectOn()
    {
        fire.SetActive(true);
    }

    public override void ObjectOff()
    {
        fire.SetActive(false);
    }
}
