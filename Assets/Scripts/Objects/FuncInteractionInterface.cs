using System.Collections.Generic;

/// <summary>
/// �ܺ� ���ͷ��ǿ� ���� �������̽���, �ڽ��� �����ؾ��� ��ɵ��� ��ȯ�Ѵ�.
/// </summary>
public interface IOuterFuncInteraction
{
    public List<FuncInteractionData> GetOuterFuncInteractionList();
}

public interface IOnOffFuncInteraction
{
    public List<FuncInteractionData> GetOnOffFuncInteractionList();
}