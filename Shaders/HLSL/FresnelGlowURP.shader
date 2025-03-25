Shader "MyShaders/FresnelGlowURP"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _DistortionTex ("Distortion Texture", 2D) = "white" {}
        _DistortionIntensity("Distortion Intensity", Range(0,10)) = 0
        
        [HDR] _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelIntensity("Fresnel Intensity", Range(0,10)) = 0
        _FresnelRamp("Fresnel Ramp", Range(0,10)) = 0
        
        [HDR] _InvFresnelColor("Inverted Fresnel Color", Color) = (1,1,1,1)
        _InvFresnelIntensity("Inverted Fresnel Intensity", Range(0,10)) = 0
        _InvFresnelRamp("Inverted Fresnel Ramp", Range(0,10)) = 0
        
        _RotateSpeed("Rotate Speed", Range(0.0,1.0)) = 0
        
        [Toggle] NORMAL_MAP ("Normal Mapping", Float) = 0
        _NormalMap ("Normal Map", 2D) = "white" {}
        
        [Toggle] ERODE_EFFECT ("Erode Effect", Float) = 0
        _ErodeTex ("Erode Texture", 2D) = "white" {}
        _RevealValue("Reveal", Range(0,1)) = 0
        _Feather("Feather", Float) = 0
        [HDR] _ErodeColor("Erode Color",Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ NORMAL_MAP_ON
            #pragma shader_feature _ ERODE_EFFECT_ON
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                
                #if NORMAL_MAP_ON
                    float3 tangent : TEXCOORD3;
                    float3 bitangent : TEXCOORD4;
                #endif
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_DistortionTex); SAMPLER(sampler_DistortionTex);
            TEXTURE2D(_ErodeTex); SAMPLER(sampler_ErodeTex);

            float4 _MainTex_ST, _ErodeTex_ST, _FresnelColor, _InvFresnelColor, _ErodeColor;
            float _FresnelIntensity, _FresnelRamp, _DistortionIntensity;
            float _InvFresnelIntensity, _InvFresnelRamp, _RotateSpeed, _RevealValue, _Feather;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.normal = TransformObjectToWorldNormal(v.normal);
                
                #if NORMAL_MAP_ON
                    o.tangent = TransformObjectToWorldDir(v.tangent);
                    o.bitangent = cross(o.tangent, o.normal);
                #endif
                
                o.viewDir = normalize(GetWorldSpaceViewDir(v.vertex));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float distort = SAMPLE_TEXTURE2D(_DistortionTex, sampler_DistortionTex, i.uv + _Time.xx * _RotateSpeed).r;
                
                float3 finalNormal = i.normal;
                #if NORMAL_MAP_ON
                    float3 normalMap = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));
                    finalNormal = normalMap.r * i.tangent + normalMap.g * i.bitangent + normalMap.b * finalNormal;
                #endif
                
                float fresnelAmount = 1 - max(0, dot(finalNormal, i.viewDir));
                fresnelAmount *= distort * _DistortionIntensity;
                fresnelAmount = pow(fresnelAmount, _FresnelRamp) * _FresnelIntensity;

                float3 fresnelColor = fresnelAmount * _FresnelColor.rgb;
                
                float invFresnelAmount = max(0, dot(finalNormal, i.viewDir));
                invFresnelAmount *= distort * _DistortionIntensity;
                invFresnelAmount = pow(invFresnelAmount, _InvFresnelRamp) * _InvFresnelIntensity;
                float3 invFresnelColor = invFresnelAmount * _InvFresnelColor.rgb;

                float3 finalColor = fresnelColor + invFresnelColor;

                half4 result;

                #if ERODE_EFFECT_ON
                    float mask = SAMPLE_TEXTURE2D(_ErodeTex, sampler_ErodeTex, i.uv).r;
                    float revealAmountTop = step(mask, _RevealValue + _Feather);
                    float revealAmountBottom = step(mask, _RevealValue - _Feather);
                    float revealDifference = revealAmountTop - revealAmountBottom;
                    float3 finalColorErode = lerp(finalColor, _ErodeColor.rgb, revealDifference);
                
                    result = half4(finalColorErode, (invFresnelAmount + fresnelAmount) * revealAmountTop);
                #else
                    result = half4(finalColor, invFresnelAmount + fresnelAmount);
                #endif

                return result;
            }
            ENDHLSL
        }
    }
}
