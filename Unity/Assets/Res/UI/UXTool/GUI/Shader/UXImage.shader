// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// changed by gdk
Shader "UI/UXImage"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}

        [Toggle(IS_CALALPHA)] _IsCalAlpha ("_IsCalAlpha", float) = 0
        _SelfUV ("_SelfUV", Vector) = (0,0,1,1)
        _AlphaUV ("_AlphaUV", Vector) = (0,0,1,1)

        _Color ("Tint", Color) = (1,1,1,1)
        

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
         [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("_SrcBlend", Int) = 5
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("_DstBlend", Int) = 10
       
        [HideInInspector]_Blend ("_blend", Int) = 0
       
        [Toggle(GAMMA_TO_LINEAR)] _GammaToLinear ("Gamma to Linear", Float) = 0
        [Toggle(LINEAR_TO_GAMMA)] _LinearToGamma ("Linear to Gamma", Float) = 0
        
        
        [Toggle(ENABLE_GREY)] _EnableGrey ("Enable Grey", Float) = 0
        _Contrast("Contrast", Range(0,2)) = 1
        _Saturation("Saturation", Range(0,2)) = 0  

        
        [Toggle(RADIUS_CORNER)] _RadiusCorner ("Radius Corner", Float) = 0
         // Definition in Properties section is required to Mask works properly
        _r ("r", Vector) = (0,0,0,0)
        _halfSize ("halfSize", Vector) = (0,0,0,0)
        _rect2props ("rect2props", Vector) = (0,0,0,0)
        _AtlasPosition("atlasUVRect", vector) = (0,0,1,1)
        _hasTex ("has Tex", Float) = 1
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
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        //Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Assets/Res/UI/UXTool/GUI/Shader/ShaderLibrary/SDFUtils.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #pragma multi_compile_local _ LINEAR_TO_GAMMA
            #pragma multi_compile_local _ GAMMA_TO_LINEAR
            #pragma multi_compile_local _ ENABLE_GREY
            #pragma multi_compile_local _ RADIUS_CORNER
            #pragma multi_compile_local _ IS_CALALPHA
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float4 _SelfUV;
            float4 _AlphaUV;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            float4 _MainTex_TexelSize;
            fixed _Saturation;
            fixed _Contrast;
            float4 _r;
            float4 _halfSize;
            float4 _rect2props;
            float4 _AtlasPosition;
            float _hasTex;

            inline float2 NormalizeAtlasSpriteUV(float2 uv)
            {
            	float4 atlasOffsetScale = _AtlasPosition * _MainTex_TexelSize.xyxy;
	            float2 fixUV = (uv - atlasOffsetScale.xy) / atlasOffsetScale.zw;
                return fixUV;
            }

            inline fixed4 mixAlpha(fixed4 col, float sdfAlpha){
                col.a = min(col.a, sdfAlpha);
                return col;
            }

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                half4 color = IN.color * (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                #ifdef ENABLE_GREY
                //saturation调整饱和度
                fixed gray = 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;  
                fixed3 grayColor = fixed3(gray, gray, gray);  
                color.rgb = lerp(grayColor, color, _Saturation);  
                //contrast调整对比度
                fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
                
                color.rgb = lerp(avgColor, color, _Contrast);
                #endif

                #ifdef LINEAR_TO_GAMMA
                color.rgb = LinearToGammaSpace(color.rgb);
                #endif

                #ifdef GAMMA_TO_LINEAR
                color.rgb = GammaToLinearSpace(color.rgb);
                color.a = pow(color.a, 1 / 2.2f);
                #endif

                #ifdef RADIUS_CORNER
                if (color.a <= 0) {
                    return color;
                }

                float2 uv = _hasTex * NormalizeAtlasSpriteUV(IN.texcoord) + (1 - _hasTex) * IN.texcoord;
                //float2 uv = _hasTex * IN.texcoord;
                float alpha = CalcAlphaForIndependentCorners(uv, _halfSize.xy, _rect2props, _r);

                #ifdef UNITY_UI_ALPHACLIP
                clip(alpha - 0.001);
                #endif
                
                return mixAlpha(color, alpha);
                #endif
                #ifdef IS_CALALPHA
                //todo
                float needCalSign = step(_SelfUV.x, IN.texcoord.x)*step(IN.texcoord.x,_SelfUV.z)
                                *step(_SelfUV.y, IN.texcoord.y)*step(IN.texcoord.y ,_SelfUV.w);
                float xrate = (IN.texcoord.x - _SelfUV.x)/(_SelfUV.z-_SelfUV.x);
                float yrate = (IN.texcoord.y - _SelfUV.y)/(_SelfUV.w-_SelfUV.y);
                float2 alphauvs = float2(xrate*(_AlphaUV.z-_AlphaUV.x)+_AlphaUV.x, yrate*(_AlphaUV.w-_AlphaUV.y)+_AlphaUV.y);
                fixed4 alphaColor = tex2D(_AlphaTex, alphauvs);
                color.a *= alphaColor.a;

                #endif
                return color;
            }
        ENDCG
        }
    }
}
