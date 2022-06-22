using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircleWallSetting))]
public class CircleWallSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CircleWallSetting circleWallSetting = (CircleWallSetting)target;

        if (GUILayout.Button("Setting Circle Wall", GUILayout.Width(120), GUILayout.Height(20)))
        {
             circleWallSetting.SettingCircleWall();
        }

    }

}
