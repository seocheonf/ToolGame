#if UNITY_EDITOR
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpecialInteraction;

[CustomEditor(typeof(UniqueTool), true)]
public class SetCatchedPointEdit : Editor
{

    private void OnSceneGUI()
    {

        UniqueTool targetTool = target as UniqueTool;

        targetTool.CatchedLocalPositionEdit = Handles.PositionHandle(targetTool.CatchedLocalPositionEdit, Quaternion.identity);
        
    }

}

#endif