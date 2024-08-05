using UnityEngine;

/// <summary>
/// �Է°� ��ɿ� ���� ���� ���� ���.
/// </summary>
public struct FuncInteractionData
{
    public KeyCode keyCode;
    public string description;
    
    public FuncInteractionFunction OnFuncInteraction;
    public FuncInteractionFunction DurationFuncInteraction;
    public FuncInteractionFunction OffFuncInteraction;

    /// <summary>
    /// [�Է� ��� - ���] ������ ������ ����ü ������ ������
    /// </summary>
    /// <param name="keyCode">�Է� ���</param>
    /// <param name="description">��� ����</param>
    /// <param name="OnFunc">�Է¹޴� ������ ���</param>
    /// <param name="DurationFunc">�Է¹޴� ������ ���</param>
    /// <param name="OffFunc">�Է��� ������� ���� ���</param>
    public FuncInteractionData(KeyCode keyCode, string description, FuncInteractionFunction OnFunc, FuncInteractionFunction DurationFunc, FuncInteractionFunction OffFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = OnFunc;
        DurationFuncInteraction = DurationFunc;
        OffFuncInteraction = OffFunc;
    }

    #region �����ε� ���Ž�

    /// <summary>
    /// [�Է� ��� - �Է¹޴� ������ ���] ������ ������ ����ü ������ ������
    /// </summary>
    /// <param name="keyCode">�Է� ���</param>
    /// <param name="description">��� ����</param>
    /// <param name="OnFunc">�Է¹޴� ������ ���</param>
    public FuncInteractionData(KeyCode keyCode, string description, OnFuncInteractionFunction OnFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = OnFunc;
        DurationFuncInteraction = null;
        OffFuncInteraction = null;
    }

    /// <summary>
    /// [�Է� ��� - �Է¹޴� ������ ���] ������ ������ ����ü ������ ������
    /// </summary>
    /// <param name="keyCode">�Է� ���</param>
    /// <param name="description">��� ����</param>
    /// <param name="DurationFunc">�Է¹޴� ������ ���</param>
    public FuncInteractionData(KeyCode keyCode, string description, DurationFuncInteractionFunction DurationFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = null;
        DurationFuncInteraction = DurationFunc;
        OffFuncInteraction = null;
    }

    /// <summary>
    /// [�Է� ��� - �Է��� ������� ���� ���] ������ ������ ����ü ������ ������
    /// </summary>
    /// <param name="keyCode">�Է� ���</param>
    /// <param name="description">��� ����</param>
    /// <param name="OffFunc">�Է��� ������� ���� ���</param>
    public FuncInteractionData(KeyCode keyCode, string description, OffFuncInteractionFunction OffFunc)
    {
        this.keyCode = keyCode;
        this.description = description;

        OnFuncInteraction = null;
        DurationFuncInteraction = null;
        OffFuncInteraction = OffFunc;
    }

    #endregion
}