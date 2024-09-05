using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toast : MovablePositionObject
{
    public Collider toaster;
    [SerializeField] CrowdControlState giveCrowdControlState;
    [SerializeField] float duration;
    // 토스터가 토스트를 버그없이 발사 될려면
    // 토스트는 Collider가 off 상태를 기본적으로 놔둬야 하고
    // Ignore -> Collider On -> 발사
    // 이 순서대로 작동해야만 원래 원했던 기능이 작동 됨  

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
