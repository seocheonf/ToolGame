using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MeshSet
{
    
    public MeshFilter meshFilter;

    public MeshSet(MeshFilter meshFilter, Mesh mesh)
    {
        this.meshFilter = meshFilter;
        MeshSetting(mesh);
    }

    public void MeshSetting(Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }
    

}

//[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Umbrella : UniqueTool
{

    //

    [SerializeField]
    private ResourceEnum.Material umbrellaMaterial;
    private Mesh umbrellaOpen;
    private Mesh umbrellaClosed;

    private MeshRenderer umbrellaMeshRenderer;

    private MeshSet umbrellaMeshSet;

    private MeshCollider umbrellaOpenCollider;
    private MeshCollider umbrellaClosedCollider;

    //true�� ��ħ, false�� ����
    private bool umbrellaMode = true;
    private bool UmbrellaMode
    {
        get => umbrellaMode;
        set
        {
            umbrellaMode = value;
            umbrellaMeshSet.MeshSetting(CurrentMesh);
            if(umbrellaMode)
            {
                umbrellaOpenCollider.enabled = true;
                physicsInteractionObjectCollider = umbrellaOpenCollider;
                umbrellaClosedCollider.enabled = false;
            }
            else
            {
                umbrellaOpenCollider.enabled = false;
                physicsInteractionObjectCollider = umbrellaClosedCollider;
                umbrellaClosedCollider.enabled = true;
            }
        }
    }
    
    private Mesh CurrentMesh
    {
        get
        {
            return UmbrellaMode ? umbrellaOpen : umbrellaClosed;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        //���� �ʱ�ȭ
        umbrellaOpen = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaOpen);
        umbrellaClosed = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaClosed);

        //umbrellaMeshRenderer = GetComponent<MeshRenderer>();
        //umbrellaMeshRenderer.material = ResourceManager.GetResource(umbrellaMaterial);

        umbrellaMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        umbrellaMeshRenderer.material = ResourceManager.GetResource(umbrellaMaterial);

        umbrellaOpenCollider = gameObject.AddComponent<MeshCollider>();
        umbrellaOpenCollider.convex = true;
        umbrellaOpenCollider.sharedMesh = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaOpen);
        physicsInteractionObjectCollider = umbrellaOpenCollider;

        umbrellaClosedCollider = gameObject.AddComponent<MeshCollider>();
        umbrellaClosedCollider.convex = true;
        umbrellaClosedCollider.sharedMesh = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaClosed);
        umbrellaClosedCollider.enabled = false;

        umbrellaMeshSet = new MeshSet(GetComponent<MeshFilter>(), CurrentMesh);

        //��� ��� �۾�

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;




        //��� �غ� �۾�

        holdingFuncInteractionList.Add(new FuncInteractionData(KeyCode.Tab, "��� ��ġ�� ����", UmbrellaModeChange, null, null));


    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //��� ���� �۾�

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;

    }

    public override void GetSpecialInteraction(WindData source)
    {

        //�� ���� �ʿ�
      

        //�Ϲ����� �� �ۿ�
        base.GetSpecialInteraction(source);

        //Ư���ۿ� - ������ ���� ����
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
            //�ϰ� ���� ��, �ϰ� �ӵ� ����
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

    //0 - ��, 1 - ��, 2 - ��, 3 - ��, 4 - ��
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
