using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum PortalTarget
{
    Scene_S,
    Scene_I,
    Portal,
    None
}

delegate void ResponseGameObject(GameObject playable);

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

    private ResponseGameObject DoOnPlayerEnter;

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
            case PortalTarget.None:
                DoOnPlayerEnter = DoNone;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ready)// && other.TryGetComponent(out Playable result))
        {  
            DoOnPlayerEnter?.Invoke(other.gameObject);
        }
    }

    private void DoSceneChangeS(GameObject target)
    {
        if (target.TryGetComponent(out Playable result))
        {
            GameManager.Instance.SceneChange(targetSceneName);
        }
    }
    private void DoSceneChangeI(GameObject target)
    {
        if (target.TryGetComponent(out Playable result))
        {
            GameManager.Instance.SceneChange(targetSceneIndex);
        }
    }
    private void DoTeleport(GameObject target)
    {
        //if(targetPortal != null)
        //{
        //    if(target.TryGetComponent(out ITeleportTarget_Portal result))
        //    {
        //        result.TP_Portal(targetPortal.transform.position);
        //    }
        //}
        //else
        //    Debug.LogError("이동 목적지가 되는 포탈이 없어요!!!!");

        // 위 VS 아래

        if(targetPortal == null)
        {
            Debug.LogError("이동 목적지가 되는 포탈이 없어요!!!!");
            return;
        }

        if (target.TryGetComponent(out ITeleportTarget_Portal result))
        {
            result.TP_Portal(targetPortal.transform.position);
            return;
        }
    }
    private void DoNone(GameObject playable)
    {

    }
}



//==============================================================================//



/// <summary>
/// 포탈에 의한 TP대상에게 부착한다.
/// </summary>
public interface ITeleportTarget_Portal
{
    public void TP_Portal(Vector3 targetPosition);
}