Shader "Custom/CutLine"
{
    Properties
    {
        _Color ("Color", Color) = (1,0,0,1)
        _CutWidth ("Width", Float) = 0.05
        _CutHalfLength ("Half Length", Float) = 0.25
        [HideInInspector] _CutPoint ("Cut Point", Vector) = (.5,0,0)
        [HideInInspector] _CutNormal ("Cut Normal", Vector) = (1,1,0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue"="Transparent"}

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float3 _CutPoint;
                float3 _CutNormal;
                float  _CutWidth;
                float  _CutHalfLength;
            CBUFFER_END

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VertexInput
            {
                float4 positionOS : POSITION;
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END

            VertexOutput vert(VertexInput IN)
            {
                VertexOutput OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            half4 frag(VertexOutput IN) : SV_Target
            {
                float3 planeNormal = normalize(_CutNormal);
                float dist = dot(IN.positionWS - _CutPoint, planeNormal);
                float stroke = step(abs(dist), _CutWidth);

                return half4(_Color.rgb, _Color.a * stroke);
            }
            ENDHLSL
        }
    }
}
