using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable : Character
{
    //9ũ��
    private UniqueTool[] currentStoreUniqueTool;

    private UniqueTool currentTargetUniqueTool;
    private IOuterFuncInteraction currentTargetOuterFuncInteraction;

    private List<FuncInteractionData> originInputFuncInteractionList;
    private List<FuncInteractionData> currentHoldingFuncInteractionList;
    private List<FuncInteractionData> currentOuterFuncInteractionList;

    private bool isSit;

    private int currentSightOrdinal;

    private bool isJump;


    protected override void MyStart()
    {
        base.MyStart();
        FuncInteractionData jump = new();
        jump.keyCode = KeyCode.Space;
        jump.description = "����";
        jump.OnFuncInteraction = OnJump;
        ControllerManager.AddInputFuncInteraction(jump);

        FuncInteractionData forward = new();
        forward.keyCode = KeyCode.W;
        forward.description = "������ �̵�";
        forward.OnFuncInteraction = OnMoveForward;
        ControllerManager.AddInputFuncInteraction(forward);

        FuncInteractionData backward = new();
        backward.keyCode = KeyCode.S;
        backward.description = "�ڷ� �̵�";
        backward.OnFuncInteraction = OnMoveBackward;
        ControllerManager.AddInputFuncInteraction(backward);

        FuncInteractionData left = new();
        left.keyCode = KeyCode.A;
        left.description = "�������� �̵�";
        left.OnFuncInteraction = OnMoveLeft;
        ControllerManager.AddInputFuncInteraction(left);

        FuncInteractionData right = new();
        right.keyCode = KeyCode.D;
        right.description = "���������� �̵�";
        right.OnFuncInteraction = OnMoveRight;
        ControllerManager.AddInputFuncInteraction(right);
    }

    protected override void Jump()
    {
        isJump = true;
    }

    private void OnSit()
    {

    }
    private void Sit()
    {

    }
    private void UnSit()
    {

    }

    private void OnRush()
    {

    }
    private void Rush()
    {

    }

    private void ChangeSightOrdinal()
    {

    }

    private void TargetGameObjectUpdate()
    {

    }
    private void TargetUniqueTool()
    {

    }
    private void TargetOuterFuncInteraction()
    {

    }

    private void DoTargetingUniqueTool()
    {

    }
    private void DoUnTargetingUniqueTool()
    {

    }

    private void DoTargetingOuterFuncInteraction()
    {

    }
    private void DoUnTargetingOuterFuncInteraction()
    {

    }

    private void OnSwitchFuncInteraction()
    {

    }
}
