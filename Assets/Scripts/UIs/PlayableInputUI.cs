using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableInputUI : FixedUIComponent
{
    public RectTransform TextsArea;
    private float initialHeight;

    protected override void MyStart()
    {
        base.MyStart();
        initialHeight = 300;
    }

    public void SetInputInfo(List<FuncInteractionData> list)
    {

    }

    public void SetScrollPosition(float scrollPosition)
    {
        Vector3 next = TextsArea.localPosition;
        next.y += scrollPosition;
        next.y = Mathf.Clamp(next.y, -initialHeight, initialHeight);
        //Debug.Log(next.y);
        TextsArea.localPosition = next;
    }


}
