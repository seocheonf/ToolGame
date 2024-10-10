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
    bool currentRadiusDirection = false; // �����̸� �ٱ�������

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
     * ���� ī�޶� ��ġ���� ���� ���� ���� ���⺤�͸� deltaTime�� ���ؼ� ���ϰ�,
     * ���� radius�� �Ѿ�ٸ� �ٽ� ����ֵ��� �Ѵ�.
     * ���� ���� radius�� Ŭ ����, �߽������� �������ϴ� ���⺤�͸� ������ ���ɼ��� �� ����.
     * ������ ������ �ϴ�, ������ ������ ���ÿ�, ����ġ�� �ξ ��� ������ �����ϰ� �Ѵ�.
     * 
     * ����ġ�� radius���⼺, right���⼺, up���⼺
     * 
     * �ֱٿ� ��� Ư�� �������� ���� ���õǾ��ٸ�, ����(up, forward, right�� ���� +, -)�� ����Ȯ��(����)�� �޶����� ��.
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
