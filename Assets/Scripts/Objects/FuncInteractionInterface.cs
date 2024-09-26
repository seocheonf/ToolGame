using System.Collections.Generic;

/// <summary>
/// 외부 인터렉션에 대한 인터페이스로, 자신이 수행해야할 기능들을 반환한다.
/// </summary>
public interface IOuterFuncInteraction
{
    public List<FuncInteractionData> GetOuterFuncInteractionList();
}

public interface IOnOffFuncInteraction
{
    public void DoOn();

    public void DoOff();
}

public interface ITriggerFuncInteraction
{
    public void DoTrigger();
}