using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AstarMapBaking))]
public class AstarMapBakingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AstarMapBaking astarMapBaking = (AstarMapBaking)target;

        if (GUILayout.Button("Baking", GUILayout.Width(120), GUILayout.Height(20)))
        {
            astarMapBaking.Baking();
        }

    }

}
