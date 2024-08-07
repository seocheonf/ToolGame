using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HngmoCharacter : Character, ICameraTarget
{
    public UniqueTool sampleTool;

    FuncInteractionData AboutTool;

    protected override void Initialize()
    {
        base.Initialize();

        AboutTool = (new FuncInteractionData(KeyCode.Mouse1, "우산 들기", ToolSet, null, null));
    }


    protected override void MyStart()
    {
        base.MyStart();
        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsUpdate += CustomUpdate;
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;

        ControllerManager.AddInputFuncInteraction(AboutTool);

        GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraType.ThirdView);
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        GameManager.ObjectsUpdate -= CustomUpdate;
        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;

        ControllerManager.RemoveInputFuncInteraction(AboutTool);

    }

    private void CustomUpdate(float deltaTime)
    {
        if(Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * deltaTime * 5f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * deltaTime * 5f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * deltaTime * 5f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position -= Vector3.left * deltaTime * 5f;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddForce(Vector3.up * 10f, ForceType.VelocityForce);
        }
    }

    private void CustomFixedUpdate(float fixedDeltaTime)
    {

        while (receivedForceQueue.TryDequeue(out ForceInfo result))
        {
            AddForce(result);
        }

    }

    private void ToolSet()
    {
        if (currentHoldingUniqueTool == null)
            PickUpTool(sampleTool);
        else
            PutTool();
    }


    public override void PickUpTool(UniqueTool target)
    {
        base.PickUpTool(target);
        target.FakeCenterPosition = catchingLocalPosition + transform.position;
    }

    public FirstViewCameraData FirstViewCameraSet()
    {
        FirstViewCameraData tempt = new FirstViewCameraData();
        tempt.SetInfo(transform.position, transform.forward);
        return tempt;
    }
    public ThirdViewCameraData ThirdViewCameraSet()
    {
        ThirdViewCameraData tempt = new ThirdViewCameraData();
        tempt.SetInfo(transform.position, 0, 0, 3, 10, 1, 5);
        return tempt;
    }


}
