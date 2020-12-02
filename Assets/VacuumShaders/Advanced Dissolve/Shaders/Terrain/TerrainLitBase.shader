Shader "Hidden/VacuumShaders/Advanced Dissolve/Universal Render Pipeline/Terrain/Lit (Base Pass)"
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _MainTex("Albedo(RGB), Smoothness(A)", 2D) = "white" {}
        _MetallicTex ("Metallic (R)", 2D) = "black" {}
        [HideInInspector] _TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" {}



        //Advanced Dissolve
		_DissolveCutoff("Dissolve", Range(0,1)) = 0.25
		
		//Mask
		[HideInInspector][KeywordEnum(None, XYZ Axis, Plane, Sphere, Box, Cylinder, Cone)]  _DissolveMask("Mak", Float) = 0
		[HideInInspector][Enum(X,0,Y,1,Z,2)]                                                _DissolveMaskAxis("Axis", Float) = 0
		[HideInInspector][Enum(World,0,Local,1)]                                            _DissolveMaskSpace("Space", Float) = 0	 
		[HideInInspector]																   _DissolveMaskOffset("Offset", Float) = 0
		[HideInInspector]																   _DissolveMaskInvert("Invert", Float) = 1		
		[HideInInspector][KeywordEnum(One, Two, Three, Four)]							   _DissolveMaskCount("Count", Float) = 0		
	
		[HideInInspector]  _DissolveMaskPosition("", Vector) = (0,0,0,0)
		[HideInInspector]  _DissolveMaskNormal("", Vector) = (1,0,0,0)
		[HideInInspector]  _DissolveMaskRadius("", Float) = 1

		//Alpha Source
		[HideInInspector] [KeywordEnum(Main Map Alpha, Custom Map, Two Custom Maps, Three Custom Maps)] _DissolveAlphaSource("Alpha Source", Float) = 0
		[HideInInspector] _DissolveMap1("", 2D) = "white" { }
		[HideInInspector] [UVScroll] _DissolveMap1_Scroll("", Vector) = (0,0,0,0)
		[HideInInspector] _DissolveMap1Intensity("", Range(0, 1)) = 1
		[HideInInspector] [Enum(Red, 0, Green, 1, Blue, 2, Alpha, 3)] _DissolveMap1Channel("", INT) = 3
		[HideInInspector] _DissolveMap2("", 2D) = "white" { }
		[HideInInspector] [UVScroll] _DissolveMap2_Scroll("", Vector) = (0,0,0,0)
		[HideInInspector] _DissolveMap2Intensity("", Range(0, 1)) = 1
	    [HideInInspector] [Enum(Red, 0, Green, 1, Blue, 2, Alpha, 3)] _DissolveMap2Channel("", INT) = 3
		[HideInInspector] _DissolveMap3("", 2D) = "white" { }
		[HideInInspector] [UVScroll] _DissolveMap3_Scroll("", Vector) = (0,0,0,0)
		[HideInInspector] _DissolveMap3Intensity("", Range(0, 1)) = 1
	    [HideInInspector] [Enum(Red, 0, Green, 1, Blue, 2, Alpha, 3)] _DissolveMap3Channel("", INT) = 3


		[HideInInspector] [Enum(Multiply, 0, Add, 1)] _DissolveSourceAlphaTexturesBlend("Texture Blend", Float) = 0
		[HideInInspector] _DissolveNoiseStrength("Noise", Float) = 0.1
		[HideInInspector] [Enum(UV0, 0, UV1, 1)] _DissolveAlphaSourceTexturesUVSet("UV Set", Float) = 0
		[HideInInspector] [Toggle] 			     _DissolveCombineWithMasterNodeAlpha("", Float) = 0


		[HideInInspector] [KeywordEnum(Normal, Triplanar, Screen Space)] _DissolveMappingType("Triplanar", Float) = 0
		[HideInInspector] [Enum(World, 0, Local, 1)] _DissolveTriplanarMappingSpace("Mapping", Float) = 0
		[HideInInspector] _DissolveMainMapTiling("", FLOAT) = 1


		//Edge
		[HideInInspector] _DissolveEdgeWidth("Edge Size", Range(0,1)) = 0.15
		[HideInInspector] [Enum(Cutout Source, 0, Main Map, 1)] _DissolveEdgeDistortionSource("Distortion Source", Float) = 0
		[HideInInspector] _DissolveEdgeDistortionStrength("Distortion Strength", Range(0, 2)) = 0


		//Color
		[HideInInspector] _DissolveEdgeColor("Edge Color", Color) = (0,1,0,1)
		[HideInInspector] [PositiveFloat] _DissolveEdgeColorIntensity("Intensity", FLOAT) = 0
		[HideInInspector] [Enum(Solid, 0, Smooth, 1, Smooth Squared, 2)] _DissolveEdgeShape("Shape", INT) = 0
		[HideInInspector] [Toggle] 			                             _DissolveCombineWithMasterNodeColor("", Float) = 0

		[HideInInspector][KeywordEnum(None, Gradient, Main Map, Custom)] _DissolveEdgeTextureSource("", Float) = 0
		[HideInInspector][NoScaleOffset]								 _DissolveEdgeTexture("Edge Texture", 2D) = "white" { }
		[HideInInspector][Toggle]										 _DissolveEdgeTextureReverse("Reverse", FLOAT) = 0
		[HideInInspector]												 _DissolveEdgeTexturePhaseOffset("Offset", FLOAT) = 0
		[HideInInspector]												 _DissolveEdgeTextureAlphaOffset("Offset", Range(-1, 1)) = 0
		[HideInInspector]												 _DissolveEdgeTextureMipmap("", Range(0, 10)) = 1		
		[HideInInspector][Toggle]										 _DissolveEdgeTextureIsDynamic("", Float) = 0

		[HideInInspector][PositiveFloat] _DissolveGIMultiplier("GI Strength", Float) = 1	
		
		//Global
		[HideInInspector][KeywordEnum(None, Mask Only, Mask And Edge, All)] _DissolveGlobalControl("Global Controll", Float) = 0

		//Meta
		[HideInInspector] _Dissolve_ObjectWorldPos("", Vector) = (0,0,0,0)	
    }

    HLSLINCLUDE

    #pragma multi_compile __ _ALPHATEST_ON

    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Geometry-100" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 200

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            Name "ForwardLit"
            // Lightmode matches the ShaderPassName set in UniversalPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Pipeline
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #define _METALLICSPECGLOSSMAP 1
            #define _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A 1

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

            #pragma vertex SplatmapVert
            #pragma fragment SplatmapFragment

            #pragma shader_feature_local _NORMALMAP
            // Sample normal in pixel shader when doing instancing
            #pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
            #define TERRAIN_SPLAT_BASEPASS 1

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitInput.hlsl"


            // Advnaced Dissolve keywords
			#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL
			#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
			#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE
			#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM
			#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR
            #include "../cginc/AdvancedDissolve.cginc"

            #include "TerrainLitPasses.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitInput.hlsl"


            // Advnaced Dissolve keywords
			#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL
			#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
			#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE
			#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM
			#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR
            #include "../cginc/AdvancedDissolve.cginc"

            #include "TerrainLitPasses.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitInput.hlsl"


            // Advnaced Dissolve keywords
			#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL
			#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
			#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE
			#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM
			#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR
            #include "../cginc/AdvancedDissolve.cginc"

            #include "TerrainLitPasses.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex TerrainVertexMeta
            #pragma fragment TerrainFragmentMeta

            #define _METALLICSPECGLOSSMAP 1
            #define _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A 1

            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitMetaPass.hlsl"

            ENDHLSL
        }

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Universal Render Pipeline/Terrain/Lit/SceneSelectionPass"
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    //CustomEditor "LitShaderGUI"
}
