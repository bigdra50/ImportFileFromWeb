using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// glslのファイルをshaderlabのファイルへ変換する
/// </summary>
public class Converter
{
    private readonly string _glslFilePath = Path.Combine(Application.dataPath, "GLSL2ShaderLab", "Shader", "PietMondrianPaint.glsl");
    private readonly string _shaderPath = Path.Combine(Application.dataPath, "GLSL2ShaderLab", "Shader");
    private string _shaderName = string.Empty;
    
    private string _codeInsertRage = @"@block\s*(?<Block>[^\s\n]+)\s*\n(?<Value>[\s\S]*?)\n\s*(:?//\s*)*?@endblock";
    // nyaaaaan
    List<string> _shaderLabTemplate = new List<string>()
    {
        @"Shader ""Custom/Template""
        {
        Properties
        {
            _iChannel0(""iChannel0"", 2D) = ""white""{} 
            _iChannel1(""iChannel1"", 2D) = ""white""{} 
            _iChannel2(""iChannel2"", 2D) = ""white""{} 
        }
        SubShader
        {
        Tags { ""RenderType""=""Opaque"" }
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
        
        #include ""UnityCG.cginc""
                    
            float mod(float x, float y)
            {
                return x-y*floor(x/y);
            }"
        
    ,@"                
                return fragColor;
            }
            ENDCG
        }
    }
}",
    };


    Dictionary<string, string> _glslHlslDic = new Dictionary<string, string>()
    {
        {"void mainImage( out vec4 fragColor, in vec2 fragCoord )", "fixed4 frag(v2f_img i) : SV_Target"},
        {"void mainImage(out vec4 fragColor, in vec2 fragCoord )", "fixed4 frag(v2f_img i) : SV_Target"},
        {"void mainImage( out vec4 fragColor,in vec2 fragCoord )", "fixed4 frag(v2f_img i) : SV_Target"},
        {"void mainImage(out vec4 fragColor,in vec2 fragCoord )", "fixed4 frag(v2f_img i) : SV_Target"},
        {"void mainImage( out vec4 fragColor, in vec2 fragCoord)", "fixed4 frag(v2f_img i) : SV_Target"},
        {"void mainImage(out vec4 fragColor, in vec2 fragCoord)", "fixed4 frag(v2f_img i) : SV_Target"},
        {"void mainImage( out vec4 fragColor,in vec2 fragCoord)", "fixed4 frag(v2f_img i) : SV_Target"},
        {"void mainImage(out vec4 fragColor,in vec2 fragCoord)", "fixed4 frag(v2f_img i) : SV_Target"},
        {"vec2", "float2"},
        {"vec3", "float3"},
        {"vec4", "float4"},
        {"mat3", "float3x3"},
        {"mat4", "float4x4"},
        {"mat3x4", "float3x4"},
        {"ivec2", "int2"},
        {"ivec3", "int3"},
        {"ivec4", "int4"},
        {"uvec2", "uint2"},
        {"uvec3", "uint3"},
        {"uvec4", "uint4"},
        {"bvec2", "bool2"},
        {"bvec3", "bool3"},
        {"bvec4", "bool4"},
        {"lowp float", "min10float"},
        {"midiump float", "half(min16float)"},
        {"highp float", "float"},
        {"highp int", "int"},
        {"highp uint", "uint"},
        {"midiump uint", "min16uint"},
        {"midiump int", "min16int"},
        {"lowp int", "min12int"},
        {"lowp uint", "min12int"},
        {"inversesqrt(", "rsqrt("},
        {"mix(", "lerp("},
        {"mod(", "fmod("},    // ちょっと違うので注意
        {"texture(", "tex2D("},
        {"texture2D(", "tex2D("},
        {"fract(", "frac("},
        {"atan(", "atan2("},
        {"iTime", "_Time.y"},
        {"iMouse.xy", "float2(1.0, 1.0)"},    // TODO: マウス入力
        {"iMouse.x", "1"},    
        {"iMouse.y", "1"},   
        {"iResolution", "_ScreenParams"},
        {"fragCoord", "i.uv*_ScreenParams"},
        {"iChannel0", "_iChannel0"},
        {"iChannel1", "_iChannel1"},
        {"iChannel2", "_iChannel2"},
    };

    public Converter()
    {
        
    }

    public Converter(string fileName)
    {
        this._shaderName = fileName;
    }
    public void Start()
    {
        var hlslLines = File.ReadLines(_glslFilePath, Encoding.UTF8)
            .Select(s => s = ReplaceGlsl2Hlsl(s));

        using (var writer = new StreamWriter(Path.Combine(_shaderPath, _shaderName)))
        {
            writer.WriteLine(_shaderLabTemplate.First());
            var cnt = 0;
            foreach (var hlslLine in hlslLines)
            {
                if (cnt < 44)
                {
                    writer.WriteLine(hlslLine);
                }
                cnt++;
            }
            writer.WriteLine(_shaderLabTemplate.Last());
            AssetDatabase.ImportAsset(FileUtil.GetProjectRelativePath(Path.Combine(_shaderPath, _shaderName)));
        }
        
        // if (File.Exists(_glslFilePath))
        // {
        //     using (var reader = new StreamReader(_glslFilePath, Encoding.UTF8))
        //     {
        //         while (!reader.EndOfStream)
        //         {
        //             var glslLine = reader.ReadLine();
        //             var hlslLine = ReplaceGlsl2Hlsl(glslLine);
        //         }
        //     }
        // }
    }

    public string ReplaceGlsl2Hlsl(string glslLine)
    {
        var hlslLine = glslLine;
        // TODO: とりあえずdicから置き換えるだけのを実装
        foreach (var dic in _glslHlslDic)
        {
            if (hlslLine.Contains(dic.Key))
            {
                hlslLine = hlslLine.Replace(dic.Key, dic.Value);
            }
        }
        
        
        return hlslLine;
    }
    
}
