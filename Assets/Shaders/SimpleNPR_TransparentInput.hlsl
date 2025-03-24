#ifndef SIMPLENPR_TRANSPARENT_INPUT
#define SIMPLENPR_TRANSPARENT_INPUT
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

CBUFFER_START(UnityPerMaterial)
// float4 _BaseMap_ST;
// half4 _BaseColor;
half _Cutoff;

half4 _MainColor;
half _MainTexA;
float4 _MainTex_ST;
// half4 _ShadowColor;
// half4 _TilingAndOffset;
half _ShadowIntensity;
half _RampIndex;
float2 _ShadowSmooth;

half _NormalScale;
float4 _NormalMap_ST;
half3 _SpecularColor;
half _Specular1,_AddSpecular,_SpecularInShadow;
half3 _ReflectionCol;
float4 _ReflectionSetting;
float4 _RimCol;
float _RimWidth,_Threshold;
half _ReflectionIntensity;

float4 _EmissiveTex_ST;
half3 _EmissiveColor;
float2 _LambertSmooth;

//half _NoiseScale,_NoiseScale2;
// float _UseWSUpMask;
// float4 _UpColorMask;

// half _GlobeNoiseScale;
// half _GlobeNoiseScale2;
// half _AdditionLightNoiseScale;
// half _AdditionLightNoiseStep;
// half _AdditionLightIntensity;
            
half _HalftoneEffect;
half _HalftoneStep;
half3 _HalftoneColor;
half _HalftoneScale;
half _HalftoneUVZScale;
half2 _EdgePowScale;

half _UseVertexColorRGB;
half _UseVertexColorA;

//outline
int _IsTangentSpace;
half3 _OutlineCol;
half _OutlineWidth;
half _ViewSpaceZOffset;

CBUFFER_END

TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
TEXTURE2D(_ShadowColorTex); SAMPLER(sampler_ShadowColorTex);
TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
// TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
// TEXTURE2D(_NoiseTex2); SAMPLER(sampler_NoiseTex2);
TEXTURE2D(_EmissiveTex); SAMPLER(sampler_EmissiveTex);
// TEXTURE2D(_GlobeNoiseTex);SAMPLER(sampler_GlobeNoiseTex);
// TEXTURE2D(_GlobeNoiseTex2);SAMPLER(sampler_GlobeNoiseTex2);
// TEXTURE2D(_LightNoiseTex);SAMPLER(sampler_LightNoiseTex);
TEXTURE2D(_RampShadowTex);
//SAMPLER(sampler_LinearClamp);


//粒子系统中模型坐标
float4 _objectZeroPositionWS_Particle;
float _IsParticle;
#endif