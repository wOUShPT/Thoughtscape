#include "../cginc/AdvancedDissolve.cginc"

PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = PackVaryings(output);
    return packedOutput;
}

half4 frag(PackedVaryings packedInput) : SV_TARGET 
{    
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);


 //Advnaced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 float4 dissolaveAlpha = AdvancedDissolveGetAlpha(unpacked.texCoord0.xy, unpacked.texCoord1.xy, unpacked.positionWS.xyz, unpacked.normalWS.xyz, surfaceDescription.Alpha);
 DoDissolveClip(dissolaveAlpha); 


 float3 dissolveAlbedo = 0;
 float3 dissolveEmission = 0;
 float dissolveBlend = DoDissolveAlbedoEmission(dissolaveAlpha, dissolveAlbedo, dissolveEmission, unpacked.texCoord1.xy, surfaceDescription.Color);


 surfaceDescription.Color = lerp(surfaceDescription.Color, dissolveAlbedo, dissolveBlend);
 surfaceDescription.Color = lerp(surfaceDescription.Color, dissolveEmission, dissolveBlend);
//Advnaced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#if _AlphaClip
    clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
#endif

#ifdef _ALPHAPREMULTIPLY_ON
    surfaceDescription.Color *= surfaceDescription.Alpha;
#endif

    return half4(surfaceDescription.Color, surfaceDescription.Alpha);
}
