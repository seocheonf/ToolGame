using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HngmoPlayable : Playable
{
    protected override void MyStart()
    {
        base.MyStart();

        FuncInteractionData AboutTool = (new FuncInteractionData(KeyCode.Mouse1, "우산 들기", ToolSet, null, null));
        ControllerManager.AddInputFuncInteraction(AboutTool);
    }

    public UniqueTool sampleTool;
    private void ToolSet()
    {
        Debug.Log("adsf");
        if (currentHoldingUniqueTool == null)
            PickUpTool(sampleTool);
        else
            PutTool();
    }


    public override Vector3 CurrentSightEulerAngle
    {
        get
        {
            Vector3 result = Vector3.zero;
            result.x = -xRot;
            result.y = yRot;
            return result;
        }
    }

}