using SpecialInteraction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // ����� ��ħ�� ���� ����. true�� ��ħ, false�� ����.
    private bool umbrellaMode;


    //������ ��ġ
    [SerializeField]
    protected Vector3 catchedLocalPositionKnob;
    //�Ǵٸ� ��ġ
    [SerializeField]
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

        //�⺻ Joint ������ ������
        catchedLocalPosition = catchedLocalPositionKnob;

        ChangeInitialCondition(initialCondition);

        conditionFuncInteractionDictionary = new Dictionary<UmbrellaCondition, List<FuncInteractionData>>();

        //��� ��� �۾�
        GameManager.ObjectsFixedUpdate -= CustomUpdate;
        GameManager.ObjectsFixedUpdate += CustomUpdate;


        //��� �غ� �۾�
        for(UmbrellaCondition i = 0; i<UmbrellaCondition.Length; i++)
        {
            conditionFuncInteractionDictionary[i] = new List<FuncInteractionData>();
        }

        //������ ���� �� �� �� ���
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.Q, "��� ��ġ��", TryOpenUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.Tab, "��� ������", TryReverseUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Closed].Add(new FuncInteractionData(KeyCode.F, "���� ��ȯ", SwitchUmbrellaDirectionFixed, null, null));

        //������ ���� �� �� �� ���
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.Q, "��� ����", TryCloseUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode.F, "���� ��ȯ", SwitchUmbrellaDirectionFixed, null, null));

        //������ ���� �� �� �� ���
        conditionFuncInteractionDictionary[UmbrellaCondition.Reverse].Add(new FuncInteractionData(KeyCode.Tab, "��� ������", TryCloseUmbrella, null, null));
        conditionFuncInteractionDictionary[UmbrellaCondition.Reverse].Add(new FuncInteractionData(KeyCode.CapsLock, "���� �ɱ�", TryHookOnUmbrella, null, null));

        //�ɷ� ���� �� �� �� ���
        //conditionFuncInteractionDictionary[UmbrellaCondition.Open].Add(new FuncInteractionData(KeyCode., "���", null, null, null));

    }


    //����� ����
    private void TryOpenUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Closed)
            return;
        OpenUmbrella();
    }
    private void OpenUmbrella()
    {
        // ����� ��ġ��
        SetUmbrellaMode(true);
        // ��� ��� ��ġ�� �ٲٰ�
        SetUmbrellaJointPoint(true);

        // �÷��̾ �ִٸ� ��� ��� ��ġ�� �÷��̾� ��� ��ġ�� ��ġ ��Ű��
        if(holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // ����� Angle������ �������� ����
        SetUmbrellaDirection(UmbrellaDirection.Fixed);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Open, conditionFuncInteractionDictionary);
    }

    //����� �ݱ�
    private void TryCloseUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse && currentCondition != UmbrellaCondition.Open)
            return;
        CloseUmbrella();
    }
    private void CloseUmbrella()
    {
        // ����� ����
        SetUmbrellaMode(false);
        // ��� ��� ��ġ�� �ٲٰ�
        SetUmbrellaJointPoint(true);

        // �÷��̾ �ִٸ� ��� ��� ��ġ�� �÷��̾� ��� ��ġ�� ��ġ ��Ű��
        if (holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // ����� Angle������ �������� ����
        SetUmbrellaDirection(UmbrellaDirection.Fixed);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Closed, conditionFuncInteractionDictionary);
    }

    //����� ������
    private void TryReverseUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Closed && currentCondition != UmbrellaCondition.Hook)
            return;
        ReverseUmbrella();
    }
    private void ReverseUmbrella()
    {
        // ����� ����
        SetUmbrellaMode(false);
        // ��� ��� ��ġ�� �ٲٰ�
        SetUmbrellaJointPoint(false);

        // �÷��̾ �ִٸ� ��� ��� ��ġ�� �÷��̾� ��� ��ġ�� ��ġ ��Ű��
        if (holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // ����� Angle������ �ü����� ����
        SetUmbrellaDirection(UmbrellaDirection.Sight);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Reverse, conditionFuncInteractionDictionary);
    }

    //����� �ɱ�
    private void TryHookOnUmbrella()
    {
        if (currentCondition != UmbrellaCondition.Reverse)
            return;

        HookOnUmbrella();
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (holdingCharacter == null)
            return;

        //holdingCharacter.CurrentSightEulerAngle <= �̳��� �þ߰��� ��Ÿ���� ���Ϸ� ��
        Vector3 sightForward = Quaternion.Euler(holdingCharacter.CurrentSightEulerAngle) * Vector3.forward;

        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(FakeCenterPosition, FakeCenterPosition + sightForward * 5f);


            Collider[] hithit = Physics.OverlapBox(FakeCenterPosition + sightForward * 5f, Vector3.one * 2, holdingCharacter.CurrentSightQuaternionAngle);
            if(hithit.Length == 0)
            {
                return;
            }

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(FakeCenterPosition + sightForward * 5f, Quaternion.Euler(holdingCharacter.CurrentSightEulerAngle), Vector3.one);
            Gizmos.matrix = rotationMatrix;

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 4);

        }

    }

    private void HookOnUmbrella()
    {
        //�� �۾��� ��� �ִ� ĳ���Ͱ� ���� �� ����Ǳ� �ϳ�, Ȥ�ø𸣴� nullüũ
        if (holdingCharacter == null)
            return;
        
        Vector3 sightForward = holdingCharacter.CurrentSightForward;
        Collider[] hithit = Physics.OverlapBox(FakeCenterPosition + sightForward * 5f, Vector3.one * 2, holdingCharacter.CurrentSightQuaternionAngle);

        //���� ����� �� ĳĪ
        Vector3 dir = Vector3.one * float.MaxValue;
        UmbrellaHookTarget resultTarget = null;
        foreach(Collider each in hithit)
        {
            Debug.Log(each.name);
            Debug.Log("Ca");
            if(each.TryGetComponent(out UmbrellaHookTarget result))
            {
                Debug.Log("Catch");
                Vector3 dirSub = FakeCenterPosition - each.transform.position;
                float magSub = dirSub.magnitude;
                if (magSub < dir.magnitude)
                {
                    dir = dirSub;
                    resultTarget = result;
                }
            }
        }
        if (resultTarget == null)
        {
            Debug.Log("empty");
            return;
        }

        Debug.Log("name : " + resultTarget.name);
        return;



        // ����� ����
        SetUmbrellaMode(false);
        // ��� ��� ��ġ�� �ٲٰ�
        SetUmbrellaJointPoint(false);

        // �÷��̾ �ִٸ� ��� ��� ��ġ�� �÷��̾� ��� ��ġ�� ��ġ ��Ű��
        if (holdingCharacter != null)
            FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        // ����� Angle������ ������ ����
        SetUmbrellaDirection(UmbrellaDirection.Free);

        ChangeCondition(ref currentCondition, UmbrellaCondition.Hook, conditionFuncInteractionDictionary);
    }

    //����� ���⼺ ����
    private void SetUmbrellaDirection(UmbrellaDirection dir)
    {
        currentStandardaAngle = dir;
        if(currentStandardaAngle == UmbrellaDirection.Fixed)
        {
            ChangeUmbrellaDirectionFixed(currentFixedAngle);
        }
    }

    enum UmbrellaDirectionFixed
    {
        Forward,
        Up,
        Backward,
        Down,
        Length
    }

    private UmbrellaDirectionFixed currentFixedAngle;

    private void SwitchUmbrellaDirectionFixed()
    {
        if (currentFixedAngle == (UmbrellaDirectionFixed.Length - 1))
            currentFixedAngle = 0;
        else
            currentFixedAngle += 1;

        ChangeUmbrellaDirectionFixed(currentFixedAngle);
    }

    private void ChangeUmbrellaDirectionFixed(UmbrellaDirectionFixed dir)
    {
        currentFixedAngle = dir;
        if (holdingCharacter != null)
        {
            switch (currentFixedAngle)
            {
                case UmbrellaDirectionFixed.Forward:
                    //FakeCenterRotation = Quaternion.LookRotation(-holdingCharacter.transform.up);
                    //FakeCenterForward = -holdingCharacter.transform.up;
                    //FakeCenterRotation = Quaternion.Euler(90,0,0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(holdingCharacter.transform.forward);
                    //beforePosition = FakeCenterPosition;
                    //transform.up = holdingCharacter.transform.forward;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = holdingCharacter.transform.forward;
                    break;
                case UmbrellaDirectionFixed.Backward:
                    //FakeCenterRotation = Quaternion.LookRotation(-holdingCharacter.transform.up);
                    //FakeCenterForward = holdingCharacter.transform.up;
                    //FakeCenterRotation = Quaternion.Euler(-90, 0, 0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(-holdingCharacter.transform.forward);
                    //beforePosition = FakeCenterPosition;
                    //transform.up = -holdingCharacter.transform.forward;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = -holdingCharacter.transform.forward;
                    break;
                case UmbrellaDirectionFixed.Up:
                    //FakeCenterRotation = Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterForward = holdingCharacter.transform.forward;
                    //FakeCenterRotation = Quaternion.Euler(0, 0, 0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(holdingCharacter.transform.up);
                    //beforePosition = FakeCenterPosition;
                    //transform.up = holdingCharacter.transform.up;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = holdingCharacter.transform.up;
                    break;
                case UmbrellaDirectionFixed.Down:
                    //FakeCenterRotation = Quaternion.LookRotation(-holdingCharacter.transform.forward);
                    //FakeCenterForward = -holdingCharacter.transform.forward;
                    //FakeCenterRotation = Quaternion.Euler(180, 0, 0) * Quaternion.LookRotation(holdingCharacter.transform.forward);
                    //FakeCenterRotation = Quaternion.Euler(-holdingCharacter.transform.up);
                    //beforePosition = FakeCenter   Position;
                    //transform.up = -holdingCharacter.transform.up;
                    //FakeCenterPosition = beforePosition;
                    FakeCenterUp = -holdingCharacter.transform.up;
                    break;
            }
        }
    }

    private void ChangeInitialCondition(UmbrellaCondition newCondition)
    {
        currentCondition = newCondition;
        
        switch (currentCondition)
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
                //HookOnUmbrella();
                break;
            default:
                currentCondition = UmbrellaCondition.Open;
                OpenUmbrella();
                break;
        }
    }

    /// <summary>
    /// ����� ��ħ�� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="mode">true�� ��ħ, false�� ����</param>
    private void SetUmbrellaMode(bool mode)
    {
        umbrellaMode = mode;
        //��������
        if (umbrellaMode)
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
    /// ����� ��� �ִ� ���ο� ���� collider isTrigger on/off ����.\n
    /// ���� �ٸ� ������ ���� nullüũ �����ؼ� ������ ��.
    /// </summary>
    private void ChangeUmbrellaStatus()
    {
        //��� �ֳĿ� ���� �ٲ�
        if(holdingCharacter != null)
        {
            umbrellaOpenCollider.isTrigger = true;
            umbrellaClosedCollider.isTrigger = true;
        }
        else
        {
            umbrellaOpenCollider.isTrigger = false;
            umbrellaClosedCollider.isTrigger = false;
        }
    }

    /// <summary>
    /// ����� ���� ������ �����ϴ� �Լ�
    /// </summary>
    /// <param name="jointPoint">true�� ������, false�� �Ǵٸ�</param>
    private void SetUmbrellaJointPoint(bool jointPoint)
    {
        if (jointPoint)
        {
            catchedLocalPosition = catchedLocalPositionKnob;
        }
        else
        {
            catchedLocalPosition = catchedLocalPositionKnobReverse;
        }
    }

    private void ChangeCondition<T>(ref T currentCondition, T newCondition, Dictionary<T, List<FuncInteractionData>> conditionFunc) where T : System.Enum
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

    private void ChangeUmbrellaDirectionSight()
    {
        //ĳ������ �þ߸� �ٶ󺸰� ����
        SetFakeCenterEulerAngle(holdingCharacter.CurrentSightEulerAngle);
        //����� ������ �߰��� �����ϴ� ����. �⺻ ����� forward�� ����ϴ� �Ͱ�, �߰� ȸ���� ���� ���� �� ���̰� �ϱ� ���� �۾�
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(-120, 22.5f, 0));

        /*
        Vector3 tempt = FakeCenterPosition;
        transform.eulerAngles = holdingCharacter.CurrentSightEulerAngle;
        transform.rotation *= Quaternion.Euler(-90, 0, 0);
        FakeCenterPosition = tempt;
        */
    }

    private void ChangeUmbrellaDirectionFixedUpdate(float fixedDeltaTime)
    {
        if(holdingCharacter != null)
        {
            switch(currentStandardaAngle)
            {
                case UmbrellaDirection.Fixed:
                    //ChangeUmbrellaDirectionFixed(currentFixedAngle);
                    break;
                case UmbrellaDirection.Sight:
                    ChangeUmbrellaDirectionSight();
                    break;
            }
        }
    }




    //--------------------


    public override void GetSpecialInteraction(WindData source)
    {

        //�� ���� �ʿ�
      

        //�Ϲ����� �� �ۿ�
        base.GetSpecialInteraction(source);

        //Ư���ۿ� - ������ ���� ����
        if (umbrellaMode)
        {
            float dotValue = Vector3.Dot(transform.up, source.Direction);
            AddForce(new ForceInfo(transform.up * dotValue * 5f * source.intensity, ForceType.VelocityForce));
        }

    }

    private void CustomUpdate(float deltaTime)
    { 
        //ChangeUmbrellaDirectionUpdate(deltaTime);

    }

    protected override void MainFixedUpdate(float fixedDeltaTime)
    {
        ChangeUmbrellaDirectionFixedUpdate(fixedDeltaTime);
        if(GetDownSpeed() <= 0 && umbrellaMode)
        {
            float dotValue = Vector3.Dot(Vector3.up, transform.up);
            AccelDownForce(1 - dotValue * 0.05f);
        }
        base.MainFixedUpdate(fixedDeltaTime);
    }


    /* Legacy
    private void CustomFixedUpdate(float fixedDeltaTime)
    {
        ChangeUmbrellaDirectionFixedUpdate(fixedDeltaTime);
        ApplyForce();
    }

    private void ApplyForce()
    {
        if(holdingCharacter == null)
        {
            //�ϰ� ���� ��, �ϰ� �ӵ� ����
            if (currentRigidbody.velocity.y <= 0 && umbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                Vector3 downVelocity = currentRigidbody.velocity;
                downVelocity.y *= (1 - dotValue * 0.05f);
                currentRigidbody.velocity = downVelocity;
            }
        }
        else
        {
            if (holdingCharacter.GetDownSpeed() <= 0 && umbrellaMode)
            {
                float dotValue = Vector3.Dot(Vector3.up, transform.up);
                holdingCharacter.AccelDownForce((1 - dotValue * 0.05f));
            }
        }

        while (receivedForceQueue.TryDequeue(out ForceInfo result))
        {
            AddForce(result);
        }
    }
    */

    public override void PutTool()
    {
        //Ÿ�� ������ٵ� �ʱ�ȭ
        currentRigidbody = initialRigidbody;

        //���� ���� ���� �ʱ�ȭ
        physicsInteractionObjectCollider.isTrigger = false;
        currentRigidbody.isKinematic = false;
        currentRigidbody.velocity = holdingCharacter.GetVelocity();
        ControllerManager.RemoveInputFuncInteraction(conditionFuncInteractionDictionary[currentCondition]);
        transform.parent = null;
        holdingCharacter = null;
        ChangeUmbrellaStatus();
    }

    public override void PickUpTool(Character source)
    {
        //���� ���� ���� ����
        transform.parent = source.transform;
        holdingCharacter = source;
        currentRigidbody.isKinematic = true;
        physicsInteractionObjectCollider.isTrigger = true;
        ChangeUmbrellaStatus();
        FakeCenterPosition = holdingCharacter.GetCatchingPosition();

        SetUmbrellaDirection(currentStandardaAngle);

        ControllerManager.AddInputFuncInteraction(conditionFuncInteractionDictionary[currentCondition]);

        //Ÿ�� ������ٵ� ����
        currentRigidbody = holdingCharacter.CurrentRigidbody;
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        //��� ���� �۾�

        GameManager.ObjectsUpdate -= CustomUpdate;
        

    }
}
