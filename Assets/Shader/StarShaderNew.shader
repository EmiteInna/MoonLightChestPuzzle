Shader "Custom/CharaShaderNew" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _DarkTex("Texture2",2D)="white"{}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Transparency ("Transparency", Range(0.0, 1.0)) = 1
    }

    SubShader {
        Tags { "RenderMode"="Transparent"
        "Queue"="Transparent+15"}

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
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
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _Transparency;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
          //      clip(col.a-0.02f);
                col.rgb *= _Transparency;
                return col;
            }
            ENDCG
        }
    }
}