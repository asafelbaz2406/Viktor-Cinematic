Shader "Hexcore/LightningSRP"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Gradient ("Gradient Texture", 2D) = "white" {}

        _Stretch ("Stretch", Range(-2,2)) = 1.0
        _Offset ("Offset", Range(-2,2)) = 1.0
        _Speed ("Speed", Range(-2,2)) = 1.0

        [HDR] _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        Blend One OneMinusSrcAlpha
        ColorMask RGB
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5 // Ensure proper SRP support
            #pragma multi_compile_particles

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_Gradient); SAMPLER(sampler_Gradient);

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                float2 texcoord2 : TEXCOORD3;
            };

            // SRP Batcher Compatible CBUFFER
            CBUFFER_START(UnityPerMaterial)
            float4 _TintColor;
            float4 _MainTex_ST;
            float4 _Gradient_ST;
            float _Stretch;
            float _Offset;
            float _Speed;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz); // URP Function
                
                o.color = v.color;
                o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord2 = TRANSFORM_TEX(v.texcoord, _Gradient);

                // Custom Data from particle system
                o.texcoord.z = v.texcoord.z; // Lifetime
                o.texcoord.w = v.texcoord.w; // Offset
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Custom Data from particle system
                float lifetime = i.texcoord.z;
                float randomOffset = i.texcoord.w;

                // Fade the edges
                float gradientFalloff = smoothstep(0.99, 0.95, i.texcoord2.x) * smoothstep(0.99, 0.95, 1 - i.texcoord2.x);

                // Moving UVs
                float2 movingUV = float2(i.texcoord.x + randomOffset + (_Time.x * _Speed), i.texcoord.y);
                float tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, movingUV).r * gradientFalloff;

                // Cutoff for alpha
                float cutoff = step(lifetime, tex);

                // Stretched UV for gradient map
                float2 uv = float2(tex * _Stretch - lifetime + _Offset, 1);
                float4 colorMap = SAMPLE_TEXTURE2D(_Gradient, sampler_Gradient, uv);

                // Final color
                half4 col;
                col.rgb = colorMap.rgb * _TintColor.rgb * i.color.rgb;
                col.a = cutoff;
                col *= col.a; // Premultiplied alpha

                return col;
            }
            ENDHLSL
        }
    }
}
