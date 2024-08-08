using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UniqueTool : MovablePositionObject, IOuterFuncInteraction, IOnOffFuncInteraction
{
    protected List<FuncInteractionData> holdingFuncInteractionList;

    protected List<FuncInteractionData> outerFuncInteractionList;
    protected List<FuncInteractionData> onoffFuncInteractionList;


    protected float angle;
    protected Character holdingCharacter;

    //���� ����
    [SerializeField]
    private Vector3 catchedLocalPosition;
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

#endif

    protected override void Initialize()
    {
        base.Initialize();

        holdingFuncInteractionList = new List<FuncInteractionData>();
        outerFuncInteractionList = new List<FuncInteractionData>();
        onoffFuncInteractionList = new List<FuncInteractionData>();

    }

    public virtual void PutTool()
    {
        transform.parent = null;
        holdingCharacter = null;
        physicsInteractionObjectRigidbody.isKinematic = false;
        physicsInteractionObjectCollider.isTrigger = false;
        ControllerManager.RemoveInputFuncInteraction(holdingFuncInteractionList);
        
    }
    protected void CheckPutToolUpdate(float deltaTime)
    {

    }
    protected void PutToolEnd()
    {
        
    }
    public virtual void PickUpTool(Character source)
    {
        transform.parent = source.transform;
        holdingCharacter = source;
        physicsInteractionObjectRigidbody.isKinematic = true;
        physicsInteractionObjectCollider.isTrigger = true;
        ControllerManager.AddInputFuncInteraction(holdingFuncInteractionList);
    }

    public List<FuncInteractionData> GetHoldingFuncInteractionList()
    {
        return default;
    }

    //�������̽� ��üȭ

    public List<FuncInteractionData> GetOuterFuncInteractionList()
    {
        return default;
    }

    public List<FuncInteractionData> GetOnOffFuncInteractionList()
    {
        return default;
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
    



}
