// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "QFSW/Blur"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _OverlayColor("Overlay Color", Color) = (1,1,1,1)
        _MainTex("Tint Color (RGB)", 2D) = "white" {}
        _Radius("Radius", Range(0, 20)) = 1
    }

    Category
    {

    Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
    Blend SrcAlpha OneMinusSrcAlpha
    SubShader{

    GrabPass{
    Tags{ "LightMode" = "Always" }
    }
    Pass{
    Tags{ "LightMode" = "Always" }

    CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

    struct appdata_t
    {
        float4 vertex : POSITION;
        float2 texcoord: TEXCOORD0;
        fixed4 color : COLOR;
    };

    struct v2f
    {
        float4 vertex : POSITION;
        float4 uvgrab : TEXCOORD0;
        fixed4 color : COLOR;
    };

    v2f vert(appdata_t v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
        float scale = -1.0;
#else
        float scale = 1.0;
#endif
        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
        o.uvgrab.zw = o.vertex.zw;
        o.color = v.color;
        return o;
    }

    sampler2D _GrabTexture;
    float4 _GrabTexture_TexelSize;
    uniform float _Radius;
    uniform float _BlurMultiplier = 1;

    half4 frag(v2f i) : COLOR
    {
        _Radius *= _BlurMultiplier;
        _Radius *= i.color.a;
        half4 sum = half4(0,0,0,0);
#define GRABPIXEL(weight, kernelx) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx * _Radius, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
        sum += GRABPIXEL(0.05, -4.0);
        sum += GRABPIXEL(0.09, -3.0);
        sum += GRABPIXEL(0.12, -2.0);
        sum += GRABPIXEL(0.15, -1.0);
        sum += GRABPIXEL(0.18,  0.0);
        sum += GRABPIXEL(0.15, +1.0);
        sum += GRABPIXEL(0.12, +2.0);
        sum += GRABPIXEL(0.09, +3.0);
        sum += GRABPIXEL(0.05, +4.0);

        return sum;
    }
        ENDCG
    }

    GrabPass
    {
        Tags{ "LightMode" = "Always" }
    }
    Pass{
        Tags{ "LightMode" = "Always" }

        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

        struct appdata_t 
        {
            float4 vertex : POSITION;
            float2 texcoord: TEXCOORD0;
            fixed4 color : COLOR;
        };

    struct v2f {
        float4 vertex : POSITION;
        float4 uvgrab : TEXCOORD0;
        fixed4 color : COLOR;
    };

    v2f vert(appdata_t v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
        float scale = -1.0;
#else
        float scale = 1.0;
#endif
        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
        o.uvgrab.zw = o.vertex.zw;
        o.color = v.color;
        return o;
    }

    sampler2D _GrabTexture;
    float4 _GrabTexture_TexelSize;
    uniform float _Radius;
    uniform float _BlurMultiplier = 1;

    half4 frag(v2f i) : COLOR{
        _Radius *= _BlurMultiplier;
        _Radius *= i.color.a;
        half4 sum = half4(0,0,0,0);
#define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Radius, i.uvgrab.z, i.uvgrab.w))) * weight

        sum += GRABPIXEL(0.05, -4.0);
        sum += GRABPIXEL(0.09, -3.0);
        sum += GRABPIXEL(0.12, -2.0);
        sum += GRABPIXEL(0.15, -1.0);
        sum += GRABPIXEL(0.18,  0.0);
        sum += GRABPIXEL(0.15, +1.0);
        sum += GRABPIXEL(0.12, +2.0);
        sum += GRABPIXEL(0.09, +3.0);
        sum += GRABPIXEL(0.05, +4.0);

        return sum;
    }
        ENDCG
    }

        GrabPass{
        Tags{ "LightMode" = "Always" }
    }
        Pass{
        Tags{ "LightMode" = "Always" }

        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

        struct appdata_t {
        float4 vertex : POSITION;
        float2 texcoord: TEXCOORD0;
        float4 color: COLOR;
    };

    struct v2f
    {
        float4 vertex : POSITION;
        float4 uvgrab : TEXCOORD0;
        float2 uvmain : TEXCOORD1;
        float4 color : COLOR;
    };

    float4 _MainTex_ST;

    v2f vert(appdata_t v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
        float scale = -1.0;
#else
        float scale = 1.0;
#endif
        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
        o.uvgrab.zw = o.vertex.zw;
        o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.color = v.color;
        return o;
    }

    fixed4 _Color;
    fixed4 _OverlayColor;
    sampler2D _GrabTexture;
    float4 _GrabTexture_TexelSize;
    sampler2D _MainTex;

    half4 frag(v2f i) : COLOR
    {
        half4 blurredCol = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab)); // Gets the color of what is behind it and blurred
        half4 texCol = tex2D(_MainTex, i.uvmain); // Gets the color of the supplied texture
        half4 tint = texCol * _Color; // Gets the tint color to apply

        half4 col = blurredCol * tint; // Tints the color
        col = col * (1 - _OverlayColor.a) + _OverlayColor * _OverlayColor.a; // Blends it with the overlay color
        col *= i.color;
        col = blurredCol * (1 - texCol.a) + col * texCol.a; // Blends it using the tex color so transparent regions stay transparent
        
        return col;
    }
        ENDCG
    }
    }
    }
}