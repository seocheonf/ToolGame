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

    //Ű�� �����Ǵ� ����� �ݵ�� �ϳ��� ����Ǿ�� ��.
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



        #region OuterKeyCode - KeyCode ����

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
            Debug.LogError("OuterKeyCode�� KeyCode���� ���� �������� �ʾҾ��!!!");

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
    /// Ű �Է¿� �����ϴ� ����� ����ϴ� �Լ�
    /// </summary>
    /// <param name="funcInteractionData">�߰��ϰ��� �ϴ� [Ű:���]�� ����</param>
    public static void AddInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        addFuncInteractionQueue.Enqueue(funcInteractionData);
    }

    /// <summary>
    /// Ű �Է¿� �����ϴ� ����� ������ ����ϴ� �Լ�
    /// </summary>
    /// <param name="funcInteractionData">�߰��ϰ��� �ϴ� [Ű:���]�� ����</param>
    private static void RealAddInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if (!inputFuncInteractionDictionary.TryAdd(funcInteractionData.keyCode, funcInteractionData))
        {
            Debug.LogError("�̹� �Ҵ�� Ű�� �ߺ����� �Ҵ�Ǿ����!!!");
            inputFuncInteractionDictionary[funcInteractionData.keyCode] = funcInteractionData;
        }
    }

    /// <summary>
    /// Ű �Է� ó���� �����ϴ� �Լ�. [Ű:���]�� ������ �޾� [Ű]�� �ش��ϴ� �Է� ó���� �����Ѵ�.
    /// </summary>
    /// <param name="funcInteractionData">�����ϰ��� �ϴ� [Ű:���]�� ����</param>
    public static void RemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        removeFuncInteractionQueue.Enqueue(funcInteractionData);
    }

    /// <summary>
    /// Ű �Է� ó���� ������ �����ϴ� �Լ�. [Ű:���]�� ������ �޾� [Ű]�� �ش��ϴ� �Է� ó���� �����Ѵ�.
    /// </summary>
    /// <param name="funcInteractionData">�����ϰ��� �ϴ� [Ű:���]�� ����</param>
    private static void RealRemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if (!inputFuncInteractionDictionary.Remove(funcInteractionData.keyCode))
        {
            Debug.LogError("������ ��ϵ� [Ű-���] ���� �����!");
        }
    }

    /// <summary>
    /// [Ű:���]�� �����鿡 ���Ͽ�, ������ Ű �Է¿� �����ϴ� ����� ����ϴ� �Լ�
    /// </summary>
    /// <param name="funcInteractionDataList">�߰��ϰ��� �ϴ� [Ű:���]�ֵ� ����</param>
    public static void AddInputFuncInteraction(List<FuncInteractionData> funcInteractionDataList)
    {
        foreach (var each in funcInteractionDataList)
        {
            AddInputFuncInteraction(each);
        }
    }
    /// <summary>
    /// Ű �Է� ó������ �����ϴ� �Լ�. [Ű:���]�� �������� �޾� ������ [Ű]�� �ش��ϴ� �Է� ó���� �����Ѵ�.
    /// </summary>
    /// <param name="funcInteractionDataList">�����ϰ��� �ϴ� [Ű:���]�ֵ� ����</param>
    public static void RemoveInputFuncInteraction(List<FuncInteractionData> funcInteractionDataList)
    {
        foreach (var each in funcInteractionDataList)
        {
            RemoveInputFuncInteraction(each);
        }
    }


}