#if UNITY_EDITOR
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpecialInteraction;

[CustomEditor(typeof(Water))]
public class DirectionSettingWater : Editor
{
    private void OnSceneGUI()
    {

        Water targetWater = target as Water;

        Handles.color = Color.blue;

        targetWater.GetWaterData(out WaterData result);

        Quaternion tempt = Handles.DoRotationHandle(result.offset, targetWater.transform.position);
        targetWater.SetWaterData(ref tempt);

    }
}























#endif