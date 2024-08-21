using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HngmoHookTest2 : MonoBehaviour
{
    public GameObject hook;
    
    // Update is called once per frame
    void Update()
    {
        transform.up = hook.transform.position - transform.position;
    }
}
