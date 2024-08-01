using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCapCap : MyComponent, ICameraTarget
{
    float sensitivity = 100f;
    float Xrot;
    float Yrot;
    float clampAngle = 89;

    FirstViewCameraData tempt;
    ThirdViewCameraData tempt2;

    Vector3 forward;

    protected override void MyStart()
    {
        base.MyStart();

        tempt = new();
        tempt2 = new();
        GameManager.ObjectsUpdate += TestUpdate;
        GameManager.ObjectsFixedUpdate += TestFixedUpdate;
    }


    private void TestUpdate(float deltaTime)
    {
        Xrot += -ControllerManager.MouseMovement.y * sensitivity * deltaTime;
        Yrot += ControllerManager.MouseMovement.x * sensitivity * deltaTime;
        Xrot = Mathf.Clamp(Xrot, -clampAngle, clampAngle);
        transform.rotation = Quaternion.Euler(Xrot, Yrot, 0);
        forward = transform.forward;
        transform.rotation = Quaternion.Euler(0, Yrot, 0);



        /*
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraType.FirstView);
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraType.ThirdView);
        }
        */
    }
    private void TestFixedUpdate(float deltaTime)
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            transform.position += Vector3.up * deltaTime;
        }

        if (Input.GetKey(KeyCode.F1))
        {
            transform.Rotate(Vector3.up, deltaTime * 30f);
        }
    }



    public FirstViewCameraData FirstViewCameraSet()
    {
        tempt.targetPosition = transform.position;// + Vector3.back * 5;
        tempt.targetForward = forward;

        return tempt;
    }
    public ThirdViewCameraData ThirdViewCameraSet()
    {
        tempt2.targetPosition = transform.position;
        tempt2.Xrot = Xrot;
        tempt2.Yrot = Yrot;
        tempt2.minDistance = 3;
        tempt2.maxDistance = 10;
        tempt2.TPPOffsetY = 1f;
        tempt2.TPPOffsetZ = 5f;

        return tempt2;
    }
}
