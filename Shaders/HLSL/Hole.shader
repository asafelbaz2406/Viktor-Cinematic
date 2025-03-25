Shader "Hexcore/Hole"
{
    Properties
    {
        [HDR]_Color("Color",Color) = (1,1,1,0.5)    
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+2"}
        ColorMask RGB
        
        ZTest off
        ZWrite On
        Cull Front
        Lighting Off
          
            
        Stencil
        {
            Ref 1
            Comp equal
            Pass zero
            fail zero
            zfail zero
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex, _NoiseTex;
            float4 _MainTex_ST, _NoiseTex_ST;
            float4 _Color;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.uv.zw, _NoiseTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                fixed4 noiseCol = tex2D(_NoiseTex, i.uv.zw);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col * _Color * noiseCol;
            }
            ENDCG
        }
    }
}