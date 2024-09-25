using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedUIGenerateTest : LimitPositionObject
{
    FixedUIComponent ui1;
    FloatingUITest ui2;
    protected override void Initialize()
    {
        base.Initialize();
        ui1 = GameManager.Instance.UI.GetFixedUI<PlayableInputUI>(ToolGame.FixedUIType.PlayableInputUI);
        ui2 = GameManager.Instance.UI.GetFloatingUI<FloatingUITest>(ToolGame.FloatingUIType.FloatingUITest);
    }






    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            ui1.TriggerActive();
            //ui2.TriggerActive();
            //GameObject.Instantiate(ResourceManager.GetResource(ResourceEnum.Prefab.UI_Floating_FloatingUITest));
        }
    }
}
