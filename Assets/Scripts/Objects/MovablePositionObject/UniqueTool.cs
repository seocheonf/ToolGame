using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;

public abstract class UniqueTool : MovablePositionObject
{
    protected List<FuncInteractionData> holdingFuncInteractionList;

    protected float angle;
    protected Character holdingCharacter;

    //잡힐 지점
    [SerializeField]
    protected Vector3 catchedLocalPosition;
    protected Vector3 CatchedLocalPosition
    {
        get
        {
            return transform.rotation * catchedLocalPosition;
        }
    }


#if UNITY_EDITOR

    public virtual Vector3 CatchedLocalPositionEdit
    {
        get
        {
            return transform.position + catchedLocalPosition;
        }
        set
        {
            catchedLocalPosition = value - transform.position;
        }
    }

    protected virtual void Reset()
    {
        GameObject child = new GameObject("CastTarget");
        child.layer = LayerMask.NameToLayer("Cast_UniqueTool");
        child.transform.parent = transform;
        child.transform.localPosition = Vector3.zero;
        child.AddComponent<SphereCollider>().isTrigger = true;
    }

#endif


    protected override void Initialize()
    {
        base.Initialize();
        
        holdingFuncInteractionList = new List<FuncInteractionData>();
    }

    public override void GetSpecialInteraction(WaterData source)
    {
        //들리지 않았을 때만 물의 효과를 받아라.
        if (holdingCharacter == null)
        {
            //조류
            AddForce(new ForceInfo(source.Direction * source.intensity, ForceType.DurationForce));
            //부력
            AddForce(new ForceInfo(Vector3.up * source.amount, ForceType.UnityDuration));
        }
    }


    public virtual void PutTool()
    {
        if(holdingCharacter == null)
        {
            Debug.LogError("도구가 들려있지 않은데 놓지 마세요!!");
            return;
        }

        //타겟 리지드바디 초기화
        currentRigidbody = initialRigidbody;

        //기존 본인 설정 초기화
        physicsInteractionObjectCollider.isTrigger = false;
        currentRigidbody.isKinematic = false;
        currentRigidbody.velocity = holdingCharacter.GetVelocity();
        //깔쌈한테스트// ControllerManager.RemoveInputFuncInteraction(holdingFuncInteractionList);
        transform.parent = null;
        holdingCharacter = null;

    }
    protected void CheckPutToolUpdate(float deltaTime)
    {

    }
    protected void PutToolEnd()
    {
        
    }
    public virtual void PickUpTool(Character source)
    {
        if(holdingCharacter != null)
        {
            Debug.LogError("도구가 이미 들려있는데 들려고 하지 마세요!!");
            return;
        }

        //기존 본인 설정 세팅
        transform.parent = source.transform;
        holdingCharacter = source;
        currentRigidbody.isKinematic = true;
        physicsInteractionObjectCollider.isTrigger = true;
        FakeCenterPosition = holdingCharacter.GetCatchingPosition();
        //깔쌈한테스트// ControllerManager.AddInputFuncInteraction(holdingFuncInteractionList);

        //타겟 리지드바디 설정
        currentRigidbody = holdingCharacter.CurrentRigidbody;

        Debug.Log($"{transform.eulerAngles} 대입 후 ");
    }


    public List<FuncInteractionData> GetHoldingFuncInteractionList()
    {
        return holdingFuncInteractionList;
    }




    /// <summary>
    /// 거짓 월드 중심 좌표. 이 값을 바꾸면, 이 거짓 월드 중심 좌표를 기준으로하는 좌표로 위치가 옮겨진다.
    /// </summary>
    public Vector3 FakeCenterPosition
    {
        get
        {
            return transform.position + CatchedLocalPosition;
        }
        set
        {
            transform.position = value - CatchedLocalPosition;
        }
    }
    
    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 하는 캐릭터의 회전 각도. 값을 변경하면, 거짓 월드 중심 좌표를 기준으로 회전한다.
    /// </summary>
    public Quaternion FakeCenterRotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            Vector3 beforePosition = FakeCenterPosition;
            transform.rotation = value;
            FakeCenterPosition = beforePosition;
        }
    }

    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 하는 캐릭터의 Up 방향 벡터. 거짓 월드 중심 좌표를 기준으로 특정 방향을 바라본다.
    /// </summary>
    public Vector3 FakeCenterUp
    {
        get
        {
            return transform.up;
        }
        set
        {
            Vector3 beforePosition = FakeCenterPosition;
            transform.up = value;
            FakeCenterPosition = beforePosition;
        }
    }

    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 eulerAnlge을 설정한다.
    /// </summary>
    /// <param name="eulerAngle">기준 eulerAngle</param>
    public void SetFakeCenterEulerAngle(Vector3 eulerAngle)
    {
        Vector3 tempt = FakeCenterPosition;
        transform.eulerAngles = eulerAngle;
        FakeCenterPosition = tempt;
    }

    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로, 현재 회전 상태에서 주어진 쿼터니언 회전만큼을 추가한다.
    /// </summary>
    /// <param name="quaternionRot">쿼터니언 회전 값</param>
    public void SetFakeCenterQuaternionProductRotation(Quaternion quaternionRot)
    {
        Vector3 tempt = FakeCenterPosition;
        transform.rotation *= quaternionRot;
        FakeCenterPosition = tempt;
    }

    public delegate void ToolTrasnformTask();
    /// <summary>
    /// 거짓 월드 중심 좌표를 기준으로 transform 변환을 주고 싶을 때, transform 변환 식을 매개변수로 받아, 거짓 월드 좌표를 기준으로 하는 작업을 진행한다.
    /// </summary>
    /// <param name="TransformTask">거짓 월드 중심 좌표를 기준으로 수행하고자 하는 transform 변환</param>
    public void SetFakeCenterFreeTransform(ToolTrasnformTask TransformTask)
    {
        Vector3 tempt = FakeCenterPosition;
        TransformTask();
        FakeCenterPosition = tempt;
    }


    protected override void RegisterFuncInInitialize()
    {
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
        GameManager.ObjectsFixedUpdate += MainFixedUpdate;
    }
    protected override void RemoveFuncInDestroy()
    {
        GameManager.ObjectsFixedUpdate -= MainFixedUpdate;
    }



    //깔쌈한테스트//
    public virtual List<FuncInteractionData> GetHoldingFuncInteractionDataList()
    {
        return holdingFuncInteractionList;
    }
}
