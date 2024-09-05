using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toast : MovablePositionObject
{
    public Collider toaster;
    [SerializeField] CrowdControlState giveCrowdControlState;
    [SerializeField] float duration;
    // �佺�Ͱ� �佺Ʈ�� ���׾��� �߻� �ɷ���
    // �佺Ʈ�� Collider�� off ���¸� �⺻������ ���־� �ϰ�
    // Ignore -> Collider On -> �߻�
    // �� ������� �۵��ؾ߸� ���� ���ߴ� ����� �۵� ��  

    private void Update()
    {
        IgnoreToaster();
    }

    void IgnoreToaster()
    {
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), toaster);
        gameObject.GetComponent<Collider>().enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Character>())
        {
            collision.collider.GetComponent<Character>().SetCrowdControl(giveCrowdControlState, duration);
        }
    }

    protected override void RegisterFuncInInitialize()
    {

    }
    protected override void RemoveFuncInDestroy()
    {

    }
}
