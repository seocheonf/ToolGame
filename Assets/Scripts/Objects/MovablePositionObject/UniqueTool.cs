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

    protected override void Initialize()
    {
        base.Initialize();

        holdingFuncInteractionList = new List<FuncInteractionData>();
        outerFuncInteractionList = new List<FuncInteractionData>();
        onoffFuncInteractionList = new List<FuncInteractionData>();

    }

    public virtual void PutTool()
    {

    }
    protected void CheckPutToolUpdate(float deltaTime)
    {

    }
    protected void PutToolEnd()
    {

    }
    public virtual void PickUpTool(Character source)
    {

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
}
