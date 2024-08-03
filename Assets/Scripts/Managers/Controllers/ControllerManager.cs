using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnFuncInteractionFunction();
public delegate void DurationFuncInteractionFunction();
public delegate void OffFuncInteractionFunction();


public class ControllerManager : Manager
{

    private static Vector2 mouseMovement;
    public static Vector2 MouseMovement => mouseMovement;

    //키에 대응되는 기능은 반드시 하나만 보장되어야 함.
    private static Dictionary<KeyCode, FuncInteractionData> inputFuncInteractionDictionary;

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        inputFuncInteractionDictionary = new Dictionary<KeyCode, FuncInteractionData>();

    }

    public override void ManagerUpdate(float deltaTime)
    {
        base.ManagerUpdate(deltaTime);

        //Cursor.lockState = CursorLockMode.Locked;

        mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        foreach (var each in inputFuncInteractionDictionary)
        {
            if(Input.GetKeyDown(each.Key))
            {
                each.Value.OnFuncInteraction?.Invoke();
            }
            if(Input.GetKey(each.Key))
            {
                each.Value.DurationFuncInteraction?.Invoke();
            }
            if(Input.GetKeyUp(each.Key))
            {
                each.Value.OffFuncInteraction?.Invoke();
            }
        }

    }

    /// <summary>
    /// 키 입력에 대응하는 기능을 등록하는 함수
    /// </summary>
    /// <param name="funcInteractionData">추가하고자 하는 [키:기능]쌍 정보</param>
    public static void AddInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if(!inputFuncInteractionDictionary.TryAdd(funcInteractionData.keyCode, funcInteractionData))
        {
            inputFuncInteractionDictionary[funcInteractionData.keyCode] = funcInteractionData;
        }
    }
    /// <summary>
    /// 키 입력 처리를 제거하는 함수. [키:기능]쌍 정보를 받아 [키]에 해당하는 입력 처리를 제거한다.
    /// </summary>
    /// <param name="funcInteractionData">제거하고자 하는 [키:기능]쌍 정보</param>
    public static void RemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if(!inputFuncInteractionDictionary.Remove(funcInteractionData.keyCode))
        {
            Debug.LogError("이전에 등록된 [키-기능] 쌍이 없어요!");
        }
    }

    /// <summary>
    /// [키:기능]쌍 정보들에 대하여, 각각의 키 입력에 대응하는 기능을 등록하는 함수
    /// </summary>
    /// <param name="funcInteractionDataList">추가하고자 하는 [키:기능]쌍들 정보</param>
    public static void AddInputFuncInteraction(List<FuncInteractionData> funcInteractionDataList)
    {
        foreach (var each in funcInteractionDataList)
        {
            AddInputFuncInteraction(each);
        }
    }
    /// <summary>
    /// 키 입력 처리들을 제거하는 함수. [키:기능]쌍 정보들을 받아 각각의 [키]에 해당하는 입력 처리를 제거한다.
    /// </summary>
    /// <param name="funcInteractionDataList">제거하고자 하는 [키:기능]쌍들 정보</param>
    public static void RemoveInputFuncInteraction(List<FuncInteractionData> funcInteractionDataList)
    {
        foreach (var each in funcInteractionDataList)
        {
            RemoveInputFuncInteraction(each);
        }
    }


}