using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HngmoCharacter : Character, ICameraTarget
{
    public UniqueTool sampleTool;

    public float sampleSensitive;

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

        GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraViewType.ThirdView);
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
        

        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddForce(Vector3.up * 10f, ForceType.VelocityForce);
        }


    }

    float xRot;
    float yRot;

    private void CustomFixedUpdate(float fixedDeltaTime)
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * fixedDeltaTime * 10f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * fixedDeltaTime * 10f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * fixedDeltaTime * 10f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * fixedDeltaTime * 10f;
        }


        xRot += ControllerManager.MouseMovement.x * sampleSensitive;
        yRot += ControllerManager.MouseMovement.y * sampleSensitive;
        transform.eulerAngles = new Vector3(0, xRot, 0);
    
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


    public override Vector3 CurrentSightEulerAngle
    {
        get
        {
            Vector3 result = Vector3.zero;
            result.x = -yRot;
            result.y = xRot;
            return result;
        }
    }
}
