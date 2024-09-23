using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;

public class FixedUIComponent : MonoBehaviour
{

    private FixedUIType uiType;

    public FixedUIType UIType
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
