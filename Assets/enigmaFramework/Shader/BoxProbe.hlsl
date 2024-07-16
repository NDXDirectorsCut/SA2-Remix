//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

void BoxProjection_float(float3 ViewDirWS, float3 NormalWS, float3 PositionWS, float LOD,out float3 Out)
{
    #ifdef SHADERGRAPH_PREVIEW
        Out = float3(1,1,1);
    #else
        float3 viewDirWS = normalize(ViewDirWS);
        float3 normalWS = normalize(NormalWS);
        float3 reflDir = normalize(reflect(-viewDirWS, normalWS));
        float3 factors = ((reflDir > 0 ? unity_SpecCube0_BoxMax.xyz : unity_SpecCube0_BoxMin.xyz) - PositionWS) / reflDir;
        float scalar = min(min(factors.x, factors.y), factors.z);
        float3 uvw = reflDir * scalar + (PositionWS - unity_SpecCube0_ProbePosition.xyz);
        float4 sampleRefl;
        
        #ifdef UNITY_SPECCUBE_BOX_PROJECTION
            sampleRefl = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, uvw, LOD);//SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, uvw, LOD);
        #else
            sampleRefl = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflDir, LOD);
        #endif

        float3 specCol = DecodeHDREnvironment(sampleRefl, unity_SpecCube0_HDR);
        Out = specCol;
    #endif
}

#endif