using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HngmoHookTest : MonoBehaviour
{
    public Rigidbody target;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            target.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
        }
    }
}
