using SpecialInteraction;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BigLeaf : UniqueTool
{
    /// <summary>
    /// 추가 부력 배율. 0이상의 값
    /// </summary>
    private const float bigLeafBuoyancyRatio = 2f;

    enum BigLeafCondition
    {
        Normal,
        Forward,
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
        conditionFuncInteractionDictionary[BigLeafCondition.Forward].Add(new FuncInteractionData(KeyCode.F,"바람 일으키기 On/Off",SetWindOnOff,null,null));
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
        //일반적인 상태라면 바람을 끄도록 하자. 내가 들고 나서 켜야만 켜진 상태로 유지 가능!
        SetWind(false);


        ConditionFixedUpdate = null;
        ConditionFixedUpdate = Condition_Normal_FixedUpdate;

    }
    private void SetForwardCondition()
    {

        //forward는 들리지 않았을 때는 할 일을 해서는 안됨
        if (holdingCharacter != null)
        {
            ConditionFixedUpdate = null;
            ConditionFixedUpdate = Condition_Forward_FixedUpdate;
        }
    }


    //나뭇잎 상태 일반
    private void Condition_Normal_FixedUpdate(float fixedDeltaTime)
    {
        if (GetDownSpeed() <= 0)
        {
            float dotValue = Vector3.Dot(Vector3.up, transform.forward);
            dotValue = dotValue > 0 ? dotValue : -dotValue;
            AccelDownForce(1 - dotValue * 0.05f);
        }
    }

    //나뭇잎 상태 Forward
    private void Condition_Forward_FixedUpdate(float fixedDeltaTime)
    {
        ChangeBigLeafDirectionSight(fixedDeltaTime);
    }


    float animationValue;
    private void ChangeBigLeafDirectionSight(float fixedDeltaTime)
    {
        //캐릭터의 시야를 바라보게 설정
        Vector3 tempt = holdingCharacter.CurrentSightEulerAngle_Interaction;
        //Debug.Log($"{tempt} / {holdingCharacter.transform.eulerAngles}");

        //Debug.Log($"{tempt.y - holdingCharacter.transform.eulerAngles.y}");

        //if (tempt.y != holdingCharacter.transform.eulerAngles.y)
        //{
        //    Debug.LogError("warning");
        //}
        SetFakeCenterEulerAngle(tempt);
        //SetFakeCenterEulerAngle(holdingCharacter.transform.eulerAngles);

        //대상의 방향을 추가로 조정하는 과정. 기본 우산의 forward를 고려하는 것과, 추가 회전을 통해 눈에 잘 보이게 하기 위한 작업
        float additionalValue = 15f;
        additionalValue *= MathF.Sin(animationValue);
        animationValue += fixedDeltaTime * 6f;
        float fanningValue = isFanning ? 82.8943329f + additionalValue : 82.8943329f;
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(fanningValue, 343.5914f, 26.6907959f)); // => 현재는 에셋을 회전시켜 정면을 찾은 뒤, 그 각도를 가져옴
        Vector3 modelPosition = transform.position;
        Quaternion modelRotation = transform.rotation;
        SetFakeCenterEulerAngle(tempt);
        SetFakeCenterQuaternionProductRotation(Quaternion.Euler(82.8943329f, 343.5914f, 26.6907959f)); // => 현재는 에셋을 회전시켜 정면을 찾은 뒤, 그 각도를 가져옴
        bigLeafModeling.transform.position = modelPosition;
        bigLeafModeling.transform.rotation = modelRotation;
    }


    public override void GetSpecialInteraction(WaterData source)
    {
        //조류
        AddForce(new ForceInfo(source.Direction * source.intensity, ForceType.DurationForce));
        //부력
        AddForce(new ForceInfo(Vector3.up * source.amount * bigLeafBuoyancyRatio, ForceType.UnityDuration));

        
        if (holdingCharacter == null)
        {
            
        }
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

        //바람이 꺼진다면 모델링과 원본 방향 재 조정
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

}
