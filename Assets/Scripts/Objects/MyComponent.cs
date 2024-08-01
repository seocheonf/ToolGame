using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyComponent : MonoBehaviour
{

    protected virtual void MyStart() { }
    protected virtual void MyDestroy() { }

    protected virtual void OnEnable()
    {
        GameManager.ObjectsStart -= MyStart;
        GameManager.ObjectsStart += MyStart;
    }
    protected virtual void OnDisable()
    {
        GameManager.ObjectsDestroy -= MyDestroy;
        GameManager.ObjectsDestroy += MyDestroy;
    }
}