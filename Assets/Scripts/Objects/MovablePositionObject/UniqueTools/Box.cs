using SpecialInteraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolGame;

public class Box : UniqueTool
{
    public override void GetSpecialInteraction(WindData source)
    {
        if (source.origin == this)
        {
            return;
        }

        AddForce(new ForceInfo(source.Direction * source.intensity * 3f, ForceType.DurationForce));
    }
}
