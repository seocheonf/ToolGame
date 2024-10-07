using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum PortalTarget
{
    Scene_S,
    Scene_I,
    Portal
}

delegate void ResponsePlayer(Playable playable);

public class PortalDevice : MyComponent
{
    [SerializeField]
    PortalTarget target;

    [SerializeField]
    private string targetSceneName;
    [SerializeField]
    private int targetSceneIndex;
    [SerializeField]
    private PortalDevice targetPortal;

    private bool ready = false;

    private ResponsePlayer DoOnPlayerEnter;

    protected override void MyStart()
    {
        base.MyStart();
        ready = true;
        SetPortalTarget(target);
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
        ready = false;
        DoOnPlayerEnter = null;
    }

    private void SetPortalTarget(PortalTarget newTarget)
    {
        target = newTarget;
        switch(target)
        {
            case PortalTarget.Scene_S:
                DoOnPlayerEnter = DoSceneChangeS;
                break;
            case PortalTarget.Scene_I:
                DoOnPlayerEnter = DoSceneChangeI;
                break;
            case PortalTarget.Portal:
                DoOnPlayerEnter = DoTeleport;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ready && other.TryGetComponent(out Playable result))
        {
            DoOnPlayerEnter?.Invoke(result);
        }
    }

    private void DoSceneChangeS(Playable playable)
    {
        GameManager.Instance.SceneChange(targetSceneName);
    }
    private void DoSceneChangeI(Playable playable)
    {
        GameManager.Instance.SceneChange(targetSceneIndex);
    }
    private void DoTeleport(Playable playable)
    {
        if (targetPortal != null)
            playable.transform.position = targetPortal.transform.position;
        else
            Debug.LogError("이동 목적지가 되는 포탈이 없어요!!!!");
    }
}
