Shader "Custom/CutLine"
{
    Properties
    {
        _CutAngle ("Angle", Range(-180, 180)) = 0
        _CutAxis ("Rotation Axis", Vector) = (0,0,1,0)
        
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
                float  _CutAngle;
                float4 _CutAxis;

                float4 _Color;
                float4 _CutPoint;
                float4 _CutNormal;
                float  _CutWidth;
                float  _CutHalfLength;
            CBUFFER_END

            struct VertexInput  { float4 positionOS : POSITION; };
            struct VertexOutput { float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; };

            float3 RotateAroundAxis(float3 v, float3 axis, float degrees)
            {
                float a = radians(degrees);
                float s = sin(a);
                float c = cos(a);
                return v * c + cross(axis, v) * s + axis * dot(axis, v) * (1 - c);
            }

            VertexOutput vert(VertexInput IN)
            {
                VertexOutput OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            half4 frag(VertexOutput IN) : SV_Target
            {
                float3 axis = normalize(_CutAxis.xyz);
                float3 planeNormal = RotateAroundAxis(normalize(_CutNormal.xyz), axis, _CutAngle);

                float dist = dot(IN.positionWS - _CutPoint.xyz, planeNormal);
                float stroke = step(abs(dist), _CutWidth * 0.5);
                return half4(_Color.rgb, _Color.a * stroke);
            }
            ENDHLSL
        }
    }
}