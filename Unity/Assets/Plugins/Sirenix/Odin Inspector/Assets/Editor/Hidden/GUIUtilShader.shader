Shader "Hidden/Sirenix/OdinGUIShader"
{
    SubShader
    {
        Lighting Off
        Cull Off
        ZWrite Off
        ZTest Always
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
            float _SirenixOdin_GreyScale;
            float4 _SirenixOdin_GUIColor;
            float4 _SirenixOdin_GUIUv;
            float4 _SirenixOdin_HueColor;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float test1(float x, float y) {
                if (x >= y) {
                    return 0;
                } else {
                    return 1;
                }
            }

            float test2(float x, float y) {
                return step(x, y);
            }

            float3 rgb2hsv(float3 c) {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c) {
                c = float3(c.x, clamp(c.yz, 0.0, 1.0));
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            float4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                uv.y = 1 - uv.y;
                uv.x = _SirenixOdin_GUIUv.x + uv.x * _SirenixOdin_GUIUv.z;
                uv.y = _SirenixOdin_GUIUv.y + uv.y * _SirenixOdin_GUIUv.w;
                uv.y = 1 - uv.y;

                // Greyscale
                float4 col = tex2D(_MainTex, uv);
                float3 greyScale = (0.3 * col.r) + (0.59 * col.g) + (0.11 * col.b);
                col.rgb = lerp(col.rgb, greyScale, _SirenixOdin_GreyScale);

                // Change hue
                float3 h = col.rgb;
                h = rgb2hsv(h);
                float hue = rgb2hsv(_SirenixOdin_HueColor.rgb).x;
                h.x = hue;
                h = hsv2rgb(h);
                col.rgb = lerp(col.rgb, h, _SirenixOdin_HueColor.a);

                // Blend color
                col *= _SirenixOdin_GUIColor;

                return col;
            }
            ENDCG
        }
    }
}