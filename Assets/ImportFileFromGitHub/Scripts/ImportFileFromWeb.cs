using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UniRx;
using UniRx.Async;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ImportFileFromWeb
{

    // For Debug
    private string _githubPath = "https://raw.githubusercontent.com";
    private string _gitHubHost = "raw.githubusercontent.com";
    private string _shaderToyPath = "www.shadertoy.com";
    private string _userName = "bigdra50";
    private string _filePath = "Shaders/master/ShaderLab/ATField.shader/";

    public async void StartImportAsync(string uriStr)
    {
        //var importFile = new ImportedFile();
        try
        {
            var srcPath = GetRawSourcePath(uriStr);
            if (srcPath == null) return;
            Debug.Log(srcPath);
            var importFile = await GetRawSourceAsync(srcPath);
            //var importFile = new ImportFile(Path.GetFileName(srcPath.ToString()), src);
            Import(importFile);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

    Uri GetRawSourcePath(string uriStr)
    {
        Uri uri;
        try
        {
            uri = new Uri(uriStr.TrimEnd(' ', '/'));
        }
        catch (Exception e)
        {
            throw (e);
            return null;
        }
        // uriの形式をいい感じにフォーマットする
        var fmt = new URLFormatter();
        uri = fmt.Format(uri);
        return uri;
    }

    async UniTask<ImportFile> GetRawSourceAsync(Uri uri)
    {
        var name = "";
        var src = "";
        var req = UnityWebRequest.Get(uri);
        await req.SendWebRequest();
        if (req.isHttpError || req.isNetworkError)
        {
            throw new Exception(req.error);
        }
        
        if (req.uri.Host == _githubPath)
        {
            // githubからならraw fileがとれてるはず
            return new ImportFile(Path.GetFileName(req.uri.AbsolutePath), req.downloadHandler.text);
        }else if (req.uri.Host == _shaderToyPath)
        {
            // shader toyからはjsonがくる
            Debug.Log("ShaderToy");
            var importedFile = new ImportFile("name", "src");
            var shaderToyData = JsonUtility.FromJson<ShaderToyData>(req.downloadHandler.text);
            
            Debug.Log("Serialized Json");
            Debug.Log(shaderToyData.Shader.ver);
            Debug.Log(shaderToyData.Shader.info.name);
            Debug.Log(shaderToyData.Shader.renderpass[0].code);

            src = shaderToyData.Shader.renderpass[0].code;
            return new ImportFile(shaderToyData.Shader.info.name, shaderToyData.Shader.renderpass[0].code, ImportFile.Extension.glsl);
        }

        return new ImportFile(name, src);
    }

    
    void Import(ImportFile importFile)
    {

        var dirPath = Path.Combine(Application.dataPath, @"Resources\PlayGround\");
        var filePath = "";
        //print(i.Src);
        if (importFile.Ext == ImportFile.Extension.cs)
        {
            dirPath = Path.Combine(dirPath, @"Scripts\");
            filePath = Path.Combine(dirPath, importFile.Name);
        }
        else if (importFile.Ext == ImportFile.Extension.shader)
        {
            dirPath = Path.Combine(dirPath, @"Shader\");
            filePath = Path.Combine(dirPath, importFile.Name);
        }
        else
        {
            dirPath = Path.Combine(dirPath, @"Text\");
            filePath = Path.Combine(dirPath, importFile.Name);

        }

        try
        {
            Directory.CreateDirectory(dirPath);
            using (var writer = new StreamWriter(filePath, false))
            {
                writer.Write(importFile.Src);
                writer.Flush();
                writer.Close();
                //File.WriteAllText(filePath, i.Src);
                AssetDatabase.ImportAsset(FileUtil.GetProjectRelativePath(filePath));
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}

public class ImportFile
{
    private readonly string name;
    private readonly string src;
    private readonly Extension ext;

    public string Name => name;
    public string Src => src;
    public Extension Ext => ext;

    public ImportFile(string name, string src)
    {
        // nameに拡張子が含まれてるやつ
        this.name = name.Replace(" ", "");
        this.src = src;
        var extension = Path.GetExtension(name).TrimStart('.');
        this.ext = extension == "cs" ? Extension.cs : extension == "shader" ? Extension.shader : Extension.other;
    }

    public ImportFile(string name, string src, Extension ext)
    {
        // nameには拡張子なし
        this.src = src;
        this.ext = ext;
        this.name = name.Replace(" ", "") + "." + ext.ToString();
    }

    public enum Extension
    {
        cs,
        shader,
        glsl,
        other,
    }
}

[Serializable]
public struct ShaderToyData
{
    public ShaderData Shader;
    
    [Serializable]
    public struct ShaderData
    {
        public float ver;
        public Info info;
        public RenderPass[] renderpass;
        
        [Serializable]
        public struct Info
        {
            public string id;
            public string name;
            public string username;
            public string description;
        }
        
        [Serializable]
        public struct RenderPass
        {
            public string code;
        }
    }
}





