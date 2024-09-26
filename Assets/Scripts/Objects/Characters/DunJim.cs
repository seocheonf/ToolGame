using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;

public class DunJim : Rascal
{
    FixedJoint joint = null;
    protected Playable throwPlayer;
    [SerializeField] Transform grabPosition;
    [SerializeField] float wantUpPower;
    [SerializeField] float wantPower;
    float throwTime = 1;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!throwPlayer)
        {
            throwPlayer = collision.collider.GetComponent<Playable>();
        }

        if (collision.collider.GetComponent<Playable>())
        {
            ThrowPlayer(throwPlayer);
        }
    }

    void ThrowPlayer(Playable player)
    {
        currentGeneralState = GeneralState.Action;
        player.SetCrowdControl(CrowdControlState.Stun, 3); //캐릭터에게 일단 스턴을 부여 한 뒤
        isAction = true;//적은 움직이는걸 멈춰야 하고
        player.transform.position = grabPosition.transform.position; //플레이어를 한 1초 동안 위로 잡고있어야 됨
        if (joint == null) joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = player.GetComponent<Rigidbody>();

        //왜 점프를 하는걸까.......
        //그 고양이 마리오 로봇처럼 가만히 있다가 냅다 던져버려야 되는데
        //계속 비빌려고 점프하는거같은 느낌?
    }

    void ThrowTimeUpdate(Playable player, float deltaTime)
    {
        if (isAction)
        {
            throwTime -= deltaTime;

            if (throwTime <= 0)
            {
                Destroy(joint);
                player.AddForce((player.transform.forward * wantPower + Vector3.up * wantUpPower), ForceType.VelocityForce); // 1초가 지나면 이 코드를 실행해야됨
                currentGeneralState = GeneralState.Normal;
                throwTime = 1;
                isAction = false;
            }
        }
    }

    private void Update()
    {
        ThrowTimeUpdate(throwPlayer, Time.deltaTime);
    }
}
