using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MeshSet
{
    
    public MeshCollider meshCollider;
    public MeshFilter meshFilter;

    public MeshSet(MeshCollider meshCollider, MeshFilter meshFilter, Mesh mesh)
    {
        this.meshCollider = meshCollider;
        this.meshFilter = meshFilter;
        MeshSetting(mesh);
    }

    public void MeshSetting(Mesh mesh)
    {
        meshFilter.mesh = mesh;
        //meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
    

}

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Umbrella : UniqueTool
{

    //

    [SerializeField]
    private ResourceEnum.Material umbrellaMaterial;
    private Mesh umbrellaOpen;
    private Mesh umbrellaClosed;

    private MeshRenderer umbrellaMeshRenderer;

    private MeshSet umbrellaMeshSet;

    //true는 펼침, false는 닫힘
    private bool umbrellaMode = true;
    private bool UmbrellaMode
    {
        get => umbrellaMode;
        set
        {
            
            //CurrentMesh2.enabled = false;
            umbrellaMode = value;
            //CurrentMesh2.enabled = true;
            //umbrellaMeshSet.meshFilter.mesh = CurrentMesh;
            umbrellaMeshSet.MeshSetting(CurrentMesh);
        }
    }
    /*
    private enum UmbrellaMode
    {
        Open,
        Closed
    }

    private UmbrellaMode _umbrellaMode;
    private UmbrellaMode _UmbrellaMode
    {
        set
        {

        }
    }
    */

    
    private Mesh CurrentMesh
    {
        get
        {
            return UmbrellaMode ? umbrellaOpen : umbrellaClosed;
        }
    }
    

    //private MeshCollider CurrentMesh2
    //{
    //    get
    //    {
    //        return UmbrellaMode ? open : closed;
    //    }
    //}


    //




    protected override void Initialize()
    {
        base.Initialize();

        //변수 초기화
        umbrellaOpen = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaOpen);
        umbrellaClosed = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaClosed);

        umbrellaMeshRenderer = GetComponent<MeshRenderer>();
        umbrellaMeshRenderer.material = ResourceManager.GetResource(umbrellaMaterial);

        umbrellaMeshSet = new MeshSet(physicsInteractionObjectCollider as MeshCollider, GetComponent<MeshFilter>(), CurrentMesh);

        //기능 등록 작업

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;




        //기능 준비 작업

        holdingFuncInteractionList.Add(new FuncInteractionData(KeyCode.Tab, "우산 펼치고 접기", UmbrellaModeChange, null, null));


    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //기능 해제 작업

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;

    }

    public override void GetSpecialInteraction(WindData source)
    {

        //힘 보정 필요
      

        //일반적인 힘 작용
        base.GetSpecialInteraction(source);

        //특수작용 - 펼쳐져 있을 때만
        if (UmbrellaMode)
        {
            float dotValue = Vector3.Dot(transform.up, source.Direction);
            receivedForceQueue.Enqueue(new ForceInfo(transform.up * dotValue * 5f * source.intensity, ForceType.VelocityForce));
        }

    }

    private void CustomFixedUpdate(float fixedDeltaTime)
    {

        //FakeCenterRotation *= Quaternion.Euler(1, 0, 0);
        
        ApplyForce();

    }

    private void UmbrellaModeChange()
    {
        UmbrellaMode = !UmbrellaMode;
    }

    private void ApplyForce()
    {
        if(holdingCharacter == null)
        {
            //하강 중일 때, 하강 속도 감소
            if (physicsInteractionObjectRigidbody.velocity.y <= 0 && UmbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                Vector3 downVelocity = physicsInteractionObjectRigidbody.velocity;
                downVelocity.y *= (1 - dotValue * 0.05f);
                physicsInteractionObjectRigidbody.velocity = downVelocity;
            }

            while (receivedForceQueue.TryDequeue(out ForceInfo result))
            {
                AddForce(result);
            }
        }
        else
        {
            if (holdingCharacter.GetDownSpeed() <= 0 && UmbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                holdingCharacter.AccelDownForce((1 - dotValue * 0.05f));
            }

            while (receivedForceQueue.TryDequeue(out ForceInfo result))
            {
                holdingCharacter.AddForce(result);
            }
        }
    }

    int umbDir = 0;

    public override void PickUpTool(Character source)
    {
        base.PickUpTool(source);
        //UmbrellaDirectionChange(umbDir);
    }

    //0 - 앞, 1 - 뒤, 2 - 위, 3 - 좌, 4 - 우
    private void UmbrellaDirectionChange(int dir)
    {
        switch (dir)
        {
            case 0:
                transform.up = holdingCharacter.transform.forward;
                break;
            case 1:
                transform.up = -holdingCharacter.transform.forward;
                break;
            case 2:
                transform.up = holdingCharacter.transform.up;
                break;
            case 3:
                transform.up = -holdingCharacter.transform.right;
                break;
            case 4:
                transform.up = holdingCharacter.transform.right;
                break;

        }
    }

}
