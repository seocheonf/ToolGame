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

    //잡힐 지점
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

    //인터페이스 구체화

    public List<FuncInteractionData> GetOuterFuncInteractionList()
    {
        return default;
    }

    public List<FuncInteractionData> GetOnOffFuncInteractionList()
    {
        return default;
    }






    /// <summary>
    /// 거짓 월드 중심 좌표. 이 값을 바꾸면, 이 거짓 월드 중심 좌표를 기준으로하는 좌표로 위치가 옮겨진다.
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
    /// 거짓 월드 중심 좌표를 기준으로 하는 캐릭터의 회전 각도. 값을 변경하면, 거짓 월드 중심 좌표를 기준으로 회전한다.
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
