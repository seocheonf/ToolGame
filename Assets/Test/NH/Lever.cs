using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : LimitPositionObject, IOnOffFuncInteraction
{
    [SerializeField] GameObject leverSwitch;
    [SerializeField] LimitPositionObject targetObject;

    bool isActive = false;

    public List<FuncInteractionData> GetOnOffFuncInteractionList()
    {
        List<FuncInteractionData> add = new List<FuncInteractionData>();
        return add;
    }

    public override void ObjectClick()
    {
        isActive = !isActive;
    }

    void LeverUpdate()
    {
        if (isActive) LeverOn();
        else LeverOff();
        SwitchObjectCheck();
    }

    void LeverOn()
    {
        targetObject.ObjectOn();
    }

    void LeverOff()
    {
        targetObject.ObjectOff();
    }

    void SwitchObjectCheck()
    {
        if (isActive)
        {
            leverSwitch.transform.rotation = Quaternion.Euler(0, 0, -45);
        }
        else
        {
            leverSwitch.transform.rotation = Quaternion.Euler(0, 180, -45);
        }
    }

    private void Update()
    {
        LeverUpdate();
    }
}
