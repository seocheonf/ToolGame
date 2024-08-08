#if UNITY_EDITOR
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpecialInteraction;

[CustomEditor(typeof(Character), true)]
public class SetCatchingPointEdit : Editor
{
    private void OnSceneGUI()
    {

        Character targetCharacter = target as Character;

        targetCharacter.CatchingLocalPositionEdit = Handles.PositionHandle(targetCharacter.CatchingLocalPositionEdit, Quaternion.identity);

    }
}
#endif