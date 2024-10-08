using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raccoon : Rascal
{
    [SerializeField] Bowl bowl;


    protected override void OnTriggerEnter(Collider other)
    {

    }

    protected override void OnTriggerStay(Collider other)
    {

    }

    protected override void OnTriggerExit(Collider other)
    {
        
    }

    void UpdateDirection()
    {
        if (bowl.foodValues > 3)
        {
            moveSpeed = defaultMoveSpeed + defaultAccelSpeed;
            destination = new Vector3(bowl.raccoonDirection.position.x, 0, bowl.raccoonDirection.position.z);
        }

    }

    private void Update()
    {
        UpdateDirection();
    }

    protected override void AnimatorUpdate()
    {

    }
}