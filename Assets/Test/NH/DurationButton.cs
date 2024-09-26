using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurationButton : LimitPositionObject, IOnOffFuncInteraction, ITriggerFuncInteraction
{
    [SerializeField] GameObject buttonSwitch;
    [SerializeField] MyComponent targetObject;
    [SerializeField] IOnOffFuncInteraction targetOnOff;
    [SerializeField] float durationTime;
    [SerializeField] float buttonTime;
    bool isActive = false;
    bool isButtonActive = false;

    protected override void Initialize()
    {
        base.Initialize();

        targetOnOff = targetObject.GetComponent<IOnOffFuncInteraction>();

#if UNITY_EDITOR
        if(targetOnOff == null)
        {
            Debug.LogError("등록한 타겟 오브젝트에 버튼에서 사용할 On/Off용 인터페이스가 없어요!");
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
        isActive = true;
        isButtonActive = true;
    }

    public void DoOn()
    {
        targetOnOff.DoOn();
    }

    public void DoOff()
    {
        targetOnOff.DoOff();
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
            DoOn();
            durationTime = durationTime - Time.deltaTime;
            if (durationTime <= 0) isActive = false;
        }
        else
        {
            DoOff();
            durationTime = 1.5f;
        }
        SwitchObjectCheck(Time.deltaTime);
    }

    private void CustomUpdate(float deltaTime)
    {
        ButtonUpdate();
        //Debug.Log(buttonTime);
    }
}
