using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VacuumShaders.AdvancedDissolve
{
    public class UnlitMasterGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);


            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;



            //VacuumShaders
            VacuumShaders.AdvancedDissolve.MaterialProperties.Init(properties);

            //VacuumShaders
            VacuumShaders.AdvancedDissolve.MaterialProperties.DrawDissolveOptions(materialEditor, true, true);
        }
    }
}