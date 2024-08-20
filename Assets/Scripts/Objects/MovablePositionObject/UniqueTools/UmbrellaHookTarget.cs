using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UmbrellaHookTarget : MyComponent
{
    private Rigidbody rigid;
    public Rigidbody HookRigid => rigid;


    protected override void MyStart()
    {
        base.MyStart();
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = true;
    }
}
