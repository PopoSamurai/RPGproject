Shader "Tactical/AnimatedWhiteSpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlinePixels ("Outline Thickness in Pixels", Range(1,8)) = 3
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlinePixels;
            float4 _SpriteUV;
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }
            float AlphaAt(float2 uv)
            {
                if (uv.x < _SpriteUV.x || uv.y < _SpriteUV.y || uv.x > _SpriteUV.z || uv.y > _SpriteUV.w)
                    return 0;
                return tex2D(_MainTex, uv).a;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv) * i.color;
                float2 px = _MainTex_TexelSize.xy * _OutlinePixels;
                float around = 0;
                around = max(around, AlphaAt(i.uv + float2(px.x,0)));
                around = max(around, AlphaAt(i.uv + float2(-px.x,0)));
                around = max(around, AlphaAt(i.uv + float2(0,px.y)));
                around = max(around, AlphaAt(i.uv + float2(0,-px.y)));
                around = max(around, AlphaAt(i.uv + float2(px.x,px.y)));
                around = max(around, AlphaAt(i.uv + float2(-px.x,px.y)));
                around = max(around, AlphaAt(i.uv + float2(px.x,-px.y)));
                around = max(around, AlphaAt(i.uv + float2(-px.x,-px.y)));
                float outline = saturate(around - baseColor.a) * _OutlineColor.a;
                float alpha = saturate(baseColor.a + outline);
                float3 rgb = baseColor.rgb * baseColor.a + _OutlineColor.rgb * outline * (1 - baseColor.a);
                return fixed4(rgb, alpha);
            }
            ENDCG
        }
    }
}
