using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;

public abstract class UniqueTool : MovablePositionObject
{
    protected List<FuncInteractionData> holdingFuncInteractionList;

    protected float angle;
    protected Character holdingCharacter;

    //���� ����
    [SerializeField]
    protected Vector3 catchedLocalPosition;
    protected Vector3 CatchedLocalPosition
    {
        get
        {
            return transform.rotation * catchedLocalPosition;
        }
    }


#if UNITY_EDITOR

    public virtual Vector3 CatchedLocalPositionEdit
    {
        get
        {
            return transform.position + catchedLocalPosition;
        }
        set
        {
            catchedLocalPosition = value - transform.position;
        }
    }

    protected virtual void Reset()
    {
        GameObject child = new GameObject("CastTarget");
        child.layer = LayerMask.NameToLayer("Cast_UniqueTool");
        child.transform.parent = transform;
        child.transform.localPosition = Vector3.zero;
        child.AddComponent<SphereCollider>().isTrigger = true;
    }

#endif


    protected override void Initialize()
    {
        base.Initialize();
        
        holdingFuncInteractionList = new List<FuncInteractionData>();
    }

    public override void GetSpecialInteraction(WaterData source)
    {
        //�鸮�� �ʾ��� ���� ���� ȿ���� �޾ƶ�.
        if (holdingCharacter == null)
        {
            //����
            AddForce(new ForceInfo(source.Direction * source.intensity, ForceType.DurationForce));
            //�η�
            AddForce(new ForceInfo(Vector3.up * source.amount, ForceType.UnityDuration));
        }
    }


    public virtual void PutTool()
    {
        if(holdingCharacter == null)
        {
            Debug.LogError("������ ������� ������ ���� ������!!");
            return;
        }

        //Ÿ�� ������ٵ� �ʱ�ȭ
        currentRigidbody = initialRigidbody;

        //���� ���� ���� �ʱ�ȭ
        physicsInteractionObjectCollider.isTrigger = false;
        currentRigidbody.isKinematic = false;
        currentRigidbody.velocity = holdingCharacter.GetVelocity();
        //������׽�Ʈ// ControllerManager.RemoveInputFuncInteraction(holdingFuncInteractionList);
        transform.parent = null;
        holdingCharacter = null;

    }
    protected void CheckPutToolUpdate(float deltaTime)
    {

    }
    protected void PutToolEnd()
    {
        
    }
    public virtual void PickUpTool(Character source)
    {
        if(holdingCharacter != null)
        {
            Debug.LogError("������ �̹� ����ִµ� ����� ���� ������!!");
            return;
        }

        //���� ���� ���� ����
        transform.parent = source.transform;
        holdingCharacter = source;
        currentRigidbody.isKinematic = true;
        physicsInteractionObjectCollider.isTrigger = true;
        FakeCenterPosition = holdingCharacter.GetCatchingPosition();
        //������׽�Ʈ// ControllerManager.AddInputFuncInteraction(holdingFuncInteractionList);

        //Ÿ�� ������ٵ� ����
        currentRigidbody = holdingCharacter.CurrentRigidbody;

        Debug.Log($"{transform.eulerAngles} ���� �� ");
    }


    public List<FuncInteractionData> GetHoldingFuncInteractionList()
    {
        return holdingFuncInteractionList;
    }




    /// <summary>
    /// ���� ���� �߽� ��ǥ. �� ���� �ٲٸ�, �� ���� ���� �߽� ��ǥ�� ���������ϴ� ��ǥ�� ��ġ�� �Ű�����.
    /// </summary>
    public Vector3 FakeCenterPosition
    {
        get
        {
            return transform.position + CatchedLocalPosition;
        }
        set
        {
            transform.position = value - CatchedLocalPosition;
        }
    }
    
    /// <summary>
    /// ���� ���� �߽� ��ǥ�� �������� �ϴ� ĳ������ ȸ�� ����. ���� �����ϸ�, ���� ���� �߽� ��ǥ�� �������� ȸ���Ѵ�.
    /// </summary>
    public Quaternion FakeCenterRotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            Vector3 beforePosition = FakeCenterPosition;
            transform.rotation = value;
            FakeCenterPosition = beforePosition;
        }
    }

    /// <summary>
    /// ���� ���� �߽� ��ǥ�� �������� �ϴ� ĳ������ Up ���� ����. ���� ���� �߽� ��ǥ�� �������� Ư�� ������ �ٶ󺻴�.
    /// </summary>
    public Vector3 FakeCenterUp
    {
        get
        {
            return transform.up;
        }
        set
        {
            Vector3 beforePosition = FakeCenterPosition;
            transform.up = value;
            FakeCenterPosition = beforePosition;
        }
    }

    /// <summary>
    /// ���� ���� �߽� ��ǥ�� �������� eulerAnlge�� �����Ѵ�.
    /// </summary>
    /// <param name="eulerAngle">���� eulerAngle</param>
    public void SetFakeCenterEulerAngle(Vector3 eulerAngle)
    {
        Vector3 tempt = FakeCenterPosition;
        transform.eulerAngles = eulerAngle;
        FakeCenterPosition = tempt;
    }

    /// <summary>
    /// ���� ���� �߽� ��ǥ�� ��������, ���� ȸ�� ���¿��� �־��� ���ʹϾ� ȸ����ŭ�� �߰��Ѵ�.
    /// </summary>
    /// <param name="quaternionRot">���ʹϾ� ȸ�� ��</param>
    public void SetFakeCenterQuaternionProductRotation(Quaternion quaternionRot)
    {
        Vector3 tempt = FakeCenterPosition;
        transform.rotation *= quaternionRot;
        FakeCenterPosition = tempt;
    }

    public delegate void ToolTrasnformTask();
    /// <summary>
    /// ���� ���� �߽� ��ǥ�� �������� transform ��ȯ�� �ְ� ���� ��, transform ��ȯ ���� �Ű������� �޾�, ���� ���� ��ǥ�� �������� �ϴ� �۾��� �����Ѵ�.
    /// </summary>
    /// <param name="TransformTask">���� ���� �߽� ��ǥ�� �������� �����ϰ��� �ϴ� transform ��ȯ</param>
    public void SetFakeCenterFreeTransform(ToolTrasnformTask TransformTask)
    {
        Vector3 tempt = FakeCenterPosition;
        TransformTask();
        FakeCenterPosition = tempt;
    }


    protected override void RegisterFuncInInitialize()
    {
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
        GameManager.ObjectsFixedUpdate += MainFixedUpdate;
    }
    protected override void RemoveFuncInDestroy()
    {
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
    }



    //������׽�Ʈ//
    public virtual List<FuncInteractionData> GetHoldingFuncInteractionDataList()
    {
        return holdingFuncInteractionList;
    }
}
