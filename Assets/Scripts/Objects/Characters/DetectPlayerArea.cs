using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerArea : Rascal
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Playable>())
        {
            isPlayerIn = true;
        }
    }
}
