Shader "Unlit/StencilMaskWriter"
{
    Properties
    {
        _MainTex ("Mask Texture (Alpha = Shape)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest-1" }
        LOD 100

        // Stencil: 只在不透明区域写入 1
        Stencil {
            Ref 1
            Comp Always
            Pass Replace
        }

        Lighting Off
        ZWrite Off
        ColorMask 0  // 不写入颜色

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv);
                // 如果 alpha 小于阈值，丢弃片元 → 不写 stencil
                clip(texCol.a - _Cutoff);
                // 否则继续（ColorMask 0 已禁用颜色输出）
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}