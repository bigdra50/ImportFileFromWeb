using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// テキストファイルが取得できる形式へ変換する
/// </summary>
public class URLFormatter
{
    // github
    // in: https://github.com/bigdra50/Shaders/blob/master/ShaderLab/ATField.shader
    // out: https://raw.githubusercontent.com/bigdra50/Shaders/master/ShaderLab/ATField.shader
    private string _githubHost = @"github.com";
    private string _rawGitHubHost = @"raw.githubusercontent.com";

    public Uri Format(Uri uri)
    {
        
        Debug.Log($"Start Format : {uri}");
        #region GitHub
        // github.comのリンクからrawファイルのリンクへ変換
        if (uri.Host == _rawGitHubHost)
        {
            return uri;
        }else if (uri.Host == _githubHost)
        {
            var uriPart = uri.AbsolutePath.ToString().Split('/');    // user/repo/blob/hoge/hoge.fuga
            var uriStr = $"{uri.Scheme}://{_rawGitHubHost}{uriPart[0]}/{uriPart[1]}/";
            var builder = new StringBuilder(uriStr);
            for (var i = 2; i < uriPart.Length; i++)
            {
                if (uriPart[i] == "blob")
                {
                    continue;
                }
                builder.Append(uriPart[i] + "/");
            }
            uri = new Uri(builder.ToString().TrimEnd('/'));
        }
        #endregion
        
       // Debug.Log(uri);

       Debug.Log($"Done Format: {uri}");
        return uri;
    }
}
