using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    class AdvancedDissolve_PBRMasterGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
			materialEditor.PropertiesDefaultGUI(props);

            foreach (MaterialProperty prop in props)
            {
                if (prop.name == "_EmissionColor")
                {
                    if (materialEditor.EmissionEnabledProperty())
                    {
                        materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
                    }
                    return;
                }
            }


            VacuumShaders.AdvancedDissolve.MaterialProperties.Init(props);

            VacuumShaders.AdvancedDissolve.MaterialProperties.DrawDissolveOptions(materialEditor, false, true);
        }
    }
}
