using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

namespace ImportFileFromGitHub
{
    public class Import : MonoBehaviour
    {
        [SerializeField] private StringReactiveProperty _rawSourcePath = new StringReactiveProperty("");
        private readonly ReactiveProperty<ImportedFile> _importFile = new ReactiveProperty<ImportedFile>();
        public IReadOnlyReactiveProperty<ImportedFile> ImportFile => _importFile;

        private string _githubPath = "https://raw.githubusercontent.com";
        private string _userName = "bigdra50";
        private string _filePath = "Shaders/master/ShaderLab/ATField.shader/";

        async void Start()
        {
            _rawSourcePath
                .Do(_ => print(_rawSourcePath.Value))
                .Subscribe(async _ =>
                {
                    try
                    {
                        var srcPath = GetRawSourcePath();
                        if (string.IsNullOrEmpty(srcPath)) return;
                        var src = await GetRawSourceAsync(srcPath);
                        _importFile.Value = new ImportedFile(srcPath, src);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });

            //print($"name: {ImportFile.Value.Name} \nsrc: {ImportFile.Value.Src} \next: {ImportFile.Value.Ext}");
        }

        string GetRawSourcePath()
        {
            var uri = _rawSourcePath.Value;
//        if (string.IsNullOrEmpty(uri))
//        {
//            return uri;
//            var builder = new StringBuilder();
//            var paths = new List<string>()
//            {
//                _githubPath,
//                _userName,
//                _filePath,
//            };
//            paths.ForEach(x => builder.Append(x).Append("/"));
//            uri = builder.ToString();
//        }
//
            uri = uri.TrimEnd(' ', '/');
            return uri;
        }

        async UniTask<string> GetRawSourceAsync(string uri)
        {
            var req = UnityWebRequest.Get(uri);
            await req.SendWebRequest();
            if (req.isHttpError || req.isNetworkError)
            {
                throw new Exception(req.error);
            }

            return req.downloadHandler.text;
        }
    }

    public struct ImportedFile
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
}
