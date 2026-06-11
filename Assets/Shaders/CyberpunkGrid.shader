Shader "Pinball/CyberpunkGrid"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.04, 0.04, 0.18, 1)
        _GridColor ("Grid Color", Color) = (0, 1, 1, 1)
        _GridWidth ("Grid Width", Float) = 0.02
        _GridScale ("Grid Scale", Float) = 10
        _EmissionIntensity ("Emission Intensity", Float) = 1.5
        _PulseSpeed ("Pulse Speed", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _GridColor;
                float _GridWidth;
                float _GridScale;
                float _EmissionIntensity;
                float _PulseSpeed;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.uv = input.uv;
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 gridUV = input.uv * _GridScale;
                float2 grid = abs(frac(gridUV - 0.5) - 0.5) / fwidth(gridUV);
                float gridLine = min(grid.x, grid.y);
                float gridMask = 1.0 - min(gridLine, 1.0);

                float pulse = sin(_Time.y * _PulseSpeed) * 0.3 + 0.7;
                float3 gridEmission = _GridColor.rgb * _EmissionIntensity * gridMask * pulse;

                float3 normalWS = normalize(input.normalWS);
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(normalWS, normalize(mainLight.direction)));
                float3 diffuse = mainLight.color * NdotL;

                float3 ambient = float3(0.02, 0.02, 0.08);
                float3 finalColor = _BaseColor.rgb * (diffuse + ambient) + gridEmission;

                float fogFactor = input.fogFactor;
                finalColor = MixFog(finalColor, fogFactor);

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
