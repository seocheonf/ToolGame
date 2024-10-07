using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleUI : FixedUIComponent
{

    [SerializeField]
    Button GameStartButton;
    [SerializeField]
    Button OptionButton;
    [SerializeField]
    Button GameEndButton;


    protected override void MyStart()
    {
        base.MyStart();

        GameStartButton.onClick.AddListener(() =>
        {
            UIManager.SetSelectedNull();
            GameStart();
        });
        OptionButton.onClick.AddListener(() =>
        {
            UIManager.SetSelectedNull();
        });
        GameEndButton.onClick.AddListener(() =>
        {
            UIManager.SetSelectedNull();
            GameQuit();
        });

    }

    private void GameQuit()
    {
        Application.Quit();
    }

    private void GameStart()
    {
        GameManager.Instance.SceneChange("InGameStage2");
    }

}
