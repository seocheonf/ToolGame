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

    //Ű�� �����Ǵ� ����� �ݵ�� �ϳ��� ����Ǿ�� ��.
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
    /// Ű �Է¿� �����ϴ� ����� ����ϴ� �Լ�
    /// </summary>
    /// <param name="funcInteractionData">�߰��ϰ��� �ϴ� [Ű:���]�� ����</param>
    public static void AddInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if(!inputFuncInteractionDictionary.TryAdd(funcInteractionData.keyCode, funcInteractionData))
        {
            inputFuncInteractionDictionary[funcInteractionData.keyCode] = funcInteractionData;
        }
    }
    /// <summary>
    /// Ű �Է� ó���� �����ϴ� �Լ�. [Ű:���]�� ������ �޾� [Ű]�� �ش��ϴ� �Է� ó���� �����Ѵ�.
    /// </summary>
    /// <param name="funcInteractionData">�����ϰ��� �ϴ� [Ű:���]�� ����</param>
    public static void RemoveInputFuncInteraction(FuncInteractionData funcInteractionData)
    {
        if(!inputFuncInteractionDictionary.Remove(funcInteractionData.keyCode))
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