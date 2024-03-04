Shader "Hidden/SceneHighlighting"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
            
        // 0
        Pass
        {            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 0, 0, 1);
            }
            ENDCG
        }
        
        // 1
        Pass
        {
            
            ZTest Always
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform sampler2D _BaseMap;

            uniform sampler2D _BaseMap2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_BaseMap, i.uv);
                fixed4 col2 = tex2D(_BaseMap2, i.uv);
                
				if (col.x == 0.0f && col.y == 0.0f && col.z == 0.0f && col.w == 0.0f)
				{
					float d = 0.299*col2.x + 0.587*col2.y + 0.184*col2.z;
					return float4(d, d, d, 1);
				}
				return col2 * float4(1, 0, 0, 1);
            }
            ENDCG
        }
        
        // 2
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform sampler2D _BaseMap;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_BaseMap, i.uv);
                return col;
            }
            ENDCG
        }
    }
}