using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingPlayerArea : Rascal
{
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Playable>())
        {
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerIn = false;
        moveSpeed = defaultMoveSpeed;
        currentGeneralState = GeneralState.Normal;
        GetRandomPos();
    }
}
