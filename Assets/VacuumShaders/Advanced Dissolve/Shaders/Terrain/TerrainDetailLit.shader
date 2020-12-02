Shader "VacuumShaders/Advanced Dissolve/Universal Render Pipeline/Nature/Grass Detail Lit"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {  }


        //Advanced Dissolve
		[HideInInspector] _DissolveCutoff("Dissolve", Range(0,1)) = 0.25
		
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
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 100

        ZWrite On

        // Lightmapped
        Pass
        {
            Name "TerrainDetailVertex"
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex Vert
            #pragma fragment Frag

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

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            TEXTURE2D(_BaseMap);       SAMPLER(sampler_BaseMap);

            // Advnaced Dissolve keywords
			#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL
			#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
			#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE
			#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM
			#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR
            #include "../cginc/AdvancedDissolve.cginc"

 

            TEXTURE2D(_MainTex);       SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            

            struct Attributes
            {
                float4  PositionOS  : POSITION;
                float2  UV0         : TEXCOORD0;
                float2  UV1         : TEXCOORD1;
                float3  NormalOS    : NORMAL;
                half4   Color       : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2  UV01            : TEXCOORD0; // UV0
                float2  LightmapUV      : TEXCOORD1; // Lightmap UVs
                half4   Color           : TEXCOORD2; // Vertex Color
                half4   LightingFog     : TEXCOORD3; // Vetex Lighting, Fog Factor
#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                float4  ShadowCoords    : TEXCOORD4; // Shadow UVs
#endif
                float4  PositionCS      : SV_POSITION; // Clip Position
                UNITY_VERTEX_OUTPUT_STEREO


                float3 positionWS : TEXCOORD5;
                float3 normalWS : TEXCOORD6;
                ADVANCED_DISSOLVE_DATA(7)
            };

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Vertex attributes
                output.UV01 = TRANSFORM_TEX(input.UV0, _MainTex);
                output.LightmapUV = input.UV1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.PositionOS.xyz);
                output.Color = input.Color;
                output.PositionCS = vertexInput.positionCS;

                // Shadow Coords
                #if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    output.ShadowCoords = GetShadowCoord(vertexInput);
                #endif

                // Vertex Lighting
                half3 NormalWS = input.NormalOS;
                Light mainLight = GetMainLight();
                half3 attenuatedLightColor = mainLight.color * mainLight.distanceAttenuation;
                half3 diffuseColor = LightingLambert(attenuatedLightColor, mainLight.direction, NormalWS);

                #ifdef _ADDITIONAL_LIGHTS
                    int pixelLightCount = GetAdditionalLightsCount();
                    for (int i = 0; i < pixelLightCount; ++i)
                    {
                        Light light = GetAdditionalLight(i, vertexInput.positionWS);
                        half3 attenuatedLightColor = light.color * light.distanceAttenuation;
                        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, NormalWS);
                    }
                #endif

                output.LightingFog.xyz = diffuseColor;

                // Fog factor
                output.LightingFog.w = ComputeFogFactor(output.PositionCS.z);


                output.positionWS = vertexInput.positionWS;
                output.normalWS = NormalWS;

                _BaseMap = _MainTex;
                sampler_BaseMap = sampler_MainTex; 

                ADVANCED_DISSOLVE_INIT_DATA(vertexInput.positionCS, input.UV0.xy, input.UV1.xy, output)

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);


float4 dissolaveAlpha = AdvancedDissolveGetAlpha(input.UV01.xy, input.positionWS.xyz, input.normalWS, input.dissolveUV);
DoDissolveClip(dissolaveAlpha); 

float3 dissolveAlbedo = 0;
float3 dissolveEmission = 0;
float dissolveBlend = DoDissolveAlbedoEmission(dissolaveAlpha, dissolveAlbedo, dissolveEmission, input.UV01.xy, 1);



                half3 bakedGI = SampleLightmap(input.LightmapUV, half3(0.0, 1.0, 0.0));

                #if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    half3 lighting = input.LightingFog.rgb * MainLightRealtimeShadow(input.ShadowCoords) + bakedGI;
                #else
                    half3 lighting = input.LightingFog.rgb + bakedGI;
                #endif

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.UV01);
                half4 color = 1.0;
                color.rgb = input.Color.rgb * tex.rgb;


                color.rgb = lerp(color.rgb, dissolveAlbedo, dissolveBlend);
                color.rgb *= lighting;

                color.rgb += lerp(0, dissolveEmission, dissolveBlend);

                color.rgb = MixFog(color.rgb, input.LightingFog.w);

                return color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Depth"
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

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"


            // Advnaced Dissolve keywords
			#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL
			#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
			#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE
			#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM
			#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR
            #include "../cginc/AdvancedDissolve.cginc"

            #include "DepthOnlyPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Meta"
            Tags{ "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaSimple

            #pragma shader_feature _SPECGLOSSMAP

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"

            ENDHLSL
        }
    }


    CustomEditor "UnityEditor.Rendering.Universal.DetailLit"

    //Fallback "VertexLit"
}
