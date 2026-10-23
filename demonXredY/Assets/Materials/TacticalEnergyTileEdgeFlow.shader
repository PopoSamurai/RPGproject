Shader "Tactical/EnergyTileEdgeFlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Frame Sprite (RGBA)", 2D) = "white" {}
        _EnergyColor ("Energy Color", Color) = (0.23,0.55,1,1)
        [HDR] _FrameColor ("Frame Base (HDR)", Color) = (1,1,1,1)
        _FillOpacity ("Center Opacity", Range(0,0.5)) = 0.045
        _FrameOpacity ("Frame Opacity", Range(0,1)) = 0.94
        _FrameGlow ("Frame Glow Strength", Range(0,8)) = 2.0
        _BorderWidth ("Border Width (UV)", Range(0.002,0.10)) = 0.020
        _GlowWidth ("Edge Color Spread (UV)", Range(0.005,0.45)) = 0.21
        _EnergyStrength ("Edge Energy Strength", Range(0,1)) = 0.34
        _EnergyScale ("Edge Energy Frequency", Range(1,30)) = 5
        _EnergySpeed ("Edge Energy Speed", Range(0,5)) = 0.75
        _PulseAmount ("Frame Pulse Amount", Range(0,0.5)) = 0.10
        _FrameTint ("Frame Tint by Energy Color", Range(0,1)) = 0.70
        _EdgeOpacity ("Edge Color Opacity", Range(0,0.8)) = 0.25
        _FlowBandWidth ("Flow Band Width", Range(0.02,0.45)) = 0.13
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Depth Test", Float) = 4
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="False" }
        Cull Off ZWrite Off ZTest [_ZTest]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _EnergyColor;
            float4 _FrameColor;
            float _FillOpacity, _FrameOpacity, _FrameGlow, _BorderWidth, _GlowWidth;
            float _EnergyStrength, _EnergyScale, _EnergySpeed, _PulseAmount;
            float _FrameTint, _EdgeOpacity, _FlowBandWidth;
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                // Procedural border: independent of opacity of the original PNG center.
                float d = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));
                float aa = max(fwidth(d) * 1.25, 0.0008);
                float frame = 1.0 - smoothstep(_BorderWidth - aa, _BorderWidth + aa, d);
                float proximity = 1.0 - smoothstep(_BorderWidth, _BorderWidth + _GlowWidth, d);
                float coloredRim = proximity * (1.0 - frame);

                // Animate a narrow traveling highlight around the perimeter, not across the center.
                float2 q = saturate(uv);
                float perimeter = (q.y <= q.x && q.y <= 1.0-q.x) ? q.x : 0.0;
                // Use continuous square perimeter parameter based on nearest edge.
                float left = q.x, right = 1.0-q.x, bottom = q.y, top = 1.0-q.y;
                if (bottom <= left && bottom <= right && bottom <= top) perimeter = q.x;
                else if (right <= left && right <= bottom && right <= top) perimeter = 1.0 + q.y;
                else if (top <= left && top <= bottom && top <= right) perimeter = 2.0 + (1.0-q.x);
                else perimeter = 3.0 + (1.0-q.y);
                float travel = frac(perimeter * max(_EnergyScale,1.0) * 0.25 - _Time.y * _EnergySpeed * 0.22);
                float distanceToBand = min(travel, 1.0 - travel);
                float band = 1.0 - smoothstep(0.0, max(_FlowBandWidth,0.001), distanceToBand);
                float pulse = 1.0 + _PulseAmount * sin(_Time.y * 2.3);
                float edgeFlow = 1.0 + _EnergyStrength * band;

                float centerAlpha = _FillOpacity * (1.0 - proximity);
                float rimAlpha = coloredRim * _EdgeOpacity * edgeFlow;
                float frameAlpha = frame * _FrameOpacity;
                float alpha = saturate(centerAlpha + rimAlpha + frameAlpha);

                float3 frameColor = lerp(_FrameColor.rgb, _EnergyColor.rgb * 1.65 + _FrameColor.rgb * 0.35, _FrameTint);
                frameColor *= _FrameGlow * pulse * (1.0 + band * _EnergyStrength * 0.4);
                float3 rimColor = _EnergyColor.rgb * (1.0 + band * _EnergyStrength * 0.6);
                float frameMix = frameAlpha / max(alpha, 0.0001);
                float3 rgb = lerp(rimColor, frameColor, saturate(frameMix));
                return float4(rgb * i.color.rgb, alpha * i.color.a);
            }
            ENDCG
        }
    }
    Fallback Off
}
