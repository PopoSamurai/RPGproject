Shader "Unlit/SpriteWhiteFlash"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color  ("Tint", Color) = (1,1,1,1)

        _Flash ("Flash Amount", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "IgnoreProjector"="True"
        }

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

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            sampler2D _MainTex;
            float4   _MainTex_ST;
            fixed4   _Color;
            float    _Flash;

            v2f vert (appdata_t IN)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(IN.vertex);
                o.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                o.color    = IN.color * _Color;
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.rgb *= c.a;
                fixed3 flashCol = fixed3(1,1,1) * c.a;
                c.rgb = lerp(c.rgb, flashCol, _Flash);

                return c;
            }
            ENDCG
        }
    }
}