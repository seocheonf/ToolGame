using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toaster : LimitPositionObject
{
    [SerializeField] GameObject toast;
    [SerializeField] Collider playerDetectArea;
    [SerializeField] Transform toastMakeTransform;
    [SerializeField] float wantShootPower;


    public override void ObjectOn() 
    {
        playerDetectArea.enabled = true;
    }

    public override void ObjectOff()
    {
        playerDetectArea.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>())
        {
            GameObject freshToast;
            freshToast = Instantiate(toast, toastMakeTransform);
            freshToast.GetComponent<Toast>().toaster = gameObject.GetComponent<BoxCollider>();
            freshToast.GetComponent<Rigidbody>().AddForce(transform.forward * wantShootPower, ForceMode.Impulse);
            Destroy(freshToast, 3f);
        }
    }
}
