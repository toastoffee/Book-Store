Shader "Custom/URP/LitWithMinAmbient"
{
    Properties
    {
        _BaseMap("Albedo", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        _MinAmbient("Min Ambient", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
            "Queue" = "Geometry"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 支持主光源阴影（可选）
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

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
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                    float4 shadowCoord : TEXCOORD3;
                #endif
            };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            half4 _BaseMap_ST;
            half4 _BaseColor;
            half _MinAmbient;

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                    output.shadowCoord = TransformWorldToShadowCoord(vertexInput.positionWS);
                #endif

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // === 安全处理法线 ===
                half3 normalWS = normalize(input.normalWS);
                // 防止非法法线（如模型未导出法线）
                if (any(isnan(normalWS)) || all(normalWS == 0))
                    normalWS = half3(0, 1, 0);

                // === 获取主光源（Directional Light）===
                half3 lightColor = half3(1, 1, 1); // 默认白光
                half3 lightDir = half3(0, 1, 0);   // 默认从上方来
                half shadowAtten = 1.0;

                // 如果 URP 提供了主光源，就用它
                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                    Light mainLight = GetMainLight(input.shadowCoord);
                    lightColor = mainLight.color;
                    lightDir = mainLight.direction;
                    shadowAtten = mainLight.shadowAttenuation;
                #else
                    Light mainLight = GetMainLight();
                    lightColor = mainLight.color;
                    lightDir = mainLight.direction;
                #endif

                // === 计算 Lambert 漫反射强度 ===
                half NdotL = saturate(dot(normalWS, lightDir));
                half diffuseIntensity = NdotL * shadowAtten;

                // === 👇 核心：保底亮度（对标量强度操作）===
                half finalIntensity = max(diffuseIntensity, _MinAmbient);

                // === 应用贴图和颜色 ===
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                half3 finalColor = albedo.rgb * lightColor * finalIntensity;

                return half4(finalColor, albedo.a);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}