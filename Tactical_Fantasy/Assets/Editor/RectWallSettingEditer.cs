using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RectWallSetting))]
public class RectWallSettingEditer : Editor
{
    public override void OnInspectorGUI()
    {
        RectWallSetting rectWallSetting = (RectWallSetting)target;

        if(GUILayout.Button("Setting Rect Wall",GUILayout.Width(120), GUILayout.Height(20)))
        {
            rectWallSetting.SettingRectWall();
        }

    }
}
