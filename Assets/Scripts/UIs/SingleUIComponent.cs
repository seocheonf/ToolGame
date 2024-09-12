using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;

public class SingleUIComponent : UIComponent
{
    private SingleUIType myType;

    public void SetInfo(SingleUIType uiType)
    {
        myType = uiType;
    }
    public SingleUIType GetType()
    {
        return myType;
    }

}
