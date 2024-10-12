using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InStageOption : FloatingUIComponent
{
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button settingButton;
    [SerializeField]
    private Button exitButton;

    private SettingUI settingUI;

    private void Awake()
    {
        settingUI = GameManager.Instance.UI.GetFloatingUI<SettingUI>(ToolGame.FloatingUIType.SettingUI);
    }

    protected override void MyStart()
    {
        base.MyStart();

        Cursor.lockState = CursorLockMode.None;

        resumeButton.onClick.AddListener(() =>
        {
            UIManager.SetSelectedNull();
            OutOption();
        });
        settingButton.onClick.AddListener(() =>
        {
            UIManager.SetSelectedNull();
            PopSettingUI();
        });
        exitButton.onClick.AddListener(() =>
        {
            UIManager.SetSelectedNull();
            OutOption();
            ExitToTitle();
        });

    }

    protected override void MyDestroy()
    {
        base.MyDestroy();

        Cursor.lockState = CursorLockMode.Locked;

        resumeButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();

    }

    private void OutOption()
    {
        SetActive(false);
    }

    private void PopSettingUI()
    {
        settingUI.SetActive(true);
    }

    private void ExitToTitle()
    {
        GameManager.Instance.SceneChange("Title");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12))
        {
            OutOption();
            ExitToTitle();
        }
    }
}
