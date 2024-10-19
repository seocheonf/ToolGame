using SpecialInteraction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;

public class BigLeaf : UniqueTool
{
    /// <summary>
    /// �߰� �η� ����. 0�̻��� ��
    /// </summary>
    private const float bigLeafBuoyancyRatio = 2f;

    enum BigLeafCondition
    {
        Normal,
        Forward,
        Length
    }

    /// <summary>
    /// ���¿� ���� ���������� �ؾ��� ��
    /// </summary>
    FixedUpdateFunction ConditionFixedUpdate;

    private BigLeafCondition holdingCondition;
    private BigLeafCondition currentCondition;
    private BigLeafCondition CurrentCondition
    {
        get
        {
            return currentCondition;
        }
        set
        {
            currentCondition = value;
            holdingCondition = currentCondition;
            SetCondition(currentCondition);
        }
    }

    Dictionary<BigLeafCondition, List<FuncInteractionData>> conditionFuncInteractionDictionary;


    protected override void Initialize()
    {
        base.Initialize();

        //��Ȳ�� ���� Ű�Է� ��� ���� ��� �ߺ�
        conditionFuncInteractionDictionary = new Dictionary<BigLeafCondition, List<FuncInteractionData>>();
        for(BigLeafCondition i = 0; i < BigLeafCondition.Length; i++)
        {
            conditionFuncInteractionDictionary[i] = new List<FuncInteractionData>();
        }

        //Normal ������ �� �� �� ���
        //conditionFuncInteractionDictionary[BigLeafCondition.Normal].Add(new FuncInteractionData(,,,,));
        //Forward ������ �� �� �� ���
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.Action,"�ٶ� ����Ű�� On/Off",SetWindOnOff,null,null));
        //Up ������ �� �� �� ���
        //conditionFuncInteractionDictionary[BigLeafCondition.Up].Add(new FuncInteractionData(,,,,));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T1, "Test1", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T2, "Test2", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T3, "Test3", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T4, "Test4", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T5, "Test5", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T6, "Test6", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T7, "Test7", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T8, "Test8", null, null, null));
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(OuterKeyCode.T9, "Test9", null, null, null));



        CurrentCondition = BigLeafCondition.Normal;

    }

    protected override void MainFixedUpdate(float fixedDeltaTime)
    {
        ConditionFixedUpdate?.Invoke(fixedDeltaTime);




        base.MainFixedUpdate(fixedDeltaTime);
    }


    private void SetCondition(BigLeafCondition newCondition)
    {

        switch(currentCondition)
        {
            case BigLeafCondition.Normal:
                SetNormalCondition();
                break;
            case BigLeafCondition.Forward:
                SetForwardCondition();
                break;
            default://����
                Debug.LogError("�߸��� ���� ����");
                currentCondition = BigLeafCondition.Normal;
                SetNormalCondition();
                break;
        }

        //������׽�Ʈ//
        //if (holdingCharacter != null)
        //{
        //    ControllerManager.RemoveInputFuncInteraction(holdingFuncInteractionList);
        //    holdingFuncInteractionList = conditionFuncInteractionDictionary[CurrentCondition];
        //    ControllerManager.AddInputFuncInteraction(holdingFuncInteractionList);
        //} 

    }


    //������� ���� ���� ���������� �ؾ��� �ϵ��� ��ȭ��Ű�� ���ƾ� �ؿ�.

    private void SetNormalCondition()
    {
        //�Ϲ����� ���¶�� �ٶ��� ������ ����. ���� ��� ���� �Ѿ߸� ���� ���·� ���� ����!
        SetWind(false);


        ConditionFixedUpdate = null;
        ConditionFixedUpdate = Condition_Normal_FixedUpdate;

    }
    private void SetForwardCondition()
    {

        //forward�� �鸮�� �ʾ��� ���� �� ���� �ؼ��� �ȵ�
        if (holdingCharacter != null)
        {
            ConditionFixedUpdate = null;
            ConditionFixedUpdate = Condition_Forward_FixedUpdate;
        }
    }


    //������ ���� �Ϲ�
    private void Condition_Normal_FixedUpdate(float fixedDeltaTime)
    {
        if (GetDownSpeed() <= 0)
        {
            float dotValue = Vector3.Dot(Vector3.up, transform.forward);
            dotValue = dotValue > 0 ? dotValue : -dotValue;
            AccelDownForce(1 - dotValue * 0.05f);
        }
    }

    //������ ���� Forward
    private void Condition_Forward_FixedUpdate(float fixedDeltaTime)
    {
        ChangeBigLeafDirectionSight(fixedDeltaTime);
    }


    float animationValue;
    private void ChangeBigLeafDirectionSight(float fixedDeltaTime)
    {
        //ĳ������ �þ߸� �ٶ󺸰� ����
        Vector3 tempt = holdingCharacter.CurrentSightEulerAngle_Interaction;
        //Debug.Log($"{tempt} / {holdingCharacter.transform.eulerAngles}");

        //Debug.Log($"{tempt.y - holdingCharacter.transform.eulerAngles.y}");

        //if (tempt.y != holdingCharacter.transform.eulerAngles.y)
        //{
        //    Debug.LogError("warning");
        //}
        SetFakeCenterEulerAngle(tempt);
        //SetFakeCenterEulerAngle(holdingCharacter.transform.eulerAngles);

        //����� ������ �߰��� �����ϴ� ����. �⺻ ����� forward�� ����ϴ� �Ͱ�, �߰� ȸ���� ���� ���� �� ���̰� �ϱ� ���� �۾�
        float additionalValue = 15f;
        additionalValue *= MathF.Sin(animationValue);
        animationValue += fixedDeltaTime * 6f;
        float fanningValue = isFanning ? 82.8943329f + additionalValue : 82.8943329f;
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(fanningValue, 343.5914f, 26.6907959f)); // => ����� ������ ȸ������ ������ ã�� ��, �� ������ ������
        Vector3 modelPosition = transform.position;
        Quaternion modelRotation = transform.rotation;
        SetFakeCenterEulerAngle(tempt);
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(82.8943329f, 343.5914f, 26.6907959f)); // => ����� ������ ȸ������ ������ ã�� ��, �� ������ ������
        bigLeafModeling.transform.position = modelPosition;
        bigLeafModeling.transform.rotation = modelRotation;
    }


    public override void GetSpecialInteraction(WaterData source)
    {
        //����
        if (holdingCharacter == null)
        {
            AddForce(new ForceInfo(source.Direction * source.intensity, ForceType.DurationForce));
        }
        //�η�
        AddForce(new ForceInfo(Vector3.up * source.amount * bigLeafBuoyancyRatio, ForceType.UnityDuration));

        
    }




    [SerializeField]
    Wind bigLeafWind;
    bool isFanning;
    [SerializeField]
    GameObject bigLeafModeling;


    private void SetWindOnOff()
    {
        SetWind(!bigLeafWind.WindActive);
    }

    private void SetWind(bool isActive)
    {

        bigLeafWind.WindSetActive(isActive);
        isFanning = bigLeafWind.WindActive;
        animationValue = 0;

        //�ٶ��� �����ٸ� �𵨸��� ���� ���� �� ����
        if (!isFanning)
        {
            bigLeafModeling.transform.position = transform.position;
            bigLeafModeling.transform.rotation = transform.rotation;
        }
    }


    public override void PickUpTool(Character source)
    {
        base.PickUpTool(source);


        if(holdingCondition == BigLeafCondition.Normal)
            CurrentCondition = BigLeafCondition.Forward;
        else
            CurrentCondition = holdingCondition;

    }

    public override void PutTool()
    {
        CurrentCondition = BigLeafCondition.Normal;


        base.PutTool();
    }


    //������׽�Ʈ//
    public override List<FuncInteractionData> GetHoldingFuncInteractionDataList()
    {
        return conditionFuncInteractionDictionary[currentCondition];
    }
}
