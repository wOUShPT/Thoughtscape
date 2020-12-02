using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VacuumShaders.AdvancedDissolve
{
    public class DefaultShaderGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            //VacuumShaders
            VacuumShaders.AdvancedDissolve.MaterialProperties.Init(properties);

            if (VacuumShaders.AdvancedDissolve.MaterialProperties.DrawSurfaceInputs(materialEditor))
            {
                base.OnGUI(materialEditor, properties);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;



            

            //VacuumShaders
            VacuumShaders.AdvancedDissolve.MaterialProperties.DrawDissolveOptions(materialEditor, true, true);
        }
    }
}