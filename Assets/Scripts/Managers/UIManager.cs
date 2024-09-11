using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{

}

public class UIManager : Manager
{

    public T GetUI<T>(UIType uiType)
    {
        return default;
    }
    public void GetUI(UIType uiType)
    {

    }





    public override IEnumerator Initiate()
    {
        return base.Initiate();
    }
}
