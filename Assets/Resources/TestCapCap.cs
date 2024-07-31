using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCapCap : MyComponent, CameraTarget
{
    FirstViewCameraData tempt;
    protected override void MyStart()
    {
        base.MyStart();

        tempt = new();
        GameManager.ObjectsUpdate += TestUpdate;
        GameManager.ObjectsFixedUpdate += TestFixedUpdate;
    }


    private void TestUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, CameraType.FirstView);
        }
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
        tempt.cameraPosition = transform.position;// + Vector3.back * 5;
        tempt.cameraForward = transform.forward;

        return tempt;
    }
    public ThirdViewCameraData ThirdViewCameraSet()
    {
        return default;
    }
}
