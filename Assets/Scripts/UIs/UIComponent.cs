using System;
using System.Collections;
using System.Collections.Generic;
using ToolGame;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIComponent : MyComponent
{

    public StartFunction UIStart;
    public DestroyFunction UIDestroy;

    private RectTransform rectInfo;
    public RectTransform RectInfo => rectInfo;

    private void Awake()
    {
        rectInfo = GetComponent<RectTransform>();
    }

    //==========================================

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void TriggerActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public virtual void StartUI()
    {

    }

    public virtual void EndUI()
    {

    
    
    }

    protected override void MyStart()
    {
        base.MyStart();
        UIStart?.Invoke();
    }

    protected override void MyDestroy()
    {
        UIDestroy?.Invoke();
        base.MyDestroy();
    }

}
