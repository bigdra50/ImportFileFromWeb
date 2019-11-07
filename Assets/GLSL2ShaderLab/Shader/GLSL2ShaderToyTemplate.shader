Shader "Custom/Template"
{
    Properties
    {
        _iChannel0("iChannel0", 2D) = "white"{} 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            
            sampler2D _iChannel0;
            float4 _iChannel0_ST;

            #include "UnityCG.cginc"
            #define vec2 float2
            #define vec3 float3
            #define vec4 float4
            #define iTime _Time.y
            #define mix lerp
            #define fract frac
            #define mat2 float2x2
            #define mat3 float3x3
            #define mat4 float4x4
            #define iResolution _ScreenParams
            #define fragCoord i.uv*_ScreenParams
            #define fragColor col
            #define texture(img,texcoord) tex2D(img,texcoord)
            #define texture2D tex2D
            #define iChannel0 _iChannel0
            
            float mod(float x, float y)
            {
                return x-y*floor(x/y);
            }
            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 fragColor = 1;
                return fragColor;
            }
            ENDCG
        }
    }
}
