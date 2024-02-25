Shader "UXTool/Color Blindness Effect" {
  Properties {
    _MainTex ("Base (RGB)", RECT) = "white" {}
    _RedTx ("Red Coefficients", Vector) = (1,0,0)
    _GreenTx ("Green Coefficients", Vector) = (0,1,0)
    _BlueTx ("Blue Coefficients", Vector) = (0,0,1)
  }

  SubShader {
    Tags { "RenderPipeline" = "UniversalPipeline" }
    GrabPass { }
    Pass {
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct Attributes {
        float4 positionHCS : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct Varyings {
        float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
      };

      Varyings vert (Attributes v) {
        Varyings o;
        o.positionCS = UnityObjectToClipPos(v.positionHCS.xyz);
        o.uv = v.uv;
        #if UNITY_UV_STARTS_AT_TOP
        o.uv.y = 1.0 - o.uv.y;
        #endif
        return o;
      }

      sampler2D _GrabTexture;
      float3 _RedTx, _GreenTx, _BlueTx;

      half4 frag(Varyings i) : SV_Target {
        half4 original = tex2D(_GrabTexture, i.uv);

        return half4(
          (original.r*_RedTx[0])  +(original.g*_RedTx[1])  +(original.b*_RedTx[2]),
          (original.r*_GreenTx[0])+(original.g*_GreenTx[1])+(original.b*_GreenTx[2]),
          (original.r*_BlueTx[0]) +(original.g*_BlueTx[1]) +(original.b*_BlueTx[2]),
          original.a
        );
      }
      ENDHLSL
    }
  }
}