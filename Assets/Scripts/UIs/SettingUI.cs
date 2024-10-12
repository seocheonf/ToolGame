using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : FloatingUIComponent
{
    [SerializeField]
    private Button closeTab;

    protected override void MyStart()
    {
        base.MyStart();

        closeTab.onClick.AddListener(() =>
        {
            UIManager.SetSelectedNull();
            OutOption();
        });
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
    }
    private void OutOption()
    {
        SetActive(false);
    }
}
