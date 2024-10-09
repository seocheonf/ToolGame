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

delegate void ResponsePlayer(GameObject playable);

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

    private void DoSceneChangeS(GameObject playable)
    {
        if (playable.TryGetComponent(out Playable result))
        {
            GameManager.Instance.SceneChange(targetSceneName);
        }
    }
    private void DoSceneChangeI(GameObject playable)
    {
        if (playable.TryGetComponent(out Playable result))
        {
            GameManager.Instance.SceneChange(targetSceneIndex);
        }
    }
    private void DoTeleport(GameObject playable)
    {
        if (targetPortal != null)
        {
            if(!playable.TryGetComponent(out UniqueTool result2))
            {
                playable.transform.position = targetPortal.transform.position;
            }
            else
            {
                result2.TP(targetPortal.transform);
            }
        }
        else
            Debug.LogError("�̵� �������� �Ǵ� ��Ż�� �����!!!!");

        if(playable.TryGetComponent(out MovablePositionObject result))
        {
            result.CurrentRigidbody.velocity = Vector3.zero;
        }
    }
    private void DoNone(GameObject playable)
    {

    }
}



//==============================================================================//



/// <summary>
/// ��Ż�� ���� TP��󿡰� �����Ѵ�.
/// </summary>
public interface ITeleportTarget_Portal
{
    public void TP(Vector3 targetPosition);
}