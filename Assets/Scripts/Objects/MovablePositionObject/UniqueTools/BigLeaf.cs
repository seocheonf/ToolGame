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
    /// 상태에 따라서 지속적으로 해야할 일
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

        //상황에 따른 키입력 기능 보관 장소 중비
        conditionFuncInteractionDictionary = new Dictionary<BigLeafCondition, List<FuncInteractionData>>();
        for(BigLeafCondition i = 0; i < BigLeafCondition.Length; i++)
        {
            conditionFuncInteractionDictionary[i] = new List<FuncInteractionData>();
        }

        //Normal 상태일 때 할 일 대기
        //conditionFuncInteractionDictionary[BigLeafCondition.Normal].Add(new FuncInteractionData(,,,,));
        //Forward 상태일 때 할 일 대기
        //conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(,,,,));
        //Up 상태일 때 할 일 대기
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
            default://비상용
                Debug.LogError("잘못된 상태 접근");
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


    //들려있지 않을 때는 지속적으로 해야할 일들을 변화시키지 말아야 해요.

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


    //나뭇잎 상태 일반
    private void Condition_Normal_FixedUpdate(float fixedDeltaTime)
    {

    }
    //나뭇잎 상태 Forward
    private void Condition_Forward_FixedUpdate(float fixedDeltaTime)
    {
        ChangeBigLeafDirectionSight();
    }
    //나뭇잎 상태 Up
    private void Condition_Up_FixedUpdate(float fixedDeltaTime)
    {

    }

    private void ChangeBigLeafDirectionSight()
    {
        //캐릭터의 시야를 바라보게 설정
        Vector3 tempt = holdingCharacter.CurrentSightEulerAngle;
        Debug.Log($"{tempt} / {holdingCharacter.transform.eulerAngles}");

        Debug.Log($"{tempt.y - holdingCharacter.transform.eulerAngles.y}");

        if (tempt.y != holdingCharacter.transform.eulerAngles.y)
        {
            Debug.LogError("warning");
        }

        SetFakeCenterEulerAngle(tempt);
        //SetFakeCenterEulerAngle(holdingCharacter.transform.eulerAngles);
        //대상의 방향을 추가로 조정하는 과정. 기본 우산의 forward를 고려하는 것과, 추가 회전을 통해 눈에 잘 보이게 하기 위한 작업
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(82.8943329f, 343.5914f, 26.6907959f)); // => 현재는 에셋을 회전시켜 정면을 찾은 뒤, 그 각도를 가져옴
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
