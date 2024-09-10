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
        player.SetCrowdControl(CrowdControlState.Stun, 3); //ĳ���Ϳ��� �ϴ� ������ �ο� �� ��
        isAction = true;//���� �����̴°� ����� �ϰ�
        player.transform.position = grabPosition.transform.position; //�÷��̾ �� 1�� ���� ���� ����־�� ��
        if (joint == null) joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = player.GetComponent<Rigidbody>();

        //�� ������ �ϴ°ɱ�.......
        //�� ����� ������ �κ�ó�� ������ �ִٰ� ���� ���������� �Ǵµ�
        //��� ������� �����ϴ°Ű��� ����?
    }

    void ThrowTimeUpdate(Playable player, float deltaTime)
    {
        if (isAction)
        {
            throwTime -= deltaTime;

            if (throwTime <= 0)
            {
                Destroy(joint);
                player.AddForce((player.transform.forward * wantPower + Vector3.up * wantUpPower), ForceType.VelocityForce); // 1�ʰ� ������ �� �ڵ带 �����ؾߵ�
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
