using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_WorldManager : WorldManager
{
    private InStageOption stageOption;

    protected override IEnumerator RemainInitiate()
    {
        yield return base.RemainInitiate();

        stageOption = GameManager.Instance.UI.GetFloatingUI<InStageOption>(ToolGame.FloatingUIType.InStageOption);

        GameManager.Instance.UI.SetNonFloating(TurnOnStageOption);

        Cursor.lockState = CursorLockMode.Locked;

    }

    private void TurnOnStageOption()
    {
        stageOption.SetActive(true);
    }

    public override void WorldManagerDestroy()
    {
        base.WorldManagerDestroy();

        GameManager.Instance.UI.UnSetNonFloating();
    }
}
