using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurationButton : LimitPositionObject, IOnOffFuncInteraction
{
    [SerializeField] GameObject buttonSwitch;
    [SerializeField] LimitPositionObject targetObject;
    [SerializeField] float durationTime;
    [SerializeField] float buttonTime;
    bool isActive = false;
    bool isButtonActive = false;

    public List<FuncInteractionData> GetOnOffFuncInteractionList()
    {
        return default;
    }

    public override void ObjectClick()
    {
        isActive = true;
        isButtonActive = true;
    }

    void ButtonOn()
    {
        targetObject.ObjectOn();
    }

    void ButtonOff()
    {
        targetObject.ObjectOff();
    }

    void SwitchObjectCheck(float deltaTime)
    {
        if (isButtonActive == true)
        {
            buttonTime += deltaTime;
            buttonSwitch.transform.localPosition = new Vector3(0.2f, 0.25f, 0);
        }

        if (buttonTime > 0.75f && isButtonActive)
        {
            buttonSwitch.transform.localPosition = new Vector3(0.3f, 0.25f, 0);
            buttonTime = 0;
            isButtonActive = false;
        }
        
    }

    void ButtonUpdate()
    {
        if (isActive)
        {
            ButtonOn();
            durationTime = durationTime - Time.deltaTime;
            if (durationTime <= 0) isActive = false;
        }
        else
        {
            ButtonOff();
            durationTime = 1.5f;
        }
        SwitchObjectCheck(Time.deltaTime);
    }

    private void Update()
    {
        ButtonUpdate();
        //Debug.Log(buttonTime);
    }
}
