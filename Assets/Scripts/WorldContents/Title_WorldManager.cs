using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_WorldManager : WorldManager
{

    TitleUI titleUI;

    protected override IEnumerator RemainInitiate()
    {
        yield return base.RemainInitiate();

        GameManager.TurnOnBasicLoadingCavnas("Semi World Loading...");

        titleUI = GameManager.Instance.UI.GetFixedUI<TitleUI>(ToolGame.FixedUIType.TitleUI);

        titleUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        yield return null;
        GameManager.TurnOffBasicLoadingCanvas();
    }

    //protected override IEnumerator Initiate()
    //{
    //    yield return base.Initiate();
        
    //    GameManager.Instance.IsScriptEntireUpdateStop = true;

    //    GameManager.TurnOnBasicLoadingCavnas("Semi World Loading...");

    //    titleUI = GameManager.Instance.UI.GetFixedUI<TitleUI>(ToolGame.FixedUIType.TitleUI);

    //    titleUI.SetActive(true);

    //    yield return null;
    //    GameManager.TurnOffBasicLoadingCanvas();

    //    GameManager.Instance.IsScriptEntireUpdateStop = false;

    //}
}
