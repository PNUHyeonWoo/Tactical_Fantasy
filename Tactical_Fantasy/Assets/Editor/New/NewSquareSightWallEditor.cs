using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NewSquareSightWall))]
public class NewSquareSightWallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NewSquareSightWall Comp = (NewSquareSightWall)target;

        if (GUILayout.Button("Setting Rect Wall", GUILayout.Width(120), GUILayout.Height(20)))
        {
            Comp.WallObjButton();
        }

    }
}
