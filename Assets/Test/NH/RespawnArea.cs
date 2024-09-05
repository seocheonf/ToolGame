using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnArea : MonoBehaviour
{
    public Transform respawn; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            other.transform.position = respawn.transform.position;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
