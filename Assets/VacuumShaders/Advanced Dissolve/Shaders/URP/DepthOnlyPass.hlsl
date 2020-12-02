#ifndef SG_DEPTH_ONLY_PASS_INCLUDED
#define SG_DEPTH_ONLY_PASS_INCLUDED


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
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);


 //Advnaced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 float4 dissolaveAlpha = AdvancedDissolveGetAlpha(unpacked.texCoord0.xy, unpacked.texCoord1.xy, unpacked.positionWS.xyz, unpacked.normalWS.xyz, surfaceDescription.Alpha);
 DoDissolveClip(dissolaveAlpha); 
 //Advnaced Dissolve/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

 

    #if _AlphaClip
        clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
    #endif

    return 0;
}

#endif
