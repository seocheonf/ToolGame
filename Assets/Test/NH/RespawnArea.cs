using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnArea : MonoBehaviour
{
    public Transform respawn; 
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other.GetComponent<Playable>())
        {
            other.transform.position = respawn.transform.position;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
