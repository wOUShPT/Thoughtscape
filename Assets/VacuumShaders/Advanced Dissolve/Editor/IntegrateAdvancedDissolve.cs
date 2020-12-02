using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class IntegrateAdvancedDissolve : Editor
{
    [MenuItem("Assets/VacuumShaders/Integrate Advanced Dissolve", false, 4201)]
    static public void Menu()
    {
        Integrate(Selection.activeObject);
    }

    [MenuItem("Assets/VacuumShaders/Integrate Advanced Dissolve", true, 4201)]
    static public bool Validate_Menu()
    {
        if (Selection.activeObject == null)
            return false;

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
            return false;

        if (Path.GetExtension(path) != ".shader")
            return false;


        return true;
    }

    static bool IsAssetReady(Object obj)
    {
        if (obj == null)
            return false;


        string path = AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Asset path is NULL");
            return false;
        }

        if (Path.GetExtension(path) != ".shader")
        {
            Debug.LogError("Asset is not shader");
            return false;
        }

        Shader shader = (Shader)AssetDatabase.LoadAssetAtPath(path, typeof(Shader));
        if (shader == null)
        {
            Debug.LogError("Asset is not shader");
            return false;
        }


        if (ShaderUtil.ShaderHasError(shader))
        {
            Debug.LogError("Shader has errors");
            return false;
        }


        bool hasMainTex = false;
        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
        {
            if (ShaderUtil.GetPropertyName(shader, i) == "_MainTex")
                hasMainTex = true;
        }
        if (hasMainTex == false)
        {
            Debug.LogError("Shader does not use '_MainTex'");
            return false;
        }


        return true;
    }

    static void Integrate(Object sourceShaderAsset)
    {
        if (IsAssetReady(sourceShaderAsset) == false)
            return;

        string urpShadersFolder = GetPathToURPShadersFolder();
        if (string.IsNullOrEmpty(urpShadersFolder))
            return;


        string sourceShaderAssetPath = AssetDatabase.GetAssetPath(sourceShaderAsset);
        Shader sourceShaderFile = (Shader)AssetDatabase.LoadAssetAtPath(sourceShaderAssetPath, typeof(Shader));

        List<string> newShaderFile = File.ReadAllLines(sourceShaderAssetPath).ToList();

        //1) Change shader Name
        //2) Add properties
        //3) Add custom editor
        //4) Add #includes
        //5) Create new shader file asset


        //1
        ChangeShaderName(sourceShaderAssetPath, newShaderFile);

        //2
        AddProperties(sourceShaderFile, newShaderFile);

        //3
        AddCustomEditor(newShaderFile);

        //4
        AddDefines(urpShadersFolder, newShaderFile);
        
        //5
        CreateShaderAssetFile(sourceShaderAssetPath, newShaderFile);

    }




    static string GetPathToURPShadersFolder()
    {
        string[] assets = AssetDatabase.FindAssets("PBRForwardPass", null);
        if (assets != null && assets.Length > 0)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                if (string.IsNullOrEmpty(assets[i]) == false)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(assets[i]);

                    if (filePath.Contains("Advanced Dissolve") &&
                        filePath.Contains("Shaders") &&
                        filePath.Contains("URP") &&
                        filePath.Contains("PBRForwardPass.hlsl"))
                    {
                        return Path.GetDirectoryName(filePath);
                    }
                }
            }
        }

        return string.Empty;
    }

    static void ChangeShaderName(string sourceShaderAssetPath, List<string> newShaderFile)
    {
        //Get source shader name
        string originalName = Path.GetFileNameWithoutExtension(sourceShaderAssetPath);


        //Shader "name"         <-- find this line and set new shader name
        //{
        //      Properties
        //      {
        //  ...
        //  ...
        //  ...
        //      } 


        for (int i = 0; i < newShaderFile.Count; i++)
        {
            if (newShaderFile[i].Contains("Shader \""))
            {
                newShaderFile[i] = "Shader \"" + "Shader Graphs\\" + originalName + "\"";
            }
        }
    }

    static void AddProperties(Shader shader, List<string> newShaderFile)
    {
        //Properties
        //      {         <-- find this line ID and add Dissolve properties below it
        //  ...
        //  ...
        //  ...
        //      }        
        //SubShader
        //{



        int propertiesLineID = -1;

        for (int i = 0; i < newShaderFile.Count; i++)
        {
            if (newShaderFile[i].Trim() == "Properties")
            {
                propertiesLineID = i + 1;

                break;
            }
        }

        if (propertiesLineID == -1)
            return;


        List<string> properties = new List<string>();
        properties.Add(System.Environment.NewLine);
        properties.Add("//Advanced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////");
        properties.Add("[HideInInspector] _DissolveCutoff(\"Dissolve\", Range(0,1)) = 0.25");
        properties.Add(System.Environment.NewLine);
        properties.Add("//Mask");
        properties.Add("[HideInInspector] [KeywordEnum(None, XYZ Axis, Plane, Sphere, Box, Cylinder, Cone)] _DissolveMask(\"Mak\", Float) = 0");
        properties.Add("[HideInInspector] [Enum(X, 0, Y, 1, Z, 2)] _DissolveMaskAxis(\"Axis\", Float) = 0");
        properties.Add("[HideInInspector] [Enum(World, 0, Local, 1)] _DissolveMaskSpace(\"Space\", Float) = 0");
        properties.Add("[HideInInspector] _DissolveMaskOffset(\"Offset\", Float) = 0");
        properties.Add("[HideInInspector] _DissolveMaskInvert(\"Invert\", Float) = 1");
        properties.Add("[HideInInspector] [KeywordEnum(One, Two, Three, Four)] _DissolveMaskCount(\"Count\", Float) = 0");
        properties.Add(System.Environment.NewLine);
        properties.Add("[HideInInspector] _DissolveMaskPosition(\"\", Vector) = (0,0,0,0)");
        properties.Add("[HideInInspector] _DissolveMaskNormal(\"\", Vector) = (1,0,0,0)");
        properties.Add("[HideInInspector] _DissolveMaskRadius(\"\", Float) = 1");
        properties.Add(System.Environment.NewLine);
        properties.Add("//Alpha Source");
        properties.Add("[HideInInspector] [KeywordEnum(Main Map Alpha, Custom Map, Two Custom Maps, Three Custom Maps)] _DissolveAlphaSource(\"Alpha Source\", Float) = 0");
        properties.Add("[HideInInspector] _DissolveMap1(\"\", 2D) = \"white\" { }");
        properties.Add("[HideInInspector] [UVScroll] _DissolveMap1_Scroll(\"\", Vector) = (0,0,0,0)");
        properties.Add("[HideInInspector] _DissolveMap1Intensity(\"\", Range(0, 1)) = 1");
        properties.Add("[HideInInspector] [Enum(Red, 0, Green, 1, Blue, 2, Alpha, 3)] _DissolveMap1Channel(\"\", INT) = 3");
        properties.Add("[HideInInspector] _DissolveMap2(\"\", 2D) = \"white\" { }");
        properties.Add("[HideInInspector] [UVScroll] _DissolveMap2_Scroll(\"\", Vector) = (0,0,0,0)");
        properties.Add("[HideInInspector] _DissolveMap2Intensity(\"\", Range(0, 1)) = 1");
        properties.Add("[HideInInspector] [Enum(Red, 0, Green, 1, Blue, 2, Alpha, 3)] _DissolveMap2Channel(\"\", INT) = 3");
        properties.Add("[HideInInspector] _DissolveMap3(\"\", 2D) = \"white\" { }");
        properties.Add("[HideInInspector] [UVScroll] _DissolveMap3_Scroll(\"\", Vector) = (0,0,0,0)");
        properties.Add("[HideInInspector] _DissolveMap3Intensity(\"\", Range(0, 1)) = 1");
        properties.Add("[HideInInspector] [Enum(Red, 0, Green, 1, Blue, 2, Alpha, 3)] _DissolveMap3Channel(\"\", INT) = 3");
        properties.Add(System.Environment.NewLine);
        properties.Add("[HideInInspector] [Enum(Multiply, 0, Add, 1)] _DissolveSourceAlphaTexturesBlend(\"Texture Blend\", Float) = 0");
        properties.Add("[HideInInspector] _DissolveNoiseStrength(\"Noise\", Float) = 0.1");
        properties.Add("[HideInInspector] [Enum(UV0, 0, UV1, 1)] _DissolveAlphaSourceTexturesUVSet(\"UV Set\", Float) = 0");
        properties.Add("[HideInInspector] [Toggle] 			     _DissolveCombineWithMasterNodeAlpha(\"\", Float) = 0");
        properties.Add(System.Environment.NewLine);
        properties.Add("[HideInInspector] [KeywordEnum(Normal, Triplanar, Screen Space)] _DissolveMappingType(\"Triplanar\", Float) = 0");
        properties.Add("[HideInInspector] [Enum(World, 0, Local, 1)] _DissolveTriplanarMappingSpace(\"Mapping\", Float) = 0");
        properties.Add("[HideInInspector] _DissolveMainMapTiling(\"\", FLOAT) = 1");
        properties.Add(System.Environment.NewLine);
        properties.Add("//Edge");
        properties.Add("[HideInInspector] _DissolveEdgeWidth(\"Edge Size\", Range(0,1)) = 0.15");
        properties.Add("[HideInInspector] [Enum(Cutout Source, 0, Main Map, 1)] _DissolveEdgeDistortionSource(\"Distortion Source\", Float) = 0");
        properties.Add("[HideInInspector] _DissolveEdgeDistortionStrength(\"Distortion Strength\", Range(0, 2)) = 0");
        properties.Add(System.Environment.NewLine);
        properties.Add("//Color");
        properties.Add("[HideInInspector] _DissolveEdgeColor(\"Edge Color\", Color) = (0,1,0,1)");
        properties.Add("[HideInInspector] [PositiveFloat] _DissolveEdgeColorIntensity(\"Intensity\", FLOAT) = 0");
        properties.Add("[HideInInspector] [Enum(Solid, 0, Smooth, 1, Smooth Squared, 2)] _DissolveEdgeShape(\"Shape\", INT) = 0");
        properties.Add("[HideInInspector] [Toggle] 			                             _DissolveCombineWithMasterNodeColor(\"\", Float) = 0");
        properties.Add(System.Environment.NewLine);
        properties.Add("[HideInInspector] [KeywordEnum(None, Gradient, Main Map, Custom)] _DissolveEdgeTextureSource(\"\", Float) = 0");
        properties.Add("[HideInInspector] [NoScaleOffset] _DissolveEdgeTexture(\"Edge Texture\", 2D) = \"white\" { }");
        properties.Add("[HideInInspector] [Toggle] _DissolveEdgeTextureReverse(\"Reverse\", FLOAT) = 0");
        properties.Add("[HideInInspector] _DissolveEdgeTexturePhaseOffset(\"Offset\", FLOAT) = 0");
        properties.Add("[HideInInspector] _DissolveEdgeTextureAlphaOffset(\"Offset\", Range(-1, 1)) = 0");
        properties.Add("[HideInInspector] _DissolveEdgeTextureMipmap(\"\", Range(0, 10)) = 1");
        properties.Add("[HideInInspector] [Toggle] _DissolveEdgeTextureIsDynamic(\"\", Float) = 0");
        properties.Add(System.Environment.NewLine);
        properties.Add("[HideInInspector] [PositiveFloat] _DissolveGIMultiplier(\"GI Strength\", Float) = 1");
        properties.Add(System.Environment.NewLine);
        properties.Add("//Global");
        properties.Add("[HideInInspector] [KeywordEnum(None, Mask Only, Mask And Edge, All)] _DissolveGlobalControl(\"Global Controll\", Float) = 0");
        properties.Add(System.Environment.NewLine);
        properties.Add("//Meta");
        properties.Add("[HideInInspector] _Dissolve_ObjectWorldPos(\"\", Vector) = (0,0,0,0)");
        properties.Add("//Advanced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////");
        properties.Add(System.Environment.NewLine);


        newShaderFile.InsertRange(propertiesLineID + 1, properties);

    }

    static void AddCustomEditor(List<string> newShaderFile)
    {

        //Find defult "CustomEditor" and replace it
        //Loop with 10 iterations is enough to find "CustomEditor"

        int customEditroLineID = -1;
        for (int i = newShaderFile.Count - 1; i >= newShaderFile.Count - 10; i -= 1)
        {
            if (newShaderFile[i].Contains("CustomEditor"))
            {
                customEditroLineID = i;
                break;
            }
        }

        if (customEditroLineID != -1)
        {
            newShaderFile[customEditroLineID] = newShaderFile[customEditroLineID].Replace("CustomEditor", "//CustomEditor");
            newShaderFile.Insert(customEditroLineID + 1, "    CustomEditor \"VacuumShaders.AdvancedDissolve.DefaultShaderGUI\"");
        }

        //No "CustomEditor" detected
        else
        {
            //Find the end of shader file
            for (int i = newShaderFile.Count - 1; i >= newShaderFile.Count - 10; i -= 1)
            {
                if (newShaderFile[i].Trim() == "}")
                {
                    newShaderFile.Insert(i, "    CustomEditor \"VacuumShaders.AdvancedDissolve.DefaultShaderGUI\"");
                    break;
                }
            }
        }
    }

    static void AddDefines(string pathToURPShadersFolder, List<string> newShaderFile)
    {
        for (int i = 0; i < newShaderFile.Count; i++)
        {
            if (newShaderFile[i].Contains("PBRForwardPass.hlsl") ||
                newShaderFile[i].Contains("ShadowCasterPass.hlsl") ||
                newShaderFile[i].Contains("DepthOnlyPass.hlsl") ||
                newShaderFile[i].Contains("LightingMetaPass.hlsl") ||
                newShaderFile[i].Contains("UnlitPass"))
            {

                string includeFileName = string.Empty;
                if (newShaderFile[i].Contains("PBRForwardPass.hlsl"))
                    includeFileName = "PBRForwardPass.hlsl";
                else if (newShaderFile[i].Contains("ShadowCasterPass.hlsl"))
                    includeFileName = "ShadowCasterPass.hlsl";
                else if (newShaderFile[i].Contains("DepthOnlyPass.hlsl"))
                    includeFileName = "DepthOnlyPass.hlsl";
                else if (newShaderFile[i].Contains("LightingMetaPass.hlsl"))
                    includeFileName = "LightingMetaPass.hlsl";
                else if (newShaderFile[i].Contains("UnlitPass.hlsl"))
                    includeFileName = "UnlitPass.hlsl";


                newShaderFile[i] = string.Empty;

                List<string> defines = new List<string>();
                defines.Add(System.Environment.NewLine);
                defines.Add("//Advnaced Dissolve keywords/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////");
                defines.Add("#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL");
                defines.Add("#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE");
                defines.Add("#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS");
                defines.Add("#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE");
                defines.Add("#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM");
                defines.Add("#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR");
                defines.Add(System.Environment.NewLine);
                defines.Add("#define DISSOLVE_SHADER_GRAPH");
                defines.Add("#include \"" + pathToURPShadersFolder.Replace(Path.DirectorySeparatorChar, '/') + ("/" + includeFileName) + "\"");
                defines.Add("//Advnaced Dissolve keywords/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////");
                defines.Add(System.Environment.NewLine);
                defines.Add(System.Environment.NewLine);

                newShaderFile.InsertRange(i, defines);

                i += defines.Count;
            }
        }
    }
    
    static void CreateShaderAssetFile(string sourceShaderAssetPath, List<string> newShaderFile)
    {
        File.WriteAllLines(sourceShaderAssetPath, newShaderFile);

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

}
