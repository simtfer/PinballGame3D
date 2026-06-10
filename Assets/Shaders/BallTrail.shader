Shader "Pinball/BallTrail"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0,1,1,1)
        _Speed ("Scroll Speed", Float) = 2.0
        _FresnelPower ("Fresnel Power", Float) = 3.0
        _EmissionIntensity ("Emission Intensity", Float) = 2.0
        _PulseSpeed ("Pulse Speed", Float) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 vertexColor : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Speed;
                float _FresnelPower;
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
                output.vertexColor = input.color;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float3 viewDir = normalize(GetCameraPositionWS() - input.positionWS);
                float3 normalWS = normalize(input.normalWS);

                float fresnel = pow(1.0 - saturate(dot(viewDir, normalWS)), _FresnelPower);

                float scroll = frac(input.uv.x - _Time.y * _Speed);
                float pulse = sin(_Time.y * _PulseSpeed) * 0.3 + 0.7;

                float3 emission = _BaseColor.rgb * _EmissionIntensity * fresnel * pulse;

                float alpha = input.vertexColor.a * fresnel * scroll;

                return float4(emission + _BaseColor.rgb * 0.3, alpha);
            }
            ENDHLSL
        }
    }
}
