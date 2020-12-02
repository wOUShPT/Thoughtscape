#ifndef SG_LIT_META_INCLUDED
#define SG_LIT_META_INCLUDED


#include "../cginc/AdvancedDissolve.cginc"


PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = (PackedVaryings)0;
    packedOutput = PackVaryings(output);
    return packedOutput;
}

half4 frag(PackedVaryings packedInput) : SV_TARGET 
{    
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);


 //Advnaced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  float4 dissolaveAlpha = AdvancedDissolveGetAlpha(unpacked.texCoord0.xy, unpacked.texCoord1.xy, unpacked.positionWS.xyz, unpacked.normalWS.xyz, surfaceDescription.Alpha);
//  DoDissolveClip(dissolaveAlpha); 


//  float3 dissolveAlbedo = 0;
//  float3 dissolveEmission = 0;
//  float dissolveBlend = DoDissolveAlbedoEmission(dissolaveAlpha, dissolveAlbedo, dissolveEmission, unpacked.texCoord1.xy, surfaceDescription.Albedo);


//  surfaceDescription.Albedo = lerp(surfaceDescription.Albedo, dissolveAlbedo, dissolveBlend);
//  surfaceDescription.Emission = lerp(surfaceDescription.Emission, dissolveEmission, dissolveBlend);
//Advnaced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    #if _AlphaClip
        clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
    #endif

    MetaInput metaInput = (MetaInput)0;
    metaInput.Albedo = surfaceDescription.Albedo;
    metaInput.Emission = surfaceDescription.Emission;

    return MetaFragment(metaInput);
}

#endif
