using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UniRx.Async;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace ImportFileFromGitHub
{


    public class Mkdir : MonoBehaviour
    {
        [Inject] private Import _import;

        async void Start()
        {
            _import.ImportFile
                .SkipLatestValueOnSubscribe()
                .Do(_ => print("do"))
                .Subscribe(i =>
                {
                    var dirPath = Path.Combine(Application.dataPath, @"Resources\PlayGround\");
                    var filePath = "";
                    //print(i.Src);
                    if (i.Ext == ImportedFile.Extension.cs)
                    {
                        dirPath = Path.Combine(dirPath, @"Scripts\");
                        filePath = Path.Combine(dirPath, i.Name);
                    }
                    else if (i.Ext == ImportedFile.Extension.shader)
                    {
                        dirPath = Path.Combine(dirPath, @"Shader\");
                        filePath = Path.Combine(dirPath, i.Name);
                    }
                    else
                    {
                        dirPath = Path.Combine(dirPath, @"Text\");
                        filePath = Path.Combine(dirPath, i.Name);

                    }

                    try
                    {
                        Directory.CreateDirectory(dirPath);
                        using (var writer = new StreamWriter(filePath, false))
                        {
                            writer.Write(i.Src);
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
                });

            //print(filePath);
//
//        try
//        {
//            sw = File.CreateText(filePath);
//        }
//        catch (Exception e)
//        {
//            Debug.LogException(e);
//        }
//        finally
//        {
//            if (sw != null)
//            {
//                try
//                {
//                    sw.Dispose();
//                }
//                catch(Exception e)
//                {
//                    Debug.LogException(e);
//                }
//                
//            }
//        }
        }


    }
}
