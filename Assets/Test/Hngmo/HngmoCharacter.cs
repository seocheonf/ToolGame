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
            transform.position += transform.forward * deltaTime * 5f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * deltaTime * 5f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * deltaTime * 5f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * deltaTime * 5f;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddForce(Vector3.up * 10f, ForceType.VelocityForce);
        }

        xRot += ControllerManager.MouseMovement.x;
        yRot += ControllerManager.MouseMovement.y;
        transform.eulerAngles = new Vector3(0, xRot, 0);
    }

    float xRot;
    float yRot;

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
        target.FakeCenterPosition = transform.position + CatchingLocalPosition;
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
        tempt.SetInfo(transform.position, -yRot, xRot, 3, 10, 1, 5);
        return tempt;
    }


}
