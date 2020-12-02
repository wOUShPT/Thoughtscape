using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.Rendering.Universal
{
    public class DetailLit : UnityEditor.ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            VacuumShaders.AdvancedDissolve.MaterialProperties.Init(properties);


            float fieldWidth = EditorGUIUtility.fieldWidth;
            float labelWidth = EditorGUIUtility.labelWidth;

            if (VacuumShaders.AdvancedDissolve.MaterialProperties.DrawSurfaceInputs(materialEditor))
            {
                base.OnGUI(materialEditor, properties);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();



            EditorGUIUtility.fieldWidth = fieldWidth;
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.BeginChangeCheck();
            VacuumShaders.AdvancedDissolve.MaterialProperties.DrawDissolveOptions(materialEditor, true, false);
            if(EditorGUI.EndChangeCheck())
            {
                Material material = materialEditor.target as Material;
                VacuumShaders.AdvancedDissolve.MaterialProperties.MaterialChanged(material);
            }
        }
    }
}