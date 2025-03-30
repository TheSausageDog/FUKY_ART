Shader "Unlit/liquid"
{
    Properties
    {
        _water_color ("WaterColor", Color) = (1,1,1,1)
        _surface_color ("SurfaceColor", Color) = (1,1,1,1)
        _fresnel_color ("FresnelColor", Color) = (1,1,1,1)
		// _threshold("test_threshold", Range(-1, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldNormal : NORMAL;
                float4 worldPos: POSITION1;
            };

            half4 _water_color;
            half4 _surface_color;
            half4 _fresnel_color;
            float _threshold; 

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            inline half3 Pow5 (half3 x)
            {
                return x*x * x*x * x;
            }

            inline half3 FresnelTerm (half3 F0, half cosA)
            {
                half t = Pow5 (1 - cosA);   // ala Schlick interpoliation
                return F0 + (1-F0) * t;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float worldY = unity_ObjectToWorld._m13;
                float dif = worldY - i.worldPos.y;
                clip(dif - _threshold);
                float3 view = normalize(_WorldSpaceCameraPos-i.worldPos);
                float NoV = dot(i.worldNormal, view);
                float front = step(0, NoV);
                
                half4 color = front * _water_color + (1 - front) * _surface_color;
                NoV = front * NoV + (1 - front) * dot(float3(0, 1, 0), view);
                // color.a = alpha * color.a;
                color.rgb += 0.02 + _fresnel_color.rgb * Pow5 (1 - NoV);
                
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }

            ENDCG
        }
    }
}
