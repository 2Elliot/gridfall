Shader "Custom/GradientShader" {
    Properties{
        _Color1("Color 1", Color) = (1, 0, 0, 1)
        _Color2("Color 2", Color) = (0, 0, 1, 1)
        _GradientLevel("Gradient Level", Range(0, 1)) = 0.5
        _Brightness("Brightness", Range(0, 2)) = 1
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Cull Off
            Lighting Off
            ZWrite Off

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _Color1;
                float4 _Color2;
                float _GradientLevel;
                float _Brightness;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.vertex.xy * 0.5 + 0.5;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    // Gradient interpolation based on the gradient level
                    float2 gradientCoords = i.uv;
                    gradientCoords.y = lerp(_GradientLevel, 1.0, gradientCoords.y);
                    fixed4 gradientColor = lerp(_Color1, _Color2, gradientCoords.y);

                    // Adjust brightness
                    gradientColor.rgb *= _Brightness;

                    return gradientColor;
                }
                ENDCG
            }
    }
        FallBack "Diffuse"
}