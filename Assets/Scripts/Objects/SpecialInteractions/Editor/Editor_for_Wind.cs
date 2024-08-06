#if UNITY_EDITOR
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpecialInteraction;

[CustomEditor(typeof(Wind))]
public class DirectionSetting : Editor
{
    private void OnSceneGUI()
    {
        Wind targetWind = target as Wind;

        Handles.color = Color.blue;

        targetWind.GetWindData(out WindData result);

        //Handles.DrawLine(targetWind.transform.position, targetWind.transform.position + result.offset * targetWind.transform.rotation * Vector3.forward * 5f, 5);

        Quaternion tempt = Handles.DoRotationHandle(result.offset, targetWind.transform.position);
        targetWind.SetWindData(ref tempt);

    }
}























#endif