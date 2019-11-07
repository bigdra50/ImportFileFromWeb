Shader "Custom/ImportShaderToyExample"
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
           float square(vec2 r, vec2 bottomLeft, float side) {
	vec2 p = r - bottomLeft;
	return ( p.x > 0.0 && p.x < side && p.y>0.0 && p.y < side ) ? 1.0 : 0.0;
}

float character(vec2 r, vec2 bottomLeft, float charCode, float squareSide) {
	vec2 p = r - bottomLeft;
	float ret = 0.0;
	float num, quotient, remainder, divider;
	float x, y;	
	num = charCode;
	for(int i=0; i<20; i++) {
		float boxNo = float(19-i);
		divider = pow(2., boxNo);
		quotient = floor(num / divider);
		remainder = num - quotient*divider;
		num = remainder;
		
		y = floor(boxNo/4.0); 
		x = boxNo - y*4.0;
		if(quotient == 1.) {
			ret += square( p, squareSide*vec2(x, y), squareSide );
		}
	}
	return ret;
} 
            
            
mat2 rot(float th) { return mat2(cos(th), -sin(th), sin(th), cos(th)); }
            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = 1;
               	float G = 990623.; // compressed characters :-)
	float L = 69919.;
	float S = 991119.;
	
	float t = iTime;

	vec2 r = (fragCoord.xy - 0.5*iResolution.xy) / iResolution.y;
	//vec2 rL = rot(t)*r+0.0001*t;
	//vec2 rL = r+vec2(cos(t*0.02),sin(t*0.02))*t*0.05;
	float c = 0.05;//+0.03*sin(2.5*t);
	vec2 pL = (mod(r+vec2(cos(0.3*t),sin(0.3*t)), 2.0*c)-c)/c;
	float circ = 1.0-smoothstep(0.75, 0.8, length(pL));
	//vec2 rG = rot(2.*3.1415*smoothstep(0.,1.,mod(1.5*t,4.0)))*r;
	vec2 rG = mul(rot(2.*3.1415*smoothstep(0.,1.,mod(1.5*t,4.0))),r);   // 回転行列は*演算子ではなくmulで乗算
	//vec2 rStripes = rot(0.2)*r;
	vec2 rStripes = mul(rot(0.2), r);   // 回転
				
	float xMax = 0.5*iResolution.x/iResolution.y;
	float letterWidth = 2.0*xMax*0.9/4.0;
	float side = letterWidth/4.;
	float space = 2.0*xMax*0.1/5.0;
	
	r += 0.001; // to get rid off the y=0 horizontal blue line.
	float maskGS = character(r, vec2(-xMax+space, -2.5*side)+vec2(letterWidth+space, 0.0)*0.0, G, side);
	float maskG = character(rG, vec2(-xMax+space, -2.5*side)+vec2(letterWidth+space, 0.0)*0.0, G, side);
	float maskL1 = character(r, vec2(-xMax+space, -2.5*side)+vec2(letterWidth+space, 0.0)*1.0, L, side);
	float maskSS = character(r, vec2(-xMax+space, -2.5*side)+vec2(letterWidth+space, 0.0)*2.0, S, side);
	float maskS = character(r, vec2(-xMax+space, -2.5*side)+vec2(letterWidth+space, 0.0)*2.0 + vec2(0.01*sin(2.1*t),0.012*cos(t)), S, side);
	float maskL2 = character(r, vec2(-xMax+space, -2.5*side)+vec2(letterWidth+space, 0.0)*3.0, L, side);
	float maskStripes = step(0.25, mod(rStripes.x - 0.5*t, 0.5));
	
	float i255 = 0.00392156862;
	vec3 blue = vec3(43., 172., 181.)*i255;
	vec3 pink = vec3(232., 77., 91.)*i255;
	vec3 dark = vec3(59., 59., 59.)*i255;
	vec3 light = vec3(245., 236., 217.)*i255;
	vec3 green = vec3(180., 204., 18.)*i255;

	vec3 pixel = blue;
	pixel = mix(pixel, light, maskGS);
	pixel = mix(pixel, light, maskSS);
	pixel -= mul(0.1,maskStripes);	
	pixel = mix(pixel, green, maskG);
	pixel = mix(pixel, pink, mul(maskL1,circ));
	pixel = mix(pixel, green, maskS);
	pixel = mix(pixel, pink, maskL2*(1.-circ));
	
	float dirt = pow(texture(iChannel0, mul(4.0,r)).x, 4.0);
	pixel -= mul((0.2*dirt - 0.1),(maskG+maskS)); // dirt
	pixel -= smoothstep(0.45, 2.5, length(r));
	fragColor = vec4(pixel, 1.0); 
                return col;
            }
            ENDCG
        }
    }
}
