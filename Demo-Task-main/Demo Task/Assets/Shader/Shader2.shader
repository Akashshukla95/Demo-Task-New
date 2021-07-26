Shader "Unlit/Shader2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            //fixed4 is r,g,b,alpha values
            fixed4 frag (v2f IN) : SV_Target
            {
                float a = 2.18;
                float b = 3.17;
                float c = a * b;
                float d = pow(a, c);
                
                // sample the texture
                //IN.uv tells the pixel's coordinates for the current mesh/texture
                //fixed4 col = tex2D(_MainTex, IN.uv);
                //shifting the pixel by some offset
                //raising the scene by 10% using 0.1
                //fixed4 col = tex2D(_MainTex, IN.uv + float2(0, 0.1));
                //now raising y axis proportional to the x axis
                //fixed4 col = tex2D(_MainTex, IN.uv + float2(0,sin(IN.vertex.x/100)/10));
                //moving sin wave
                fixed4 col = tex2D(_MainTex, IN.uv + float2(0,sin(IN.vertex.x/50 + _Time[3]/10)/50));
                //just invert colours
                //col = 1-col;
                //just forcing all the colours/pixels to be red(1)
                //col.b = 1;
                return col;
            }
            ENDCG
        }
    }
}
