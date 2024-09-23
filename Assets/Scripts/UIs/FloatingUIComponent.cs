using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;

public class FloatingUIComponent : UIComponent
{

    private bool isBlocking;
    private FloatingUIType uiType;

    public bool IsBlocking
    {
        get
        {
            return isBlocking;
        }
        set
        {
            isBlocking = value;
        }
    }

    public FloatingUIType UIType
    {
        get
        {
            return uiType;
        }
        set
        {
            uiType = value;
        }
    }

}
