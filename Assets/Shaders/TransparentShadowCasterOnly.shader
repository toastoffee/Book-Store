Shader "Custom/InvisibleShadowCaster"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"   // 必须是 Opaque 队列！
        }

        // 关键：写入深度（必须！），但不写入颜色
        ZWrite On
        ColorMask 0   // 屏幕上不可见

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }

            half4 frag() : SV_Target
            {
                // 不会显示（ColorMask 0）
                return 0;
            }
            ENDHLSL
        }

        // Shadow Caster Pass —— 实际上 URP 会自动使用深度信息，
        // 所以这个 Pass 可省略，但保留更可靠
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // 直接输出裁剪空间位置用于阴影深度图
                o.pos = TransformWorldToHClip(TransformObjectToWorld(v.vertex.xyz));
                return o;
            }

            half4 frag() : SV_Target
            {
                return 0; // 不重要，深度已由 pos 决定
            }
            ENDHLSL
        }
    }

    Fallback Off
}