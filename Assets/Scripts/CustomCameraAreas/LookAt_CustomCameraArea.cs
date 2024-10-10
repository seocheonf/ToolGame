using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt_CustomCameraArea : MyComponent, ICameraTarget
{

    [SerializeField]
    GameObject target;

    [SerializeField]
    float upPosition;
    [SerializeField]
    float rightPosition;
    [SerializeField]
    float forwardPosition;

    private Vector3 lookAtCameraPosition;

    //일단 계획은 절대 값 초기화라 awake단에서 가능.
    private void Awake()
    {
        lookAtCameraPosition = target.transform.position + Vector3.up * upPosition + Vector3.right * rightPosition + Vector3.forward * forwardPosition;
    }

    public void CustomViewCameraSet(Camera main, float deltaTime)
    {
        main.transform.position = lookAtCameraPosition;
        main.transform.LookAt(target.transform);
    }

    public FirstViewCameraData FirstViewCameraSet()
    {
        return default;
    }

    public ThirdViewCameraData ThirdViewCameraSet()
    {
        return default;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Playable result))
        {
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, ToolGame.CameraViewType.Custom);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Playable result))
        {
            GameManager.Instance.CurrentWorld.WorldCamera.CameraBreak(this);
        }
    }
}
