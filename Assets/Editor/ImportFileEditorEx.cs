using ImportFileFromGitHub;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ImportFileEditorEx : EditorWindow
    {
        
        [MenuItem("Window/ImportFile")]
        static void Open()
        {
            EditorWindow.GetWindow<ImportFileEditorEx>("ImportFile");
        }


        private string _url = "";

        private void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("インポートしたいファイルのURLを入力して下さい.");
            GUILayout.Label("URL");
            _url = GUILayout.TextField(_url);
            
            GUILayout.Space(10);
            

            if (GUILayout.Button("Import"))
            {
                Debug.Log("Start Import");
                var import = new ImportFileFromWeb();
                import.StartImportAsync(_url);
                _url = "";
            }

            if (GUILayout.Button("Reset"))
            {
                _url = "";
            }

        }
    }
}
