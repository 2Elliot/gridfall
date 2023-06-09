Shader "Custom/LeafWind" {
    Properties{
    _MainTex("Leaf Texture", 2D) = "white" {}
    _WindForce("Wind Force", Range(0, 10)) = 1.0
    _Ambient("Ambient", Range(0, 1)) = 0.5
    }

        SubShader{
    Tags {"RenderType" = "Opaque"}
    LOD 200

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
            float3 viewDir : TEXCOORD1;
        };

        struct v2f {
            float2 uv : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
            float3 worldNormal : TEXCOORD2;
            float3 worldRefl : TEXCOORD3;
            float3 worldRefr : TEXCOORD4;
        };

        sampler2D _MainTex;
        float _WindForce;
        float _Ambient;

        v2f vert(appdata v) {
            v2f o;
            o.uv = v.uv;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.worldNormal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz);
            o.worldRefl = reflect(-v.viewDir, o.worldNormal);
            o.worldRefr = refract(v.viewDir, o.worldNormal, 1.0);
            return o;
        }

        float4 frag(v2f i) : SV_Target {
            float4 col = tex2D(_MainTex, i.uv);
            col.a *= 1 - step(0.1, col.a);
            col.rgb = lerp(col.rgb * _Ambient, col.rgb, _WindForce);
            return col;
        }
        ENDCG
    }
    }

        FallBack "Diffuse"
}