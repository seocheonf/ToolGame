using UnityEngine;

public struct FuncInteractionData
{
    public KeyCode keyCode;
    public string description;
    
    public FuncInteractionFunction OnFuncInteraction;
    public FuncInteractionFunction DurationFuncInteraction;
    public FuncInteractionFunction OffFuncInteraction;
}