using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;

public delegate void FuncInteractionFunction();


public class ControllerManager : Manager
{

    private static float mouseScrollDelta;
    public static float MouseScrollDelta => mouseScrollDelta;

    private static Vector2 mouseMovement;
    public static Vector2 MouseMovement => mouseMovement;

    //키에 대응되는 기능은 반드시 하나만 보장되어야 함.
    private static Dictionary<OuterKeyCode, FuncInteractionData> inputFuncInteractionDictionary;

    private static Queue<FuncInteractionData> addFuncInteractionQueue;
    private static Queue<FuncInteractionData> removeFuncInteractionQueue;

    private static Dictionary<OuterKeyCode, KeyCode> keyCodeConnection;

    public override IEnumerator Initiate()
    {
        yield return base.Initiate();

        inputFuncInteractionDictionary = new Dictionary<OuterKeyCode, FuncInteractionData>();
        addFuncInteractionQueue = new Queue<FuncInteractionData>();
        removeFuncInteractionQueue = new Queue<FuncInteractionData>();



        #region OuterKeyCode - KeyCode 연결

        keyCodeConnection = new Dictionary<OuterKeyCode, KeyCode>();

        keyCodeConnection.Add(OuterKeyCode.Forward, KeyCode.W);
        keyCodeConnection.Add(OuterKeyCode.Backward, KeyCode.S);
        keyCodeConnection.Add(OuterKeyCode.Leftward, KeyCode.A);
        keyCodeConnection.Add(OuterKeyCode.Rightward, KeyCode.D);
        keyCodeConnection.Add(OuterKeyCode.Jump, KeyCode.Space);
        keyCodeConnection.Add(OuterKeyCode.Dash, KeyCode.LeftShift);
        keyCodeConnection.Add(OuterKeyCode.Sit, KeyCode.LeftControl);
        keyCodeConnection.Add(OuterKeyCode.Rush, KeyCode.R);
        keyCodeConnection.Add(OuterKeyCode.TakeTool, KeyCode.Mouse1);
        keyCodeConnection.Add(OuterKeyCode.OuterFunc, KeyCode.F);
        keyCodeConnection.Add(OuterKeyCode.Reverse, KeyCode.Tab);
        keyCodeConnection.Add(OuterKeyCode.Action, KeyCode.Mouse0);
        keyCodeConnection.Add(OuterKeyCode.Rot_Forward_Left, KeyCode.Q);
        keyCodeConnection.Add(OuterKeyCode.Rot_Backward_Right, KeyCode.E);

#if UNITY_EDITOR

        if (keyCodeConnection.Count < (int)OuterKeyCode.Length)
            Debug.LogError("OuterKeyCode와 KeyCode쌍이 전부 대응되지 않았어요!!!");

#endif



#endregion
    }

    public override void ManagerUpdate(float deltaTime)
    {
        base.ManagerUpdate(deltaTime);

        Cursor.lockState = CursorLockMode.Locked;

        mouseMovement.x = Input.GetAxis("Mouse X");
        mouseMovement.y = Input.GetAxis("Mouse Y");

        mouseScrollDelta = Input.mouseScrollDelta.y;

        foreach (var each in inputFuncInteractionDictionary)
        {
            if (Input.GetKeyDown(keyCodeConnection[each.Key]))
            {
                each.Value.OnFuncInteraction?.Invoke();
            }
            if(Input.GetKey(keyCodeConnection[each.Key]))
            {
                each.Value.DurationFuncInteraction?.Invoke();
            }
            if(Input.GetKeyUp(keyCodeConnection[each.Key]))
            {
                each.Value.OffFuncInteraction?.Invoke();
            }
        }

        while (removeFuncInteractionQueue.TryDequeue(out FuncInteractionData result))
        {
            RealRemoveInputFuncInteraction(result);
        }

        while (addFuncInteractionQueue.TryDequeue(out FuncInteractionData result))
        {
            RealAddInputFuncInteraction(result);
        }


    }

    /// <summary>
    /// 키 입력에 대응하는 기능을 등록하는 함수
    /// </summary>
    /// <param name="funcInteractionData">추가하고자 하는 [키:기능]쌍 정보</param>
    public static void AddInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        addFuncInteractionQueue.Enqueue(funcInteractionData);
    }

    /// <summary>
    /// 키 입력에 대응하는 기능을 실제로 등록하는 함수
    /// </summary>
    /// <param name="funcInteractionData">추가하고자 하는 [키:기능]쌍 정보</param>
    private static void RealAddInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if (!inputFuncInteractionDictionary.TryAdd(funcInteractionData.keyCode, funcInteractionData))
        {
            Debug.LogError("이미 할당된 키에 중복으로 할당되었어요!!!");
            inputFuncInteractionDictionary[funcInteractionData.keyCode] = funcInteractionData;
        }
    }

    /// <summary>
    /// 키 입력 처리를 제거하는 함수. [키:기능]쌍 정보를 받아 [키]에 해당하는 입력 처리를 제거한다.
    /// </summary>
    /// <param name="funcInteractionData">제거하고자 하는 [키:기능]쌍 정보</param>
    public static void RemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        removeFuncInteractionQueue.Enqueue(funcInteractionData);
    }

    /// <summary>
    /// 키 입력 처리를 실제로 제거하는 함수. [키:기능]쌍 정보를 받아 [키]에 해당하는 입력 처리를 제거한다.
    /// </summary>
    /// <param name="funcInteractionData">제거하고자 하는 [키:기능]쌍 정보</param>
    private static void RealRemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if (!inputFuncInteractionDictionary.Remove(funcInteractionData.keyCode))
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