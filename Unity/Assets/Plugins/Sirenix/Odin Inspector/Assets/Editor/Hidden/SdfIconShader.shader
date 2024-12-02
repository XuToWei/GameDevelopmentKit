Shader "Hidden/Sirenix/SdfIconShader"
{
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _SirenixOdin_SdfTex;
            float _SirenixOdin_EdgeOffset;
            float4 _SirenixOdin_Color;
            float4 _SirenixOdin_BgColor;
            float4 _SirenixOdin_Uv;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float samplePixel(float2 uv) {
                return tex2D(_SirenixOdin_SdfTex, uv).a;
            }

            float linearstep(float lo, float hi, float input) {
                float diff = hi - lo;
                float offset = input - lo;
                return min(1.0, max(0.0, offset / diff));
            }

            float sampleDist(float2 uv, float dx, float edge, float padding) {
                float dist = samplePixel(uv);
                float p = -abs((dx * 3072.0) / -padding);
                float a = min(1, max(0, edge - p * 0.33333));
                float b = max(0, min(1, edge + p * 0.33333));
                return smoothstep(b, a, dist);
            }

            float4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                uv.y = 1 - uv.y;
                uv.x = _SirenixOdin_Uv.x + uv.x * _SirenixOdin_Uv.z;
                uv.y = _SirenixOdin_Uv.y + uv.y * _SirenixOdin_Uv.w;
                uv.y = 1 - uv.y;

                float alpha = 0.0;
                float edge = 0.5019608 + _SirenixOdin_EdgeOffset;

                if (_SirenixOdin_BgColor.a > 0.01) {
                    float3 colorBg = _SirenixOdin_BgColor.rgb;
                    float3 colorFg = _SirenixOdin_Color.rgb;

                    float padding = 8;
                    float dx = ddx(uv.x);
                    float2 t = float2(dx * 0.333333, 0);
                    float3 subDist = float3(
                                            sampleDist(uv.xy - t, dx, edge, padding),
                                            sampleDist(uv.xy, dx, edge, padding),
                                            sampleDist(uv.xy + t, dx, edge, padding));
                    float3 color = lerp(colorBg, colorFg, clamp(subDist, 0.0, 1.0));

                    float alpha = min(1, subDist.r + subDist.g + subDist.b);
                    float4 col = float4(color, alpha * _SirenixOdin_Color.a);

                    return col;
                } else {
                    float padding = 8;
                    float dx = ddx(uv.x);
                    float alpha = sampleDist(uv, dx, edge, padding);
                    float4 col = _SirenixOdin_Color;
                    col.a *= alpha;

                    return col;
                }
            }
            ENDCG
        }
    }
}