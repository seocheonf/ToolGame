using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class UmbrellaConditionData
{
    //����� �Ǵ� ���
    private Umbrella targetUmbrella;
    //���� ��� ��Ȳ���� �� �� �ִ� ��� ����
    private List<FuncInteractionData> holdingFuncInteractionList;

    public UmbrellaConditionData(Umbrella targetUmbrella, List<FuncInteractionData> holdingFuncInteractionList)
    {
        this.targetUmbrella = targetUmbrella;
        this.holdingFuncInteractionList = holdingFuncInteractionList;
    }

    //���� ��� ��Ȳ�� �Ǿ��� �� �ؾ��� ��
    public void SetCondition()
    {
        
    }

    //���� ��� ��Ȳ���� ���� ���� �� �ؾ��� ��
    public void UnsetCondition()
    {
        
    }





}

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Umbrella : UniqueTool
{
    enum UmbrellaCondition
    {
        Open,
        Closed,
        Reverse,
        Hook,
        Length
    }

    #region ����� ����

    enum UmbrellaDirection
    {
        Sight,
        Fixed,
        Free
    }
    private UmbrellaDirection currentStandardaAngle;


    #endregion


    [SerializeField]
    UmbrellaCondition initialCondition;
    UmbrellaCondition currentCondition;

    Dictionary<UmbrellaCondition, List<FuncInteractionData>> conditionFuncInteractionDictionary;

    private void ChangeCondition<T>(T currentCondition, T newCondition, Dictionary<T, List<FuncInteractionData>> conditionFunc) where T : System.Enum
    { 
        //��� �ְ� ����� ��� ���� ����.
        if (holdingCharacter != null)
        {
            //current�� List�� remove��û, change�� List�� add��û
            ControllerManager.RemoveInputFuncInteraction(conditionFunc[currentCondition]);
            //�ٲ� �� List�� ����
            ControllerManager.AddInputFuncInteraction(conditionFunc[newCondition]);
        }
        //�������� �ĺ����ִ� ģ�� �ٲٱ�
        currentCondition = newCondition;
    }


    [SerializeField]
    private ResourceEnum.Material umbrellaMaterial;

    private Mesh umbrellaOpen;
    private Mesh umbrellaClosed;

    private MeshFilter umbrellaMeshfilter;

    [SerializeField]
    private MeshRenderer umbrellaMeshRenderer;

    [SerializeField]
    private MeshCollider umbrellaOpenCollider;
    [SerializeField]
    private MeshCollider umbrellaClosedCollider;

    /// <summary>
    /// ����� ��ħ�� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="mode">true�� ��ħ, false�� ����</param>
    private void SetUmbrellaMode(bool mode)
    {
        if (mode)
        {
            umbrellaOpenCollider.enabled = true;
            umbrellaClosedCollider.enabled = false;
            umbrellaMeshfilter.mesh = umbrellaOpen;
            physicsInteractionObjectCollider = umbrellaOpenCollider;
        }
        else
        {
            umbrellaClosedCollider.enabled = true;
            umbrellaOpenCollider.enabled = false;
            umbrellaMeshfilter.mesh = umbrellaClosed;
            physicsInteractionObjectCollider = umbrellaClosedCollider;
        }
    }

    /// <summary>
    /// ����� ���� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="jointPoint">true�� ������, false�� �Ǵٸ�</param>
    private void SetUmbrellaJointPoint(bool jointPoint)
    {
        if(jointPoint)
        {
            catchedLocalPosition = catchedLocalPositionKnob;
        }
        else
        {
            catchedLocalPosition = catchedLocalPositionKnobReverse;
        }
    }
    //������ ��ġ
    protected Vector3 catchedLocalPositionKnob;
    //�Ǵٸ� ��ġ
    protected Vector3 catchedLocalPositionKnobReverse;

#if UNITY_EDITOR
    enum JointPoint
    {
        Knob,
        KnobReverse
    }
    [SerializeField]
    JointPoint settingJointPoint;
    public override Vector3 CatchedLocalPositionEdit
    {
        get
        {
            if(settingJointPoint == JointPoint.Knob)
                return transform.position + catchedLocalPositionKnob;
            else
                return transform.position + catchedLocalPositionKnobReverse;
        }
        set
        {
            if (settingJointPoint == JointPoint.Knob)
                catchedLocalPositionKnob = value - transform.position;
            else
                catchedLocalPositionKnobReverse = value - transform.position;
        }
    }
#endif

    protected override void Initialize()
    {
        base.Initialize();

        //���� �ʱ�ȭ
        umbrellaOpen = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaOpen);
        umbrellaClosed = ResourceManager.GetResource(ResourceEnum.Mesh.Mesh_UmbrellaClosed);

        umbrellaMeshfilter = GetComponent<MeshFilter>();

        umbrellaMeshRenderer.material = ResourceManager.GetResource(umbrellaMaterial);

        physicsInteractionObjectCollider = umbrellaOpenCollider;

        umbrellaOpenCollider.enabled = true;
        umbrellaClosedCollider.enabled = false;
        umbrellaMeshfilter.mesh = umbrellaOpenCollider.sharedMesh;

        currentCondition = initialCondition;

        //�⺻ Joint ������ ������
        catchedLocalPosition = catchedLocalPositionKnob;

        switch(currentCondition)
        {
            case UmbrellaCondition.Open:
                OpenUmbrella();
                break;
            case UmbrellaCondition.Closed:
                CloseUmbrella();
                break;
            case UmbrellaCondition.Reverse:
                ReverseUmbrella();
                break;
            case UmbrellaCondition.Hook:
                HookUmbrella();
                break;
            default:
                currentCondition = UmbrellaCondition.Open;
                OpenUmbrella();
                break;
        }

        conditionFuncInteractionDictionary = new Dictionary<UmbrellaCondition, List<FuncInteractionData>>();

        //��� ��� �۾�

        GameManager.ObjectsFixedUpdate -= CustomFixedUpdate;
        GameManager.ObjectsFixedUpdate += CustomFixedUpdate;




        //��� �غ� �۾�
        for(UmbrellaCondition i = 0; i<UmbrellaCondition.Length; i++)
        {
            conditionFuncInteractionDictionary[i] = new List<FuncInteractionData>();
        }

        //������ ���� �� �� �� ���
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.Tab, "��� ��ġ��", UmbrellaModeChange, null, null));

        //������ ���� �� �� �� ���
        conditionFuncInteractionDictionary[UmbrellaCondition.Open]  .Add(new FuncInteractionData(KeyCode.Tab, "��� ����", null, null, null));

        //������ ���� �� �� �� ���
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.Tab, "���", null, null, null));

        //�ɷ� ���� �� �� �� ���
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.Tab, "���", null, null, null));

    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////// ���º�ȭ�� �ϳ��� ������� ���� ��!
    /// </summary>
    /// 

    //����� ����
    private void OpenUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Closed)
            return;

        SetUmbrellaMode(true);
        SetUmbrellaJointPoint(true);

        currentCondition = UmbrellaCondition.Open;
    }
    //����� �ݱ�
    private void CloseUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse && currentCondition != UmbrellaCondition.Open)
            return;

        SetUmbrellaMode(false);
        SetUmbrellaJointPoint(true);
        /////////////////////////////////////////�����̿� ���ߴ� �Լ��� ĳ���� �ʿ� �ʿ�� �� ��.
        FakeCenterPosition = holdingCharacter.transform.position + holdingCharacter.CatchingLocalPosition;

        currentCondition = UmbrellaCondition.Closed;
    }
    //����� ������
    private void ReverseUmbrella()
    {
        SetUmbrellaMode(false);
        SetUmbrellaJointPoint(false);

        currentCondition = UmbrellaCondition.Reverse;
    }
    //����� �ɱ�
    private void HookUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse)
            return;

        SetUmbrellaMode(false);
        SetUmbrellaJointPoint(false);

        currentCondition = UmbrellaCondition.Hook;
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
