using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : LimitPositionObject, IOnOffFuncInteraction, ITriggerFuncInteraction
{
    [SerializeField] GameObject leverSwitch;
    [SerializeField] MyComponent targetObject;
    IOnOffFuncInteraction targetOnOff;

    bool isActive = false;

    protected override void Initialize()
    {
        base.Initialize();

        targetOnOff = targetObject.GetComponent<IOnOffFuncInteraction>();

#if UNITY_EDITOR
        if (targetOnOff == null)
        {
            Debug.LogError("등록한 타겟 오브젝트에 레버에서 사용할 On/Off용 인터페이스가 없어요!");
        }
#endif

        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsUpdate += CustomUpdate;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        GameManager.ObjectsUpdate -= CustomUpdate;
    }

    public void DoTrigger()
    {
        isActive = !isActive;
    }

    void LeverUpdate()
    {
        if (isActive) DoOn();
        else DoOff();
        SwitchObjectCheck();
    }

    public void DoOn()
    {
        targetOnOff.DoOn();
    }

    public void DoOff()
    {
        targetOnOff.DoOff();
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

    private void CustomUpdate(float deltaTime)
    {
        LeverUpdate();
    }
}
