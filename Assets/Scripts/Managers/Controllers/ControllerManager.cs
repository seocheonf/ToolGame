using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void FuncInteractionFunction();

public class ControllerManager : Manager
{
    Dictionary<KeyCode, FuncInteractionData> inputFuncInteractionDictionary;

    public override void ManagerStart()
    {
        base.ManagerStart();
        inputFuncInteractionDictionary = new Dictionary<KeyCode, FuncInteractionData>();
    }


    public override void ManagerUpdate(float deltaTime)
    {
        base.ManagerUpdate(deltaTime);

        foreach(var each in inputFuncInteractionDictionary)
        {
            if(Input.GetKeyDown(each.Key))
            {
                each.Value.OnFuncInteraction?.Invoke();
            }
            if(Input.GetKey(each.Key))
            {
                each.Value.DurationFuncInteraction();
            }
            if(Input.GetKeyUp(each.Key))
            {
                each.Value.OffFuncInteraction();
            }
        }

    }

    public static void AddInputFuncInteraction(FuncInteractionData funcInteractionData)
    {

    }
    public static void RemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    {

    }

    public static void AddInputFuncInteraction(List<FuncInteractionData> funcInteractionDataList)
    {
        foreach (var each in funcInteractionDataList)
        {
            AddInputFuncInteraction(each);
        }
    }
    public static void RemoveInputFuncInteraction(List<FuncInteractionData> funcInteractionDataList)
    {
        foreach (var each in funcInteractionDataList)
        {
            RemoveInputFuncInteraction(each);
        }
    }


}