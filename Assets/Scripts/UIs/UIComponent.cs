using System;
using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;

public class UIComponent : MyComponent
{ 
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
