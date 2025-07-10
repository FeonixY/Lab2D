using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SDFMovement))]
public class SDFEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SDFMovement test = target as SDFMovement;
        if (GUILayout.Button("Calculate EDT"))
        {
            test.Initialize();
            test.RunRasterization();
            test.EDT();
        }
    }
}

