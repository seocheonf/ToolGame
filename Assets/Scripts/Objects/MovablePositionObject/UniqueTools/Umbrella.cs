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

class UmbrellaConditionData
{
    //대상이 되는 우산
    private Umbrella targetUmbrella;
    //현재 우산 상황에서 할 수 있는 기능 모음
    private List<FuncInteractionData> holdingFuncInteractionList;

    public UmbrellaConditionData(Umbrella targetUmbrella, List<FuncInteractionData> holdingFuncInteractionList)
    {
        this.targetUmbrella = targetUmbrella;
        this.holdingFuncInteractionList = holdingFuncInteractionList;
    }

    //현재 우산 상황이 되었을 때 해야할 일
    public void SetCondition()
    {
        
    }

    //현재 우산 상황에서 빠져 나올 때 해야할 일
    public void UnsetCondition()
    {
        
    }





}

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Umbrella : UniqueTool
{

    enum UmbrellaCondition
    {
        Closed,
        Open,
        Reverse,
        Hook
    }

    

    [SerializeField]
    UmbrellaCondition initialUmbrellaCondition;
    UmbrellaCondition currentUmbrellaCondition;

    Dictionary<UmbrellaCondition, List<FuncInteractionData>> umbConditionFuncDictionary;

    private void ChangeUmbrellaCondition(UmbrellaCondition change)
    {

        if(holdingCharacter != null)
        {
            //기능 넣고 빼기는 들고 있을 때만.
            ChangeUmbrellaFuncInteraction(change);
        }



        //마지막에 식별해주는 친구 바꾸기
        currentUmbrellaCondition = change;
        holdingFuncInteractionList = ;//바뀐 놈 List로 갱신
    }
    private void ChangeUmbrellaFuncInteraction(UmbrellaCondition change)
    {
        //current의 List를 remove요청, change의 List를 add요청

        ControllerManager.RemoveInputFuncInteraction(holdingFuncInteractionList);
        ControllerManager.AddInputFuncInteraction();//바뀐 놈 List로 갱신
    }


    [SerializeField]
    private ResourceEnum.Material umbrellaMaterial;
    private Mesh umbrellaOpen;
    private Mesh umbrellaClosed;

    private MeshRenderer umbrellaMeshRenderer;

    private MeshSet umbrellaMeshSet;

    private MeshCollider umbrellaOpenCollider;
    private MeshCollider umbrellaClosedCollider;

    //true는 펼침, false는 닫힘
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

        //변수 초기화
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
