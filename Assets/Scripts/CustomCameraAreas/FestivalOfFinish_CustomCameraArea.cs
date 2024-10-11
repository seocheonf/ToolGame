using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    Vector3 nextLocalPosition = Vector3.zero;
    bool pm = true;

    public void CustomViewCameraSet(Camera main, float deltaTime)
    {
        float x_p = Random.Range(0, 1f);
        float y_p = Random.Range(0, 1f);
        float z_p = Random.Range(0, 1f);

        float x_m = Random.Range(-1f, 0);
        float y_m = Random.Range(-1f, 0);
        float z_m = Random.Range(-1f, 0);

        Vector3 vector3;

        if (pm)
            vector3 = new Vector3(x_p, y_p, z_p);
        else
            vector3 = new Vector3(x_m, y_m, z_m);

        nextLocalPosition += vector3 * deltaTime * 5f;

        if(nextLocalPosition.magnitude > maxRadius)
        {
            pm = !pm;
        }
        
        main.transform.position = target.transform.position + nextLocalPosition;


        main.transform.LookAt(target.transform);

        //FB(main, deltaTime);

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
