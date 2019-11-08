Shader "Custom/Template"
{
    Properties
    {
        _iChannel0("iChannel0", 2D) = "white"{} 
        _iChannel1("iChannel1", 2D) = "white"{} 
        _iChannel2("iChannel2", 2D) = "white"{} 
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
            sampler2D _iChannel1;
            float4 _iChannel1_ST;
            sampler2D _iChannel2;
            float4 _iChannel2_ST;

            #include "UnityCG.cginc"
            
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
