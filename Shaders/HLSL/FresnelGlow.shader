Shader "MyShaders/FresnelGlow"
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
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ NORMAL_MAP_ON
            #pragma multi_compile __ ERODE_EFFECT_ON
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                
                #if NORMAL_MAP_ON
                    float3 tangent : TEXCOORD3;
                    float3 bitangent : TEXCOORD4;
                #endif
            };

            sampler2D _MainTex, _NormalMap, _DistortionTex, _ErodeTex;
            float4 _MainTex_ST, _FresnelColor, _InvFresnelColor, _ErodeColor, _ErodeTex_ST;
            
            float _FresnelIntensity, _FresnelRamp, _DistortionIntensity;
            float _InvFresnelIntensity, _InvFresnelRamp, _RotateSpeed, _RevealValue, _Feather;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                
                #if NORMAL_MAP_ON
                    o.tangent = UnityObjectToWorldDir(v.tangent);
                    o.bitangent = cross(o.tangent, o.normal);
                #endif
                
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.uv, _ErodeTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distort = tex2D(_DistortionTex, i.uv + _Time.xx * _RotateSpeed).r;
                
                float3 finalNormal = i.normal;
                #if NORMAL_MAP_ON
                    float3 normalMap = UnpackNormal(tex2D(_NormalMap, i.uv));
                    finalNormal = normalMap.r * i.tangent + normalMap.g * i.bitangent + normalMap.b * finalNormal;
                #endif
                
                float fresnelAmount = 1 - max(0, dot(finalNormal, i.viewDir));
                fresnelAmount *= distort * _DistortionIntensity;
                fresnelAmount = pow(fresnelAmount, _FresnelRamp) * _FresnelIntensity;

                float3 fresnelColor = fresnelAmount * _FresnelColor;
                
                float invFresnelAmount = max(0, dot(finalNormal, i.viewDir));
                invFresnelAmount *= distort * _DistortionIntensity;
                invFresnelAmount = pow(invFresnelAmount, _InvFresnelRamp) * _InvFresnelIntensity;
                float3 invFresnelColor = invFresnelAmount * _InvFresnelColor;

                float3 finalColor = fresnelColor + invFresnelColor;

                fixed4 result;
                
                #if ERODE_EFFECT_ON
                    fixed4 mask = tex2D(_ErodeTex, i.uv.zw);

                    float revealAmountTop = step(mask.r, _RevealValue + _Feather);
                    float revealAmountBottom = step(mask.r, _RevealValue - _Feather);
                    float revealDifference = revealAmountTop - revealAmountBottom;
                    //float3 finalColor1 = lerp(finalColor.rgb, _ErodeColor, _RevealValue);
                    float3 finalColorErode = lerp(finalColor.rgb, _ErodeColor, revealDifference);
                
                    result =  fixed4(finalColorErode.rgb, (invFresnelAmount + fresnelAmount) * revealAmountTop);
                #else
                    result = fixed4(finalColor, invFresnelAmount + fresnelAmount);
                #endif

                return result;
                //return fixed4(finalColor, invFresnelAmount + fresnelAmount);
            }
            ENDCG
        }
    }
}
