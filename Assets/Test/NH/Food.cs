using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : UniqueTool
{
    public override void PickUpTool(Character source)
    {
        
        transform.rotation = (Quaternion.Euler(90, 0, 0));
        Debug.Log($"{transform.eulerAngles} ¥Î¿‘ ¿¸ ");
        base.PickUpTool(source);
    }

    public override void PutTool()
    {
        transform.rotation = Quaternion.identity;
        base.PutTool();

    }
}
