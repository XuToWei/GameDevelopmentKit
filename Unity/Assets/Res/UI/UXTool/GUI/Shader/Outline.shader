Shader "L28/UI/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        //Soft Mask Required
        [PerRendererData] _SoftMaskTex("Soft Mask Texture", 2D) = "white" {}
        _MaskInteraction("Mask Interaction", Vector) = (0,0,0,0)
        //UI Mask Required
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
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

        Stencil 
        {
            Ref [_Stencil]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilComp]
            Pass [_StencilOp]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            //#include "SoftMask.cginc"
            #pragma shader_feature __ SOFTMASK_EDITOR
            //#pragma multi_compile_local __ SOFTMASK

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            struct appdata_t
            {
                half4 vertex   : POSITION;
                half4 color    : COLOR;
                half4 tangent : TANGENT;
                half4 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                half2 uv3 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                half4  mask : TEXCOORD2;
                float2 uv1 : TEXCOORD3;
                float2 uv2 : TEXCOORD4;
                fixed4 outlineColor : TEXCOORD5;
                half outlineWidth : TEXCOORD6;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            half4 _ClipRect;
            float4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            half _UIMaskSoftnessX;
            half _UIMaskSoftnessY;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                #ifdef UNITY_UI_CLIP_RECT
                    half2 pixelSize = vPosition.w;
                    pixelSize /= half2(1, 1) * abs(mul((half2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                    half4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                    half2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                    OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));
                #else
                    OUT.mask = half4(0,0,0,0);
                #endif

                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                OUT.color = v.color;
                OUT.uv1 = v.uv1;
                OUT.uv2 = v.uv2;
                OUT.outlineColor = fixed4(v.uv3.xy, v.tangent.zw);
                OUT.outlineWidth = v.normal.z * _ScreenParams.y/720.0 * ((_ScreenParams.x/_ScreenParams.y)/(1624.0/720.0));
                #ifdef UNITY_COLORSPACE_GAMMA
                #else
                    OUT.outlineColor.rgb = GammaToLinearSpace(OUT.outlineColor.rgb);
                #endif
                return OUT;
            }

            half IsInRect(float2 pPos, float2 pClipRectMin, float2 pClipRectMax) //避免half精度step相乘的编译bug
            {
                pPos = step(pClipRectMin, pPos) * step(pPos, pClipRectMax);
                return pPos.x * pPos.y;
            }

            half SampleAlpha(int pIndex, v2f IN)
            {
                const half sinArray[12] = { 0, 0.5, 0.866, 1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5 };
                const half cosArray[12] = { 1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5, 0, 0.5, 0.866 };
                half2 pos = IN.texcoord + _MainTex_TexelSize.xy * half2(cosArray[pIndex], sinArray[pIndex]) * IN.outlineWidth;
                return IsInRect(pos, IN.uv1, IN.uv2) * (tex2D(_MainTex, pos) + _TextureSampleAdd).a;		
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * half4(IN.color.rgb, IsInRect(IN.texcoord, IN.uv1, IN.uv2));
                half outlineAlpha = 0;
                outlineAlpha += SampleAlpha(0, IN);
                outlineAlpha += SampleAlpha(1, IN);
                outlineAlpha += SampleAlpha(2, IN);
                outlineAlpha += SampleAlpha(3, IN);
                outlineAlpha += SampleAlpha(4, IN);
                outlineAlpha += SampleAlpha(5, IN);
                outlineAlpha += SampleAlpha(6, IN);
                outlineAlpha += SampleAlpha(7, IN);
                outlineAlpha += SampleAlpha(8, IN);
                outlineAlpha += SampleAlpha(9, IN);
                outlineAlpha += SampleAlpha(10, IN);
                outlineAlpha += SampleAlpha(11, IN);
                outlineAlpha = saturate(outlineAlpha) * IN.outlineColor.a;
                color.rgb = lerp(IN.outlineColor.rgb * outlineAlpha, color.rgb, color.a) / max(lerp(outlineAlpha, 1, color.a), 0.01);
                color.a = (outlineAlpha + color.a - outlineAlpha * color.a) * IN.color.a;
                
                #ifdef UNITY_UI_CLIP_RECT
                    half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                    color.a *= m.x * m.y;
                #endif

                // #if SOFTMASK
                //     color.a *= SoftMask(IN.vertex, IN.worldPosition);
                // #endif

                return color;
            }
            ENDCG
        }
    }
}
