using System.Collections;
using System.Collections.Generic;
using UnityEngine;


delegate void SemiCustomCamera(Camera main, float deltaTime);

public class FestivalOfFinish_CustomCameraArea : MyComponent, ICameraTarget
{

    [SerializeField]
    private Transform[] cameraMoveVertexRef;

    private Vector3[] cameraMoveVertex;

    private int vertexCount;
    private int currentIndex = 0;
    private int nextIndex;

    private float currentTime = 0;
    [SerializeField]
    private float endTime;

    private Playable target;

    private void Awake()
    {
        vertexCount = cameraMoveVertexRef.Length;
        cameraMoveVertex = new Vector3[vertexCount];

        for(int i = 0; i < vertexCount; i++)
        {
            //area�� ũ�⸦ �ݿ��Ͽ�, ī�޶� ��ġ ��ǥ�� ��Ÿ���� ������Ʈ�� local��ǥ�� �����ش�. ��, ��ġ ��ǥ ��Ÿ���� ������Ʈ�� �� area������Ʈ�� �ڽ��̾�� �Ѵ�...
            Vector3 vector3 = new Vector3(cameraMoveVertexRef[i].transform.localPosition.x * transform.lossyScale.x, cameraMoveVertexRef[i].transform.localPosition.y * transform.lossyScale.y, cameraMoveVertexRef[i].transform.localPosition.z * transform.lossyScale.z);
            cameraMoveVertex[i] = vector3;
            Debug.Log(vector3);
        }
    }

    public void CustomViewCameraSet(Camera main, float deltaTime)
    {
        nextIndex = currentIndex == vertexCount - 1 ? 0 : currentIndex + 1;
        Vector3 nextPosition = Vector3.Lerp(cameraMoveVertex[currentIndex], cameraMoveVertex[nextIndex], currentTime / endTime) ;
        currentTime += deltaTime;
        main.transform.position = target.transform.position + nextPosition;

        if(currentTime > endTime)
        {
            currentIndex = currentIndex == vertexCount - 1 ? 0 : currentIndex + 1;
            currentTime = 0;
        }

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
