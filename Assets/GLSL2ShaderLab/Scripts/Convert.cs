using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Converter
{
    private string _glslFilePath = Path.Combine(Application.dataPath, "GLSL2ShaderLab", "Shader", "PietMondrianPaint.glsl");
    public void Start()
    {
        if (File.Exists(_glslFilePath))
        {
            using (var reader = new StreamReader(_glslFilePath, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    Debug.Log(line);
                }
            }
        }
    }
    
    Dictionary<string, string> _glslHlslDic = new Dictionary<string, string>()
    {
        {"vec2", "float2"},
        {"vec3", "vec3"},
        {"vec4", "vec4"},
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
        {"iResolution", "_ScreenParams"},
        {"fragCoord", "i.uv*_ScreenParams"},
        {"iChannel0", "_iChannel0"},
        {"iChannel1", "_iChannel1"},
        {"iChannel2", "_iChannel2"},
    };
}
