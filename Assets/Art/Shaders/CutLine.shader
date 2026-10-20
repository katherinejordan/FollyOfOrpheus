Shader "Custom/CutLine"
{
    Properties
    {
        _Color ("Color", Color) = (1,0,0,1)
        _CutWidth ("Width", Float) = 0.05
        _CutHalfLength ("Half Length", Float) = 0.25
        _CutPoint ("Cut Point", Vector) = (0,0,0,0)
        _CutNormal ("Cut Normal", Vector) = (0,1,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }

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
                float4 _CutPoint;
                float4 _CutNormal;
                float  _CutWidth;
                float  _CutHalfLength;
            CBUFFER_END

            // restPos: bind-pose object-space position, baked into UV1 by RestPositionUVPostprocessor
            struct VertexInput  { float4 positionOS : POSITION; float3 restPos : TEXCOORD1; };
            struct VertexOutput { float4 positionCS : SV_POSITION; float3 restPos : TEXCOORD0; };


            VertexOutput vert(VertexInput IN)
            {
                VertexOutput OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.restPos = IN.restPos;
                return OUT;
            }

            half4 frag(VertexOutput IN) : SV_Target
            {
                float3 planeNormal = normalize(_CutNormal.xyz);
                float dist = dot(IN.restPos - _CutPoint.xyz, planeNormal);
                float stroke = step(abs(dist), _CutWidth * 0.5);
                return half4(_Color.rgb, _Color.a * stroke);
            }
            ENDHLSL
        }
    }
}