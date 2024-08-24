Shader "Custom/FlashShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _FlashColor ("Flash Color", Color) = (1,0,0,1)
        _FlashAmount ("Flash Amount", Range(0,1)) = 0
        _MainTex ("Base , Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend One Zero
            Cull Off
            ZWrite On



            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float4 positionOS = input.positionOS;
                float3 position3 = positionOS.xyz;
                float4 clipPosition = TransformObjectToHClip(position3);
                output.positionHCS = clipPosition;
                output.uv = input.uv;
                return output;
            }

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half4 _FlashColor;
            half _FlashAmount;
            sampler2D _MainTex;
            CBUFFER_END

            half4 frag(Varyings input) : SV_Target
            {
                half4 baseColor = tex2D(_MainTex, input.uv) * _BaseColor;
                half4 color = lerp(baseColor, _FlashColor, _FlashAmount);
                return color;
            }

            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
