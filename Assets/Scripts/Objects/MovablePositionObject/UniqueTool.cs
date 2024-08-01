using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UniqueTool : MovablePositionObject, IOuterFuncInteraction, IOnOffFuncInteraction
{
    private List<FuncInteractionData> holdingFuncInteractionList;

    private List<FuncInteractionData> outerFuncInteractionList;
    private List<FuncInteractionData> onoffFuncInteractionList;


    private float angle;
    private Character holdingCharacter;
    
    public virtual void PutTool()
    {

    }
    private void CheckPutToolUpdate(float deltaTime)
    {

    }
    private void PutToolEnd()
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
