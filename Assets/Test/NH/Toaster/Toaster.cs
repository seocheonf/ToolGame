using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toaster : LimitPositionObject, IOnOffFuncInteraction
{
    [SerializeField] GameObject toast;
    [SerializeField] Collider playerDetectArea;
    [SerializeField] Transform toastMakeTransform;
    [SerializeField] float wantShootPower;

    public void DoOn()
    {
        playerDetectArea.enabled = true;
    }

    public void DoOff()
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
