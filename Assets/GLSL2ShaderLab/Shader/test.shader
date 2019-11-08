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
            float4 fragColor;
        
        #include "UnityCG.cginc"
                    
            float mod(float x, float y)
            {
                return x-y*floor(x/y);
            }

float3 rect(float2 bl, float2 tr, float2 st){
   float d = .02;
   float2 blInv = step(bl-d, st);
   float2 trInv = step(tr-d, 1.-st);
   float3 r2 = float3(blInv.x * trInv.x * blInv.y * trInv.y);
   r2 = 1. - r2;
   // bottom - left
   bl = step(bl, st);

   // top - right
   tr = step(tr, 1.-st);

   float3 r1 = float3(bl.x * tr.x * bl.y * tr.y);
   return r1 + r2;
}

fixed4 frag(v2f_img i) : SV_Target
{

    float2 uv = i.uv*_ScreenParams/_ScreenParams.xy;
    float4 col;
    col.rgb = float3(248./256., 241./256., 225./256.);
    float d = .02;

                
    float3 hLine1 = rect(float2(.0, .68), float2(.0, .18), uv);
    float3 hLine2 = rect(float2(.25, .08), float2(.0, .0), uv);
    float3 vLine1 = rect(float2(.25, .0), float2(.25, .0), uv);
    float3 vLine2 = rect(float2(.98, .0), float2(.0, .0), uv);
    float3 vLine3 = rect(float2(.08, .68), float2(.0, .0), uv);

    col.rgb *= hLine1 * hLine2 * vLine1 * vLine2 * vLine3;

    if(uv.x <= .25 && uv.y >= .68){
       col.gb = float2(0.);
    }
    if(uv.x >= .98 && uv.y >= .68){
       col.b = 0.;
    }
    if(uv.x >= .75 && uv.y <= .08){
       col.rg = float2(0.);
    }

                
                return fragColor;
            }
            ENDCG
        }
    }
}
