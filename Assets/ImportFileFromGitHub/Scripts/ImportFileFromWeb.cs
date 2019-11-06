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
            var src = await GetRawSourceAsync(srcPath);
            Debug.Log(src);
            var importFile = new ImportedFile(srcPath.ToString(), src);
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

    async UniTask<string> GetRawSourceAsync(Uri uri)
    {
        var req = UnityWebRequest.Get(uri);
        await req.SendWebRequest();
        if (req.isHttpError || req.isNetworkError)
        {
            throw new Exception(req.error);
        }
        
        if (req.uri.Host == _githubPath)
        {
            // githubからならrawfileがとれてるはず
        }else if (req.uri.Host == _shaderToyPath)
        {
            // shader toyからはjsonがくる
            Debug.Log("ShaderToy");
            var importedFile = new ImportedFile("name", "src");
            var shaderToyData = JsonUtility.FromJson<ShaderToyData>(req.downloadHandler.text);
            
            Debug.Log("Serialized Json");
            Debug.Log(shaderToyData.Shader.ver);
            Debug.Log(shaderToyData.Shader.info.name);
            Debug.Log(shaderToyData.Shader.renderpass[0].code);
            
        }

        return req.downloadHandler.text;
    }

    void Import(ImportedFile importedFile)
    {

        var dirPath = Path.Combine(Application.dataPath, @"Resources\PlayGround\");
        var filePath = "";
        //print(i.Src);
        if (importedFile.Ext == ImportedFile.Extension.cs)
        {
            dirPath = Path.Combine(dirPath, @"Scripts\");
            filePath = Path.Combine(dirPath, importedFile.Name);
        }
        else if (importedFile.Ext == ImportedFile.Extension.shader)
        {
            dirPath = Path.Combine(dirPath, @"Shader\");
            filePath = Path.Combine(dirPath, importedFile.Name);
        }
        else
        {
            dirPath = Path.Combine(dirPath, @"Text\");
            filePath = Path.Combine(dirPath, importedFile.Name);

        }

        try
        {
            Directory.CreateDirectory(dirPath);
            using (var writer = new StreamWriter(filePath, false))
            {
                writer.Write(importedFile.Src);
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

public class ImportedFile
{
    private readonly string name;
    private readonly string src;
    private readonly Extension ext;

    public string Name => name;
    public string Src => src;
    public Extension Ext => ext;

    public ImportedFile(string path, string src)
    {
        this.name = Path.GetFileName(path);
        this.src = src;
        var extension = Path.GetExtension(name).TrimStart('.');
        this.ext = extension == "cs" ? Extension.cs : extension == "shader" ? Extension.shader : Extension.other;
    }

    public enum Extension
    {
        cs,
        shader,
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





