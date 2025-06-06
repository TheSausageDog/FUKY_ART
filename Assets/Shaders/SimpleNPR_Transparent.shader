﻿Shader "ProjectCard/ToonLit/SceneToonLit_Transparent"
{
    Properties
    {
        [Main(Group1, _, off, off)] 
		_group1 ("主帖图设置", float) = 0
        [Sub(Group1)]   [HDR]_MainColor ("主颜色",Color) = (1,1,1,1)
        [Sub(Group1)]   _MainTex ("主贴图", 2D) = "white" {}
        [Sub(Group1)]   _MainTexA ("主贴图A通道强度",Range(0,5)) = 1
        [SubToggle(Group1,_ALPHA_CLIP_ON)] _AlphaClip("AlphaClip",float) = 0
        [Sub(Group1)]   _Cutoff("裁切",Range(-0.01,1.001)) = 0
        
        [Main(Group5, _, off, off)] 
		_group5 ("阴影设置", float) = 0
        [Sub(Group5)]   _ShadowIntensity("ShadowIntensity",Range(0,1)) = 1
        [Sub(Group5)]   [NoScaleOffset]_RampShadowTex("RampShadow",2d) = "white" {}
        [Sub(Group5)]   _RampIndex("RampIndex",Range(0.001,0.999)) = 0.001
        [Sub(Group5)]   _ShadowSmooth("XY:投影面软硬过渡",Vector) = (0,1,0,0)
        [Sub(Group5)]   _LambertSmooth("XY:暗面软硬过渡",Vector) = (0,1,0,0)
        
        [Main(Group2, _NORMALMAP,off , on)] 
		_group2 ("法线贴图", float) = 0
        [Sub(Group2)]   _NormalScale("法线强度",float) = 1
        [Sub(Group2)]   _NormalMap("法线贴图",2D) = "bump"{}
        
        [Main(Group3, _, off, off)] 
		_group3 ("高光设置", float) = 0
        [Sub(Group3)]   [HDR]_SpecularColor("高光颜色",Color) = (1,1,1,1)
        [Sub(Group3)]   _Specular1("高光1",Range(0,1)) = 0
        [Sub(Group3)]   _AddSpecular("附加光高光",Range(0,1)) = 0
        [Sub(Group3)]   _SpecularInShadow("暗面高光强度",Range(0,1)) = 0.1
        
        [Main(RimLight, _, off, off)] 
        _rimLight ("边缘光设置", float) = 0
        [Sub(RimLight)]    [HDR]_RimCol("_RimCol",Color) = (0,0,0,0)
        [Sub(RimLight)]    _RimWidth("_RimWidth", float) = 0.012
        [Sub(RimLight)]    _Threshold("_Threshold", float) = 0.09
        [Sub(RimLight)]    _ReflectionIntensity("_ReflectionIntensity",Range(0,1)) = 0
        
        [Main(ReflectionCol, _, off, off)] 
        _reflectionCol ("反射设置", float) = 0
        [Sub(ReflectionCol)]   _ReflectionCol("反射颜色",Color) = (0,0,0,0)
        [Sub(ReflectionCol)]   _ReflectionSetting("XY:边缘过渡 Z:反射粗糙度 W:反射颜色(F0)",Vector) = (5,1,0,0)
        
        [Main(Group4, _EMISSIVE, off, on)] 
		_group4 ("自发光", float) = 0
        [Sub(Group4)]   [HDR]_EmissiveColor("自发光颜色",Color) = (0,0,0,0)
        [Sub(Group4)]   _EmissiveTex("自发光贴图",2D) = "black"{}
        
        [Main(Group8, _EDGE_HALFTONE, off, on)] 
		_group8 ("半色调边缘光", float) = 0
        [Sub(Group8)]   _HalftoneEffect("整体强度",Range(0,1)) = 0
        [Sub(Group8)]   _HalftoneStep ("淡入淡出",Range(0.0001,1)) = 0.5
        [Sub(Group8)]   [HDR]_HalftoneColor("边缘颜色",Color) = (1,1,1,1)
        [Sub(Group8)]   _HalftoneScale("网格密度",float) = 10
        [SubToggle(Group8)]   _HalftoneUVZScale("网格密度不受远近影响",float) = 0
        [Sub(Group8)]   _EdgePowScale("XY:边缘过渡PowScale",Vector) = (1,1,0,0)
        
//        [Main(Group9, _, off, OFF)] 
//        _group9 ("描边", float) = 0
//        [SubEnum(Group9, ON, 15, OFF, 0)] _ColorMask ("描边", float) = 0
//        [KWEnum(Group9, Null,_, VertexCol, _SMOOTHNORMAL_VERTEX_COLOR,Tangent, _SMOOTHNORMAL_TANGENT, UV3, _SMOOTHNORMAL_TEXCOORD3)] _SmoothNormalData ("平滑法线数据", float) = 0
//        [SubToggle(Group9,_)] _IsTangentSpace("平滑法线在切线空间",Int) = 1
//        [Sub(Group9)]  [HDR]_OutlineCol ("描边颜色",Color) = (0,0,0,1)
//        [Sub(Group9)]   _OutlineWidth ("描边宽度",float) = -0.1
//        [SubToggle(Group9,_UNIFORM_OUTLINE_WIDTH)] _UniformOutline("描边宽度始终不变",float) = 0
//        [Sub(Group9)]   _ViewSpaceZOffset ("深度偏移",Range(0,20)) = 0

//        [Header(Custom Outline)]
//        [SubToggle(Group9)] _UseVertexColorRGB ("使用顶点色:RGB颜色",float) = 0
//        [SubToggle(Group9)] _UseVertexColorA ("使用顶点色:A描边宽度",float) = 0
        
        [Main(Render, _, off, off)] 
        _Render ("渲染设置", float) = 0
        [SubEnum(Render, UnityEngine.Rendering.CullMode)] _Cull ("Cull (Default back)", Float) = 2
        [SubEnum(Render,Blend,10,Add,1)] _BlendAAA("混合模式", Int) = 1
		[SubEnum(Render,Off,0,On,1)] _ZWrite0("深度写入", float) = 0
        [SubToggle(Render,_USESHADOW)]   _UseShadow ("接收投影", Float) = 1
        [Sub(Render)]   _StencilComp ("Stencil Comparison", Float) = 8
        [Sub(Render)]   _Stencil ("Stencil ID", Float) = 0
        [Sub(Render)]   _StencilOp ("Stencil Operation", Float) = 0
        [Sub(Render)]   _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [Sub(Render)]   _StencilReadMask ("Stencil Read Mask", Float) = 255
        
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"  }
        Pass
        {
            Name "SimpleNPR"
            Tags{"LightMode" = "UniversalForward"}
            ZWrite [_ZWrite0]
            Cull [_Cull]
            Blend SrcAlpha [_BlendAAA] 
            
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHA_CLIP_ON
            #pragma shader_feature_local_fragment _USESHADOW
            
            #pragma shader_feature_local_fragment _EMISSIVE
            #pragma shader_feature_local_fragment _NORMALMAP
            #pragma shader_feature_local_fragment _EDGE_HALFTONE
            
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            #include "../Shaders/SimpleNPR_TransparentInput.hlsl"

            struct Attributes
            {
                float4 color        :COLOR ;
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float2 staticLightmapUV   : TEXCOORD1;
                //float2 dynamicLightmapUV  : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 color                    : TEXCOORD10;
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 positionVS               : TEXCOORD6;
                float3 positionOS               : TEXCOORD11;
                float3 normalWS                 : TEXCOORD2;
                float3 normalVS                 : TEXCOORD12;
                float4 tangentWS                : TEXCOORD3;
                float3 viewDirWS                : TEXCOORD4;
                float3 bitangent                : TEXCOORD5;
                float4 shadowCoord              : TEXCOORD7;
                float3 staticLightmapUV         : TEXCOORD8;
                half  fogFactor                 : TEXCOORD9;
                float4 positionNDC              : TEXCOORD13;
                float4 positionCS : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.uv = TRANSFORM_TEX(input.texcoord,_MainTex);
                output.positionWS = vertexInput.positionWS;
                output.positionVS = vertexInput.positionVS;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.positionOS = input.positionOS;
                
                float4 ndc = output.positionCS * 0.5;
                output.positionNDC.xy = float2(ndc.x,ndc.y * _ProjectionParams.x)+ndc.w;
                output.positionNDC.zw = output.positionCS.zw;
                
                output.viewDirWS = GetCameraPositionWS() - output.positionWS;
                output.normalWS = normalInput.normalWS;
                output.normalVS = TransformWorldToViewDir(output.normalWS, true);
                real sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.bitangent = normalInput.bitangentWS;
                output.shadowCoord = GetShadowCoord(vertexInput);
                #if defined(_FOG_FRAGMENT)
                    output.fogFactor  = ComputeFogFactor(vertexInput.positionCS.z);
                #endif
                OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
                output.color = input.color;
                return output;
            }
            
            half MySpecularTerm (float roughness , float3 normalWS,float3 lightDir,float3 viewDirWS)
            {
                half specularColor = 0;
                half r2 = roughness*roughness;
                float3 halfDir = SafeNormalize(normalize(lightDir) + float3(normalize(viewDirWS)));
                float NoH = saturate(dot(normalWS,halfDir));
                half a2 = r2*r2;
                specularColor = a2 / (PI*pow((pow(NoH,2)*(a2-1)+1),2)) ;
                specularColor = saturate(specularColor);
                return specularColor;
            }

            float4 TransformHClipToViewPortPos(float4 positionCS)
             {
                 float4 o = positionCS * 0.5f;
                 o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
                 o.zw = positionCS.zw;
                 return o / o.w;
             }

            void ApplyAdditionalLight(inout float3  mainColor ,float3 worldPos,float3 normalWS,float3 viewDirWS)
            {
                //MultipleLight
                half3 mixColor = 0;
                //#if _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light AdditionalLight = GetAdditionalLight(lightIndex, worldPos);
                    half lightAttenuation = AdditionalLight.distanceAttenuation * AdditionalLight.shadowAttenuation;
                    half3 addLightDiffuse = max(0,dot(AdditionalLight.direction,normalWS)*lightAttenuation).rrr * AdditionalLight.color.rgb;
                    half3 addLightSpecular = MySpecularTerm(_Specular1,normalWS,AdditionalLight.direction,viewDirWS) * lightAttenuation * AdditionalLight.color.rgb * _SpecularColor.rgb;
                    mixColor += lerp(addLightDiffuse,addLightSpecular,_AddSpecular);
                }
                //#endif
                mainColor+=mixColor;
                // //debug
                //mainColor = mixColor;
            }

            float3 RimLighting(float3 positionCS,float3 normalWS,half rimlightWidth,half rimlightThreshold,half rimlightFadeout,half3 rimlightBrightness)
            {
                float3 rimLightColor = (half3)0;

                // float SceneDepth = LoadSceneDepth(positionCS.xy);
                //
                // float linearEyeDepth = LinearEyeDepth(SceneDepth, _ZBufferParams);
                
                float linearEyeDepth = LinearEyeDepth(positionCS.z, _ZBufferParams);
                float3 normalVS = mul((float3x3)UNITY_MATRIX_V, normalWS);

                //problem.2

                //114.59f ·= 2.0 * (180 / 3.1415926)
                //
                float fixedfovValue = atan(1.0f / unity_CameraProjection._m11) * 114.59f * 0.05;
                rimlightWidth *= (1/fixedfovValue);
                //
                // return fixedValue * _fixedValueScaleFactor;//_fixedValueScaleFactor is some small number
                
                float2 uvOffset = float2(sign(normalVS.x), 0) * rimlightWidth / (1 + linearEyeDepth) / 100;
                int2 loadTexPos = positionCS.xy + uvOffset * _ScaledScreenParams.xy;
                loadTexPos = min(max(loadTexPos, 0), _ScaledScreenParams.xy - 1);
                float offsetSceneDepth = LoadSceneDepth(loadTexPos);
                float offsetLinearEyeDepth = LinearEyeDepth(offsetSceneDepth, _ZBufferParams);
                float rimLight = saturate(offsetLinearEyeDepth - linearEyeDepth) * 10 /rimlightFadeout;
                rimLight = clamp(rimLight,0,1);
                rimLight = step(rimlightThreshold,rimLight);

                return rimLight * rimlightBrightness;
            }

            half4 frag (Varyings input) : SV_Target
            {

                
                UNITY_SETUP_INSTANCE_ID(input);
                //Make ObjectZeroWS work in Particle 
                float3 objectZeroWS = lerp(TransformObjectToWorld(half3(0,0,0)),_objectZeroPositionWS_Particle.xyz, _IsParticle);
                Light mainLight =  GetMainLight();
                //Shadow
                #if ((_MAIN_LIGHT_SHADOWS) || (_MAIN_LIGHT_SHADOWS_CASCADE) )&& (_USESHADOW)
                // Light lighShadow = GetMainLight(shadowCoord);
                // half ShadowAtten = lighShadow.shadowAttenuation;
                half4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
                half4 shadowParams = GetMainLightShadowParams();
                half ShadowAtten = SampleShadowmap(TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowCoord, shadowSamplingData, shadowParams, false);
                #else
                half ShadowAtten = 1;
                #endif
                
                //NormalMap
                #ifdef  _NORMALMAP
                float3 normalTS =UnpackNormalScale( SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,input.uv*_NormalMap_ST.xy+_NormalMap_ST.zw),_NormalScale);
                half3x3 TBN = half3x3(input.tangentWS.xyz, input.bitangent, input.normalWS.xyz);
                float3 normalWS = TransformTangentToWorld(normalTS, TBN);
                normalWS = NormalizeNormalPerPixel(normalWS);
                #else
                float3 normalWS = normalize(input.normalWS);
                #endif
                
                //baseLight
                half lambert = saturate(dot(normalize(GetMainLight().direction),normalWS));
                
                //globeIllumination
                half3 globeIllumination=0;
                #if defined(LIGHTMAP_ON)
                    globeIllumination = SampleLightmap(input.staticLightmapUV,normalWS);
                #else
                    globeIllumination = SampleSH(normalWS)*0.5;
                #endif

                //specular
                half specular1 = MySpecularTerm(_Specular1,normalWS,mainLight.direction,input.viewDirWS);
                half3 specularCol = _SpecularColor * specular1 * (saturate(lambert * ShadowAtten +_SpecularInShadow)*lambert);
                
                //diffuseCol
                float2 rampUV = float2(smoothstep(_LambertSmooth.x,_LambertSmooth.x+_LambertSmooth.y,lambert ) * smoothstep(_ShadowSmooth.x,_ShadowSmooth.x+_ShadowSmooth.y,ShadowAtten) , _RampIndex);
                half3 shadowCol = saturate(SAMPLE_TEXTURE2D(_RampShadowTex,sampler_LinearClamp,rampUV) + (1-_ShadowIntensity));
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,TRANSFORM_TEX(input.uv,_MainTex))*_MainColor;
                half3 diffuseCol = mainTex.rgb * shadowCol.rgb ;

                //RimLight & reflectCol
                half3 edgeLight = saturate(pow(max(0,1-dot(normalize(input.viewDirWS),normalize(input.normalWS))),_ReflectionSetting.x) * _ReflectionSetting.y) * _ReflectionCol;
                half3 reflectCol = GlossyEnvironmentReflection(reflect(-normalize(input.viewDirWS),normalWS),input.positionWS,_ReflectionSetting.z,1);
                reflectCol = lerp(reflectCol,(reflectCol.r+reflectCol.g+reflectCol.b)/3,saturate(_ReflectionSetting.w));
                // half3 reflectCol = CalculateIrradianceFromReflectionProbes(reflect(-normalize(input.viewDirWS),normalWS),input.positionWS,_RimLightPowScale.z);
                edgeLight *= reflectCol;

                //屏幕空间深度边缘光 需要开启ZPropass与描边冲突暂不支持
                // float3 samplePositionVS = float3(input.positionVS.xy + input.normalVS.xy * _RimWidth, input.positionVS.z); // 保持z不变（CS.w = -VS.z）
                // float4 samplePositionCS = TransformWViewToHClip(samplePositionVS); // input.positionCS不是真正的CS 而是SV_Position屏幕坐标
                // float4 samplePositionVP = TransformHClipToViewPortPos(samplePositionCS);
                // float depth = input.positionNDC.z / input.positionNDC.w;
                // //float depth = LoadSceneDepth(input.positionCS.xy);
                // float linearEyeDepth = LinearEyeDepth(depth, _ZBufferParams); // 离相机越近越小
                // float offsetDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, samplePositionVP).r; // _CameraDepthTexture.r = input.positionNDC.z / input.positionNDC.w
                // float linearEyeOffsetDepth = LinearEyeDepth(offsetDepth, _ZBufferParams);
                // float depthDiff = linearEyeOffsetDepth - linearEyeDepth;
                // float rimIntensity = step(_Threshold, depthDiff);
                // edgeLight+=rimIntensity;

                half3 rimCol = step(_Threshold,pow(max(0,1-dot(normalize(input.viewDirWS),normalize(input.normalWS))),_RimWidth))*_RimCol;
                rimCol = lerp(rimCol,rimCol*reflectCol,_ReflectionIntensity);
                edgeLight+=rimCol*lambert;
                
                //emissiveCol
                #ifdef _EMISSIVE
                half3 emissiveCol = _EmissiveColor * SAMPLE_TEXTURE2D(_EmissiveTex,sampler_EmissiveTex,input.uv*_EmissiveTex_ST.xy+_EmissiveTex_ST.zw);
                #else
                half3 emissiveCol = 0;
                #endif
                
                //effects
                half3 effects = 0;
                    //edgeHalftone
                #ifdef _EDGE_HALFTONE
                    float2 halftoneUV = -input.positionVS.xy/input.positionVS.z;
                    halftoneUV = lerp(halftoneUV*distance(objectZeroWS,GetCameraPositionWS()),halftoneUV*10,_HalftoneUVZScale);//stabilize UV tilingScale
                    half halftone = length(frac(halftoneUV* _HalftoneScale)-0.5);
                    half edgeMask = pow(max(0,1-dot(normalize(input.viewDirWS),normalize(input.normalWS))),_EdgePowScale.x) * _EdgePowScale.y;
                    halftone = saturate(step(pow(halftone,edgeMask),_HalftoneStep) * edgeMask);
                    half3 edgeHalftoneCol = halftone * _HalftoneColor *  _HalftoneEffect;
                #else
                    half3 edgeHalftoneCol = 0;
                #endif
                effects+=edgeHalftoneCol;
                
                //final
                half3 finalCol = 0;
                finalCol += (diffuseCol+specularCol) * _MainLightColor + emissiveCol +edgeLight  + globeIllumination + effects;

                //applyAdditionalLight
                #if defined(_ADDITIONAL_LIGHTS)
                    ApplyAdditionalLight(finalCol,input.positionWS,normalWS,input.viewDirWS);
                #endif
                
                finalCol = MixFog(finalCol,input.fogFactor);
                
                #if _ALPHA_CLIP_ON
                clip(mainTex.a-_Cutoff);
                #endif
                
                return half4(finalCol,saturate(mainTex.a*_MainTexA));
            }
            ENDHLSL
        }

//        Pass 
//        {
//            Name "OutLinePass"
//            Tags{ "LightMode" = "SRPDefaultUnlit" }
//            ZWrite [_ColorMask]
//	        Cull Front
//	        ColorMask [_ColorMask]
//	        Blend SrcAlpha [_BlendAAA] 
//	        
//            Stencil
//            {
//                Ref [_Stencil]
//                Comp [_StencilComp]
//                Pass [_StencilOp]
//                ReadMask [_StencilReadMask]
//                WriteMask [_StencilWriteMask]
//            }
//	        
//	        HLSLPROGRAM
//	        #pragma prefer_hlslcc gles
//            #pragma exclude_renderers d3d11_9x
//            #pragma target 2.0
//	        
//	        #pragma shader_feature_local_vertex _UNIFORM_OUTLINE_WIDTH
//
//	        //SmoothNormalData
//	        //#pragma shader_feature_local_vertex _SMOOTHNORMAL_NULL
//	        #pragma shader_feature_local_fragment _ALPHA_CLIP_ON
//	        #pragma shader_feature_local_vertex _SMOOTHNORMAL_VERTEX_COLOR
//	        #pragma shader_feature_local_vertex _SMOOTHNORMAL_TANGENT
//	        #pragma shader_feature_local_vertex _SMOOTHNORMAL_TEXCOORD3
//	        
//	        
//	        #pragma multi_compile_fog
//	        
//	        #pragma vertex vert  
//	        #pragma fragment frag
//
//	        #include "../Shaders/SimpleNPR_TransparentInput.hlsl"
//
//            struct Attributes
//            {
//                float4 positionOS   : POSITION;
//                float4 color        :COLOR;
//                float3 normalOS     : NORMAL;
//                float4 tangentOS    : TANGENT;
//                float2 texcoord     : TEXCOORD0;
//                float4 smoothNormalTS     : TEXCOORD3;
//                UNITY_VERTEX_INPUT_INSTANCE_ID
//            };
//
//            struct Varyings
//            {
//                half  fogFactor                 : TEXCOORD9;
//                float4 positionCS               : SV_POSITION;
//                float3 normalWS                 : TEXCOORD4;
//                float2 uv                       : TEXCOORD2;
//                float4 color                    : TEXCOORD3;
//                UNITY_VERTEX_INPUT_INSTANCE_ID
//                UNITY_VERTEX_OUTPUT_STEREO
//            };
//            float GetCameraFOV()
//            {
//                //https://answers.unity.com/questions/770838/how-can-i-extract-the-fov-information-from-the-pro.html
//                float t = unity_CameraProjection._m11;
//                float Rad2Deg = 180 / 3.1415;
//                float fov = atan(1.0f / t) * 2.0 * Rad2Deg;
//                return fov;
//            }
//            float ApplyOutlineDistanceFadeOut(float inputMulFix)
//            {
//                //make outline "fadeout" if character is too small in camera's view
//                return saturate(inputMulFix);
//            }
//            float GetOutlineCameraFovAndDistanceFixMultiplier(float positionVS_Z)
//            {
//                float cameraMulFix;
//                if(unity_OrthoParams.w == 0)
//                {
//                    ////////////////////////////////
//                    // Perspective camera case
//                    ////////////////////////////////
//
//                    // keep outline similar width on screen accoss all camera distance       
//                    cameraMulFix = abs(positionVS_Z);
//
//                    // can replace to a tonemap function if a smooth stop is needed
//                    cameraMulFix = ApplyOutlineDistanceFadeOut(cameraMulFix);
//
//                    // keep outline similar width on screen accoss all camera fov
//            //                    cameraMulFix *= GetCameraFOV();       
//                }
//                else
//                {
//                    ////////////////////////////////
//                    // Orthographic camera case
//                    ////////////////////////////////
//                    float orthoSize = abs(unity_OrthoParams.y);
//                    orthoSize = ApplyOutlineDistanceFadeOut(orthoSize);
//                    cameraMulFix = orthoSize * 50; // 50 is a magic number to match perspective camera's outline width
//                }
//
//                return cameraMulFix * 0.5; // mul a const to make return result = default normal expand amount WS
//            }
//            float4 ClipPosZOffset(float4 positionCS, float viewSpaceZOffset)
//            {
//                float modifiedPositionVS_Z = - positionCS.w - viewSpaceZOffset;
//
//                float modifiedPositionCS_Z = modifiedPositionVS_Z * UNITY_MATRIX_P[2].z + UNITY_MATRIX_P[2].w;
//
//                positionCS.z = modifiedPositionCS_Z * positionCS.w / (-modifiedPositionVS_Z);
//
//                return positionCS;
//            }
//	        
//	        Varyings vert(Attributes v)
//	        {
//                Varyings o = (Varyings)0;
//                o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
//                o.normalWS = normalize(GetVertexNormalInputs(v.normalOS).normalWS);
//                o.color = v.color;
//                VertexPositionInputs vertexPositionInput = GetVertexPositionInputs(v.positionOS.xyz);
//                _OutlineWidth = max(-0.1,_OutlineWidth);
//                _OutlineWidth *= GetOutlineCameraFovAndDistanceFixMultiplier(vertexPositionInput.positionVS.z);
//                
//                #ifdef _UNIFORM_OUTLINE_WIDTH
//                _OutlineWidth *= vertexPositionInput.positionCS.w * 0.1;
//                #endif
//
//                _OutlineWidth *= lerp(1,o.color.a,_UseVertexColorA);
//                
//	            float3x3 TBNOS = float3x3(v.tangentOS.xyz,cross(v.normalOS,v.tangentOS.xyz) * v.tangentOS.w,v.normalOS.xyz);
//                TBNOS = lerp(float3x3(float3(1,0,0),float3(0,1,0),float3(0,0,1)),TBNOS,_IsTangentSpace);
//                
//                #if _SMOOTHNORMAL_VERTEX_COLOR
//	            o.positionCS = TransformObjectToHClip(v.positionOS.xyz + normalize(mul(v.color.rgb,TBNOS).xyz) * 0.01 * _OutlineWidth );
//	            #elif  _SMOOTHNORMAL_TANGENT
//                o.positionCS = TransformObjectToHClip(v.positionOS.xyz + normalize(v.tangentOS.xyz) * 0.01 * _OutlineWidth );
//	            #elif _SMOOTHNORMAL_TEXCOORD3
//                o.positionCS = TransformObjectToHClip(v.positionOS.xyz + normalize(mul(v.smoothNormalTS.xyz,TBNOS)) * 0.01 * _OutlineWidth );
//	            #else
//	            o.positionCS = TransformObjectToHClip(v.positionOS.xyz + v.normalOS.xyz * 0.01 * _OutlineWidth );
//	            #endif
//
//                
//                #if defined(_FOG_FRAGMENT)
//                    o.fogFactor  = ComputeFogFactor(o.positionCS.z);
//                #endif
//                o.positionCS = ClipPosZOffset(o.positionCS,_ViewSpaceZOffset);
//                return o;
//	        }
//	        
//	         half4 frag(Varyings input) : SV_Target
//	        {
//	            UNITY_SETUP_INSTANCE_ID(input);
//
//	            #if _ALPHA_CLIP_ON
//	            clip(SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv).a - _Cutoff);
//	            #endif
//	            //half3 globeIllumination = SampleSH(input.normalWS)*0.5;
//	            half3 outlineCol = lerp(_OutlineCol,input.color,_UseVertexColorRGB).rgb ;
//	            
//                return half4(MixFog(outlineCol,input.fogFactor).rgb, saturate(SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv).a*_MainTexA));
//	        }
//	     ENDHLSL
//        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull [_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            //#pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature_local_fragment  _ALPHA_CLIP_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "../Shaders/SimpleNPR_TransparentInput.hlsl"
            #include "../Shaders/SimpleNPRShadowPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            //#pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature_local_fragment  _ALPHA_CLIP_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "../Shaders/SimpleNPR_TransparentInput.hlsl"
            #include "../Shaders/SimpleNPRDepthOnlyPass.hlsl"
            ENDHLSL
        }
        
    }CustomEditor "LWGUI.LWGUI"
}
