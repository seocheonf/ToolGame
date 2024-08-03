using UnityEngine;

public struct FuncInteractionData
{
    public KeyCode keyCode;
    public string description;
    
    public OnFuncInteractionFunction OnFuncInteraction;
    public DurationFuncInteractionFunction DurationFuncInteraction;
    public OffFuncInteractionFunction OffFuncInteraction;

    /// <summary>
    /// [입력 대상 - 기능] 쌍으로 구성된 구조체 데이터 생성자
    /// </summary>
    /// <param name="keyCode">입력 대상</param>
    /// <param name="description">기능 설명</param>
    /// <param name="OnFunc">입력받는 순간의 기능</param>
    /// <param name="DurationFunc">입력받는 동안의 기능</param>
    /// <param name="OffFunc">입력이 종료됐을 때의 기능</param>
    public FuncInteractionData(KeyCode keyCode, string description, OnFuncInteractionFunction OnFunc, DurationFuncInteractionFunction DurationFunc, OffFuncInteractionFunction OffFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = OnFunc;
        DurationFuncInteraction = DurationFunc;
        OffFuncInteraction = OffFunc;
    }

    /// <summary>
    /// [입력 대상 - 입력받는 순간의 기능] 쌍으로 구성된 구조체 데이터 생성자
    /// </summary>
    /// <param name="keyCode">입력 대상</param>
    /// <param name="description">기능 설명</param>
    /// <param name="OnFunc">입력받는 순간의 기능</param>
    public FuncInteractionData(KeyCode keyCode, string description, OnFuncInteractionFunction OnFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = OnFunc;
        DurationFuncInteraction = null;
        OffFuncInteraction = null;
    }

    /// <summary>
    /// [입력 대상 - 입력받는 동안의 기능] 쌍으로 구성된 구조체 데이터 생성자
    /// </summary>
    /// <param name="keyCode">입력 대상</param>
    /// <param name="description">기능 설명</param>
    /// <param name="DurationFunc">입력받는 동안의 기능</param>
    public FuncInteractionData(KeyCode keyCode, string description, DurationFuncInteractionFunction DurationFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = null;
        DurationFuncInteraction = DurationFunc;
        OffFuncInteraction = null;
    }

    /// <summary>
    /// [입력 대상 - 입력이 종료됐을 때의 기능] 쌍으로 구성된 구조체 데이터 생성자
    /// </summary>
    /// <param name="keyCode">입력 대상</param>
    /// <param name="description">기능 설명</param>
    /// <param name="OffFunc">입력이 종료됐을 때의 기능</param>
    public FuncInteractionData(KeyCode keyCode, string description, OffFuncInteractionFunction OffFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = null;
        DurationFuncInteraction = null;
        OffFuncInteraction = OffFunc;
    }
}