Shader "VacuumShaders/Advanced Dissolve/Universal Render Pipeline/TextMeshPro/Sprite"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		_ClipRect ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0




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
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP


			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;


			// -------------------------------------
			// Advnaced Dissolve keywords
			#pragma shader_feature_local _ _DISSOLVEGLOBALCONTROL_MASK_ONLY _DISSOLVEGLOBALCONTROL_MASK_AND_EDGE _DISSOLVEGLOBALCONTROL_ALL
			#pragma shader_feature_local _ _DISSOLVEMAPPINGTYPE_TRIPLANAR _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			#pragma shader_feature_local _ _DISSOLVEALPHASOURCE_CUSTOM_MAP _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
			#pragma shader_feature_local _ _DISSOLVEMASK_XYZ_AXIS _DISSOLVEMASK_PLANE _DISSOLVEMASK_SPHERE _DISSOLVEMASK_BOX _DISSOLVEMASK_CYLINDER _DISSOLVEMASK_CONE
			#pragma shader_feature_local _ _DISSOLVEEDGETEXTURESOURCE_GRADIENT _DISSOLVEEDGETEXTURESOURCE_MAIN_MAP _DISSOLVEEDGETEXTURESOURCE_CUSTOM
			#pragma shader_feature_local _ _DISSOLVEMASKCOUNT_TWO _DISSOLVEMASKCOUNT_THREE _DISSOLVEMASKCOUNT_FOUR

			#define DISSOLVE_LEGACY_TEXTURE_SAMPLE
			#define DISSOLVE_LEGACY_MAINTEX

			#ifndef UNITY_UI_ALPHACLIP
			#define UNITY_UI_ALPHACLIP 1
			#endif
			#include "../cginc/AdvancedDissolve.cginc"


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;


				float3 positionWS    : TEXCOORD3;
				float3 normalWS      : TEXCOORD4;

				ADVANCED_DISSOLVE_DATA(5)
			};
			
			

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif
				
				OUT.color = IN.color * _Color;


				OUT.positionWS = mul(unity_ObjectToWorld, float4(IN.vertex.xyz, 1)).xyz;
				OUT.normalWS = UnityObjectToWorldNormal(IN.normal);

				ADVANCED_DISSOLVE_INIT_DATA(OUT.vertex, IN.texcoord.xy, IN.texcoord2.xy, OUT);

				return OUT;
			}

			

			fixed4 frag(v2f IN) : SV_Target
			{
				float4 dissolaveAlpha = AdvancedDissolveGetAlpha(IN.texcoord.xy, IN.positionWS.xyz, IN.normalWS, IN.dissolveUV);
				DoDissolveClip(dissolaveAlpha); 

				float3 dissolveAlbedo = 0;
				float3 dissolveEmission = 0;
				float dissolveBlend = DoDissolveAlbedoEmission(dissolaveAlpha, dissolveAlbedo, dissolveEmission, IN.texcoord.xy, 1);



				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				
				#if UNITY_UI_CLIP_RECT
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
					clip (color.a - 0.001);
				#endif


				//Diffuse
				color.rgb = lerp(color.rgb, dissolveAlbedo, dissolveBlend);

				//Emission
				color.rgb += dissolveEmission * dissolveBlend;

				return color;
			}
		ENDCG
		}
	}

	Fallback "TextMeshPro/Sprite"
	CustomEditor "VacuumShaders.AdvancedDissolve.TMP_SpriteGUI"
}
