Shader "Hidden/Sirenix/SpinningShader"
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

            uniform bool _ManualTex2SRGB;
            float4 _SirenixOdinSpinner_Shape;
            float _SirenixOdinSpinner_SpinDir;
            float _SirenixOdinSpinner_Spin;
            float4 _SirenixOdinSpinner_Color;
            float _SirenixOdinSpinner_SpinTime;
            float _SirenixOdinSpinner_T;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            #define SPINSPEED 20.0
            #define TIME _SirenixOdinSpinner_SpinTime * 0.3
            #define SPIN (TIME * SPINSPEED)
            #define PI 3.1415926535
            #define RADIUS 0.5

            float easeOutSine(float x) { return sin((x * PI) / 2.); }
            float ease(float x) { return x * x * (3.0 - 2.0 * x); }
            float anim(float x) { return pow(ease(x), 1.0); }

            float2 rotate(in float2 p, float r) {
                float c = cos(r);
                float s = sin(r);
                float2x2 m = float2x2(c, -s, s, c);
                p = mul(m, p);
                return p;
            }

            float sdOctogon(in float2 p, in float r) {
                float3 k = float3(-0.9238795325, 0.3826834323, 0.4142135623);
                p = abs(p);
                p -= 2.0 * min(dot(float2(k.x, k.y), p), 0.0) * float2(k.x, k.y);
                p -= 2.0 * min(dot(float2(-k.x, k.y), p), 0.0) * float2(-k.x, k.y);
                p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
                return length(p) * sign(p.y);
            }

            float sdUnevenCapsule(float2 p, float r1, float r2, float h) {
                p.x = abs(p.x);
                float b = (r1 - r2) / h;
                float a = sqrt(1.0 - b * b);
                float k = dot(p, float2(-b, a));
                if (k < 0.0) return length(p) - r1;
                if (k > a * h) return length(p - float2(0.0, h)) - r2;
                return dot(p, float2(a, b)) - r1;
            }

            float sdArc(in float2 p, in float2 sc, in float ra, float rb) {
                p.x = abs(p.x);
                return ((sc.y * p.x > sc.x * p.y) ? length(p - sc * ra) : abs(length(p) - ra)) - rb;
            }

            float sdBox(in float2 p, in float2 b) {
                float2 d = abs(p) - b;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
            }

            float sdEquilateralTriangle(in float2 p) {
                const float k = sqrt(3.0);
                p.x = abs(p.x) - 1.0;
                p.y = p.y + 1.0 / k;
                if (p.x + k * p.y > 0.0) p = float2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
                p.x -= clamp(p.x, -2.0, 0.0);
                return -length(p) * sign(p.y);
            }

            float sdTriangleIsosceles(in float2 p, in float2 q) {
                p.x = abs(p.x);
                float2 a = p - q * clamp(dot(p, q) / dot(q, q), 0.0, 1.0);
                float2 b = p - q * float2(clamp(p.x / q.x, 0.0, 1.0), 1.0);
                float s = -sign(q.y);
                float2 d = min(float2(dot(a, a), s * (p.x * q.y - p.y * q.x)),
  float2(dot(b, b), s * (p.y - q.y)));
                return -sqrt(d.x) * sign(d.y);
            }

            float sdExclamationPoint(in float2 p, float t) {
                float a = anim(t);

                float r1 = RADIUS * 0.08 * a;
                float r2 = RADIUS * 0.16 * a;
                float h = RADIUS * 0.45 * a;

                float t1 = length(p) - RADIUS * 0.1;
                float t2 = length(p) - RADIUS * 0.1;

                p.y += h * 0.2;

                t1 = sdUnevenCapsule(p, r1, r2, h);

                t2 = length(p + float2(0., h * 0.8)) - r2;

                return min(t1, t2);
            }

            float sdEquilateralTriangle(in float2 p, float r) {
                float2 p1 = float2(0., 1.);
                float2 p2 = float2(0.866, -.5);
                float2 p3 = float2(-0.866, -.5);
                p.y -= 0.99 * r;
                return sdTriangleIsosceles(p, float2(r * 0.85, -r * 1.48));
            }


            // SPINNER
            float spinnerDist(float2 p) {
                float rad = RADIUS * 0.9;
                p = rotate(p, SPIN);

                float t = PI * 1.57;
                float c = cos(t);
                float s = sin(t);
                float f = RADIUS * 0.25 * _SirenixOdinSpinner_T;
                return sdArc(p, float2(c, s), rad - 0.15 * RADIUS - f, 0.15 * RADIUS + f);
            }

            float spinnerAlpha(float2 p) {
                p = rotate(p, SPIN);
                return (atan2(p.x, p.y) + PI) / (PI * 2.0);
            }

            // Animate from/to spin util
            void spinAnimate(inout float2 p) {
                float t = 0.0;

                if (_SirenixOdinSpinner_SpinDir > 0.0)
                    t = -pow(_SirenixOdinSpinner_Spin, 2.0);
                else
                    t = pow(_SirenixOdinSpinner_Spin, 2.0);

                p = rotate(p, t * SPINSPEED * 0.2);
            }

            // CHECK
            float validDist(float2 p, float t) {
                float a = anim(t);

                float cr = 1.25 * a;
                float th = RADIUS * 0.00 * cr;
                float a1 = RADIUS * 0.15 * cr;
                float a2 = RADIUS * 0.32 * cr;

                float2 p1 = p - float2(RADIUS * cr * -0.2, RADIUS * cr * -0.15);
                float2 offset1 = float2(0, 0);
                float2 offset2 = float2(-a1 + th, -a2 + th);

                float t1 = sdBox(rotate(p1, PI * -0.25) + offset1, float2(th, a1)) - RADIUS * 0.095;
                float t2 = sdBox(rotate(p1, PI * +0.25) + offset2, float2(th, a2)) - RADIUS * 0.095;
                float t3 = length(p) - RADIUS;

                return max(t3, -min(t1, t2));
            }

            // WARNING
            float warningDist(float2 p, float t) {
                float s = 0.9;
                float t1 = sdExclamationPoint(p, t);
                float t2 = sdTriangleIsosceles(float2(p.x, -p.y + RADIUS * 0.9 * s), float2(RADIUS, RADIUS * 1.6) * s) - 0.2 * RADIUS;
                float t3 = length(p) - RADIUS;

                t2 = lerp(t2, t3, easeOutSine(_SirenixOdinSpinner_Spin));

                //t2 = length(p) - RADIUS;
                return max(t2, -t1);
            }

            // ERROR
            float errorDist(float2 p, float t) {
                float s = 0.9;
                float t1 = sdExclamationPoint(p, t);
                float t2 = sdOctogon(p, RADIUS);
                return max(t2, -t1);
            }

            // Odin
            float2 rotateArm(inout float2 uv) {
                float r = PI * -0.7428;
                float c = cos(PI * r);
                float s = sin(PI * r);
                float2x2 m = float2x2(c, -s, s, c);
                uv = mul(m, uv);
                uv.x = -uv.x;
                uv.y = -uv.y;
                return uv;
            }

            float sdTri(in float2 p, float size) {
                return sdEquilateralTriangle(p) + 0.6199999 - size;
            }

            float sdTriArm(in float2 p, float t) {
                float spacing = 0.9;
                float thickness = 0.6 * spacing;
                float s = 0.22;
                p.y -= s * 0.33 * 0.7 * spacing;
                p.x -= s * 0.6 * 0.7 * spacing;
                float outerTri = sdTri(p, s);
                float innerTri = -sdTri(p, s * spacing * 0.6);
                float sideTri1 = -sdTri(p - float2(s * -0.7, s * 0.656), s * 0.82);
                float sideTri2 = -sdTri((mul(p, float2x2(-1.0, 0, 0, -1.0))) - float2(s * 1.2, s * 0.4), s * 0.8);

                float f = outerTri;

                f = max(f, innerTri);
                f = max(f, sideTri1);
                f = max(f, sideTri2);
                return f + 0.00;
            }

            float odinDist(float2 p, float t) {
                p.y += 0.04;
                float arm1 = sdTriArm(rotateArm(p), t);
                float arm2 = sdTriArm(rotateArm(p), t);
                float arm3 = sdTriArm(rotateArm(p), t);
                return min(min(arm1, arm2), arm3) - 0.02;
            }

            float4 frag(v2f i) : SV_Target {
                float2 p1 = (i.uv - float2(0.5, 0.5)) * 1.0;
                float2 p2 = p1;

                spinAnimate(p2);

                float4 s = _SirenixOdinSpinner_Shape;

                float dValid = validDist(p2, s.y) * s.y;
                float dWarning = warningDist(p2, s.z) * s.z;
                float dError = errorDist(p2, s.w) * s.w;
                float d = dValid + dWarning + dError;
                float spin = _SirenixOdinSpinner_Spin;

                d = lerp(d, spinnerDist(p1), spin);
                d = lerp(d, odinDist(p1 * 0.75, s.x), s.x);

                float spinAlpha = spinnerAlpha(p1) * spin;
                float4 col = _SirenixOdinSpinner_Color;
                col.a *= lerp(1, spinAlpha, spin);
                col.a *= smoothstep(0., -abs(ddx(i.uv.x + i.uv.y)) * 1.0, d);
                col.r = clamp(0, 1, col.r);
                col.g = clamp(0, 1, col.g);
                col.b = clamp(0, 1, col.b);

                //if (_ManualTex2SRGB) col.rgb = LinearToGammaSpace(col.rgb);

                return col * col.a;
            }
            ENDCG
        }
    }
}