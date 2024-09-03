using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigLeaf : UniqueTool
{
    
    enum BigLeafCondition
    {
        Normal,
        Forward,
        Up,
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
        //conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(,,,,));
        //Up ������ �� �� �� ���
        //conditionFuncInteractionDictionary[BigLeafCondition.Up].Add(new FuncInteractionData(,,,,));



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
            case BigLeafCondition.Up:
                SetUpCondition();
                break;
            default://����
                Debug.LogError("�߸��� ���� ����");
                currentCondition = BigLeafCondition.Normal;
                SetNormalCondition();
                break;
        }

        if(holdingCharacter != null)
        {
            ControllerManager.RemoveInputFuncInteraction(holdingFuncInteractionList);
            holdingFuncInteractionList = conditionFuncInteractionDictionary[CurrentCondition];
            ControllerManager.AddInputFuncInteraction(holdingFuncInteractionList);
        } 

    }


    //������� ���� ���� ���������� �ؾ��� �ϵ��� ��ȭ��Ű�� ���ƾ� �ؿ�.

    private void SetNormalCondition()
    {



        if (holdingCharacter != null)
        {
            ConditionFixedUpdate = null;
            ConditionFixedUpdate = Condition_Normal_FixedUpdate;
        }
    }
    private void SetForwardCondition()
    {



        if (holdingCharacter != null)
        {
            ConditionFixedUpdate = null;
            ConditionFixedUpdate = Condition_Forward_FixedUpdate;
        }
    }
    private void SetUpCondition()
    {



        if (holdingCharacter != null)
        {
            ConditionFixedUpdate = null;
            ConditionFixedUpdate = Condition_Up_FixedUpdate;
        }
    }


    //������ ���� �Ϲ�
    private void Condition_Normal_FixedUpdate(float fixedDeltaTime)
    {

    }
    //������ ���� Forward
    private void Condition_Forward_FixedUpdate(float fixedDeltaTime)
    {
        ChangeBigLeafDirectionSight();
    }
    //������ ���� Up
    private void Condition_Up_FixedUpdate(float fixedDeltaTime)
    {

    }

    private void ChangeBigLeafDirectionSight()
    {
        //ĳ������ �þ߸� �ٶ󺸰� ����
        Vector3 tempt = holdingCharacter.CurrentSightEulerAngle;
        Debug.Log($"{tempt} / {holdingCharacter.transform.eulerAngles}");

        Debug.Log($"{tempt.y - holdingCharacter.transform.eulerAngles.y}");

        if (tempt.y != holdingCharacter.transform.eulerAngles.y)
        {
            Debug.LogError("warning");
        }

        SetFakeCenterEulerAngle(tempt);
        //SetFakeCenterEulerAngle(holdingCharacter.transform.eulerAngles);
        //����� ������ �߰��� �����ϴ� ����. �⺻ ����� forward�� ����ϴ� �Ͱ�, �߰� ȸ���� ���� ���� �� ���̰� �ϱ� ���� �۾�
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(82.8943329f, 343.5914f, 26.6907959f)); // => ����� ������ ȸ������ ������ ã�� ��, �� ������ ������
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

}
