using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SDFMovement))]
public class SDFEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SDFMovement movement = target as SDFMovement;
        if (GUILayout.Button("Calculate EDT"))
        {
            if (movement == null) return;
            
            movement.Initialize();
            movement.RunRasterization();
            movement.EDT();
        }
    }
}

