#ifndef UNIVERSAL_DEPTH_ONLY_PASS_INCLUDED
#define UNIVERSAL_DEPTH_ONLY_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 position     : POSITION;
    float2 texcoord     : TEXCOORD0;
    float3 normal       : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv           : TEXCOORD0;
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO


    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    ADVANCED_DISSOLVE_DATA(3)
};

Varyings DepthOnlyVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.positionCS = TransformObjectToHClip(input.position.xyz);



    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.position.xyz);
    output.positionWS = vertexInput.positionWS.xyz;
    output.normalWS = input.normal;

    ADVANCED_DISSOLVE_INIT_DATA(vertexInput.positionCS, input.texcoord.xy, input.texcoord.xy, output)


    return output;
}

half4 DepthOnlyFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    float4 dissolaveAlpha = AdvancedDissolveGetAlpha(input.uv.xy, input.positionWS.xyz, input.normalWS, input.dissolveUV);
    DoDissolveClip(dissolaveAlpha); 

    Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
    return 0;
}
#endif
