Shader "VacuumShaders/Advanced Dissolve/Universal Render Pipeline/Particles/Unlit"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _BumpMap("Normal Map", 2D) = "bump" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        // -------------------------------------
        // Particle specific
        _SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
        _SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
        _CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
        _CameraFarFadeDistance("Camera Far Fade", Float) = 2.0
        _DistortionBlend("Distortion Blend", Float) = 0.5
        _DistortionStrength("Distortion Strength", Float) = 1.0

        // -------------------------------------
        // Hidden properties - Generic
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__mode", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _BlendOp("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        // Particle specific
        [HideInInspector] _ColorMode("_ColorMode", Float) = 0.0
        [HideInInspector] _BaseColorAddSubDiff("_ColorMode", Vector) = (0,0,0,0)
        [ToggleOff] _FlipbookBlending("__flipbookblending", Float) = 0.0
        [HideInInspector] _SoftParticlesEnabled("__softparticlesenabled", Float) = 0.0
        [HideInInspector] _CameraFadingEnabled("__camerafadingenabled", Float) = 0.0
        [HideInInspector] _SoftParticleFadeParams("__softparticlefadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _CameraFadeParams("__camerafadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _DistortionEnabled("__distortionenabled", Float) = 0.0
        [HideInInspector] _DistortionStrengthScaled("Distortion Strength Scaled", Float) = 0.1
        
        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
        
        // ObsoleteProperties
        [HideInInspector] _FlipbookMode("flipbook", Float) = 0
        [HideInInspector] _Mode("mode", Float) = 0
        [HideInInspector] _Color("color", Color) = (1,1,1,1)





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

    SubShader
    {
        Tags{"RenderType" = "Opaque" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" "RenderPipeline" = "UniversalPipeline"}

        // ------------------------------------------------------------------
        //  Forward pass.
        Pass
        {
            // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Universal Render Pipeline
            Name "ForwardLit"

            BlendOp[_BlendOp]
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]
            ColorMask RGB

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _EMISSION

            // -------------------------------------
            // Particle Keywords
            #pragma shader_feature _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ _COLOROVERLAY_ON _COLORCOLOR_ON _COLORADDSUBDIFF_ON
            #pragma shader_feature _FLIPBOOKBLENDING_ON
            #pragma shader_feature _SOFTPARTICLES_ON
            #pragma shader_feature _FADING_ON
            #pragma shader_feature _DISTORTION_ON

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog

            #pragma vertex vertParticleUnlit
            #pragma fragment fragParticleUnlit
 

            // -------------------------------------
			// Advnaced Dissolve keywords
			#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL
			#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
			#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE
			#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM
			#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR

            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Particles/ParticlesUnlitInput.hlsl"
            #include "../cginc/AdvancedDissolve.cginc"
            #include "ParticlesUnlitForwardPass.hlsl"
            
            ENDHLSL
        } 
    }
	Fallback "Universal Render Pipeline/Particles/Unlit"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.AdvancedDisolve_ParticlesUnlitShader"
}
