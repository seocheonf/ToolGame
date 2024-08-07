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

    //잡을 지점
    [SerializeField]
    protected Vector3 catchedLocalPosition;

#if UNITY_EDITOR

    public virtual Vector3 CatchedLocalPosition
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






    //외부가 바라보는 나의 중심 월드 좌표
    protected Vector3 fakeCenterPosition;
    /// <summary>
    /// 거짓 월드 중심 좌표. 이 값을 바꾸면, 이 거짓 월드 중심 좌표를 기준으로하는 좌표로 위치가 옮겨진다.
    /// </summary>
    public Vector3 FakeCenterPosition
    {
        get
        {
            return transform.position + catchedLocalPosition;
        }
        set
        {
            transform.position = value - catchedLocalPosition;
        }
    }
    



}
