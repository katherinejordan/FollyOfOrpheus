Shader "Custom/MistyBackground"
{
    Properties
    {
        _BaseColor ("Base Background Color", Color) = (0.1, 0.15, 0.2, 1.0)
        _MistColor ("Mist Color", Color) = (0.7, 0.75, 0.8, 1.0)
        _MistScale1 ("Mist Layer 1 Scale", Float) = 4.0
        _MistScale2 ("Mist Layer 2 Scale", Float) = 8.0
        _ScrollSpeed1 ("Layer 1 Scroll Speed", Vector) = (0.05, 0.02, 0, 0)
        _ScrollSpeed2 ("Layer 2 Scroll Speed", Vector) = (-0.03, 0.05, 0, 0)
        _MistDensity ("Mist Density/Threshold", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Background" "Queue"="Background" "RenderPipeline"="UniversalPipeline"
        }
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Material Properties
            half4 _BaseColor;
            half4 _MistColor;
            float _MistScale1;
            float _MistScale2;
            float4 _ScrollSpeed1;
            float4 _ScrollSpeed2;
            half _MistDensity;

            // Simple 2D Pseudo-Random Noise function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float3 x)
            {
                const float3 step = float3(110, 241, 171);

                float3 i = floor(x);
                float3 f = frac(x);

                // For performance, compute the base input to a 1D hash from the integer part of the argument and the 
                // incremental change to the 1D based on the 3D -> 1D wrapping
                float n = dot(i, step);

                float3 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(lerp(hash(n + dot(step, float3(0, 0, 0))), hash(n + dot(step, float3(1, 0, 0))), u.x),
     lerp(hash(n + dot(step, float3(0, 1, 0))), hash(n + dot(step, float3(1, 1, 0))), u.x), u.y),
lerp(lerp(hash(n + dot(step, float3(0, 0, 1))), hash(n + dot(step, float3(1, 0, 1))), u.x),
    lerp(hash(n + dot(step, float3(0, 1, 1))), hash(n + dot(step, float3(1, 1, 1))), u.x), u.y), u.z);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y;

                float2 uv1 = input.uv * _MistScale1 + _ScrollSpeed1.xy * time;
                float2 uv2 = input.uv * _MistScale2 + _ScrollSpeed2.xy * time;

                float noise1 = noise(float3(uv1.x, uv1.y, 0));
                float noise2 = noise(float3(uv2.x, uv2.y, 0));

                float combinedNoise = (noise1 * 0.6) + (noise2 * 0.4);

                float mistAlpha = smoothstep(0.2, 1.0 - _MistDensity, combinedNoise);

                half4 finalColor = lerp(_BaseColor, _MistColor, mistAlpha);

                return finalColor;
            }
            ENDHLSL
        }
    }
}