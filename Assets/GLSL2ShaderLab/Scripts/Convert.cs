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
}
