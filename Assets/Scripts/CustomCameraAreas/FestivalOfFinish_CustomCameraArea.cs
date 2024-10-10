using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

delegate void SemiCustomCamera(Camera main, float deltaTime);

public class FestivalOfFinish_CustomCameraArea : MyComponent, ICameraTarget
{

    [SerializeField]
    float minRadius;
    [SerializeField]
    float maxRadius;

    float currentRadius;
    bool currentRadiusDirection = false; // 거짓이면 바깥쪽으로

    Playable target;

    SemiCustomCamera FB;

    private void Awake()
    {
        FB = Forward;
    }

    public void CustomViewCameraSet(Camera main, float deltaTime)
    {
        FB(main, deltaTime);

        //currentRadius = currentRadiusDirection ? currentRadius - deltaTime * 5f : currentRadius + deltaTime * 5f;
        //currentRadiusDirection = Mathf.Abs(currentRadius) >= maxRadius ? true : false;

        //main.transform.position = Vector3.back * currentRadius;
        //main.transform.LookAt(target.transform);
    }

    private void Forward(Camera main, float deltaTime)
    {
        currentRadius = currentRadius - deltaTime * 5f;
        main.transform.position = Vector3.back * currentRadius;
        main.transform.LookAt(target.transform);

        if (Mathf.Abs(currentRadius) >= maxRadius)
        {
            FB = null;
            FB += Backward;
        }
    }

    /*
     * 
     * 현재 카메라 위치에서 내가 가고 싶은 방향벡터를 deltaTime을 곱해서 더하고,
     * 만약 radius가 넘어간다면 다시 당겨주도록 한다.
     * 만약 현재 radius가 클 수록, 중심쪽으로 가고자하는 방향벡터를 선택할 가능성이 더 높다.
     * 앞으로 가고자 하는, 선택할 방향의 선택에, 가중치를 두어서 어떻게 갈지를 결정하게 한다.
     * 
     * 가중치는 radius경향성, right경향성, up경향성
     * 
     * 최근에 어느 특정 방향으로 많이 선택되었다면, 방향(up, forward, right에 대한 +, -)의 선택확률(비율)이 달라지는 것.
     * 
     */

    private void Backward(Camera main, float deltaTime)
    {


        if (Mathf.Abs(currentRadius) <= minRadius)
        {
            FB = null;
            FB += Forward;
        }
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
        if (other.TryGetComponent(out Playable result))
        {
            target = result;
            GameManager.Instance.CurrentWorld.WorldCamera.CameraSet(this, ToolGame.CameraViewType.Custom);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Playable result))
        {
            target = null;
            GameManager.Instance.CurrentWorld.WorldCamera.CameraBreak(this);
        }
    }
}
