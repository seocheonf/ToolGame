using UnityEngine;

/// <summary>
/// 입력과 기능에 대한 정보 저장 장소.
/// </summary>
public struct FuncInteractionData
{
    public KeyCode keyCode;
    public string description;
    
    public FuncInteractionFunction OnFuncInteraction;
    public FuncInteractionFunction DurationFuncInteraction;
    public FuncInteractionFunction OffFuncInteraction;
}