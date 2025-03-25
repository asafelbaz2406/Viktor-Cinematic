Shader "Hexcore/RandomizeVertex"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _RandomAmount("Random Amount", Range(-0.5, 0.5)) = 0.01
        _ExtrudeAmount("Extrude Amount", Range(-0.1, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RandomAmount, _ExtrudeAmount;
            
            float rand(float3 seed)
            {
                return frac(sin(dot(seed, float3(12.9898, 78.233, 151.421))) * 43758.5453);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // Generate a unique random offset per vertex
                float randomOffset = rand(v.vertex.xyz) * _ExtrudeAmount * _RandomAmount ;
                
                // Move the vertex along its normal
                v.vertex.xyz += v.normal * randomOffset;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
