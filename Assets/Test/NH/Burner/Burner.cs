using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : LimitPositionObject, IOnOffFuncInteraction
{
    [SerializeField] GameObject fire;

    public void DoOn()
    {
        fire.SetActive(true);
    }

    public void DoOff()
    {
        fire.SetActive(false);
    }

}
