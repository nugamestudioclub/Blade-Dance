Shader "UI/SineWaveImage_Fixed"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Amp ("Wave Amplitude", Float) = 0.02
        _Freq ("Wave Frequency", Float) = 10
        _Speed ("Wave Speed", Float) = 1
        

        _Bins ("X Bins", Float) = 12

        _GapCenter ("Gap Center (0-1)", Range(0,1)) = 0.5
        _GapSize   ("Gap Size", Range(0,1)) = 0.3
        _GapSoft   ("Gap Softness", Range(0,0.2)) = 0.01

        _GradA ("Gradient Edge Color", Color) = (1,1,1,1)
        _GradB ("Gradient Center Color", Color) = (0,0,0,1)
        _GradPow ("Gradient Power", Range(0.1, 8.0)) = 1.0

        _BinPhase ("Bin Phase Step", Float) = 6.28318

        _BandsTex ("Bands Tex", 2D) = "white" {}
        _BandCount ("Band Count", Float) = 64
        _Height ("Max Bar Height", Range(0,1)) = 0.25
        _Steps ("Vertical Steps", Float) = 16




    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _Amp, _Freq, _Speed;
            float _Bins;
            float _GapCenter, _GapSize, _GapSoft;
            fixed4 _GradA, _GradB;
            float _GradPow;
            sampler2D _BandsTex;
            float _BandCount;
            float _Height;
            float _Steps;

            


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // --------- 1) Pick which band (bin) this pixel belongs to ----------
                float bandCount = max(1.0, _BandCount);
                float _x = (uv.x-0.5f)<0? 0.5f-uv.x : uv.x-0.5f;
                float idx = floor(_x * bandCount);           // 0..bandCount-1
                float u = (idx + 0.5) / bandCount;             // center sample for that band

                // --------- 2) Read amplitude for this band (0..1) from the 1D bands texture ----------
                float a = tex2D(_BandsTex, float2(u, 0.5)).r;
                a = saturate(a);

                // Optional: make "stacked blocks" like classic equalizers
                float steps = max(1.0, _Steps);
                a = floor(a * steps) / steps;

                // Convert amplitude into a bar thickness (in UV units)
                float bar = a * _Height*0.5f;                       // e.g. _Height = 0.25 means max 25% from top/bottom

                // --------- 3) Build top/bottom bar mask ----------
                // top bar occupies [1-bar, 1], bottom bar occupies [0, bar]
                float topMask = step(1.0 - bar, uv.y);
                float botMask = step(uv.y, bar);
                float barsMask = saturate(topMask + botMask);  // 0..1

                // --------- 4) Center gap mask (transparent in the middle band, soft edges) ----------
                float dist = abs(uv.y - _GapCenter);
                float halfGap = _GapSize * 0.5;
                float gapMask = smoothstep(halfGap - _GapSoft, halfGap + _GapSoft, dist);

                // --------- 5) Mirrored gradient (top->center and bottom->center) ----------
                float t = saturate(abs(uv.y - 0.5) * 2.0);   // 0 at center, 1 at rims
                t = pow(t, _GradPow);
                fixed3 gradRGB = lerp(_GradB.rgb, _GradA.rgb, t);

                // --------- 6) Final color ----------
                // If you want to ignore the sprite texture and just draw the visualizer color:
                fixed4 col = fixed4(gradRGB, 1.0);

                col *= i.color; // keep UI tint/vertex color
                col.a *= barsMask * gapMask;

                return col;
            }


            ENDCG
        }
    }
}