using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityPortabilityTester.Editor
{
    public class PortabilityFixer : EditorWindow
    {
        private static UnityPortabilityTesterSettings _settings;
        private const string SettingsFileName = "Settings.asset";
        private const string AssetsPath = "Assets/";
        private const string FolderName = "UnityPortabilityTesterData";
        
        private GameObject _prefabToTest;

        [MenuItem("Window/Portability Fixer")]
        public static void OnClickOpen()
        {
            GetWindow<PortabilityFixer>().Show();
            _settings = AssetDatabase.LoadAssetAtPath<UnityPortabilityTesterSettings>(SettingsFilePath);
            if (_settings == null)
                _settings = CreateScriptableObjectOf<UnityPortabilityTesterSettings>(SettingsFilePath);
        }
        
        private static T CreateScriptableObjectOf<T>(string path) where T : ScriptableObject
        {
            var scriptableObject = ScriptableObject.CreateInstance<T>();
            if(!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
            AssetDatabase.CreateAsset(scriptableObject, path);
            AssetDatabase.SaveAssets();
            return scriptableObject;
        }

        private void OnGUI()
        {
            _prefabToTest =
                (GameObject) EditorGUILayout.ObjectField("Prefab to test", _prefabToTest, typeof(GameObject));

            if (GUILayout.Button("Find Issues"))
                UpdateIssues();
            
            //TODO: Show current issues
        }

        private void UpdateIssues()
        {
            //TODO: Update issues
        }
        
        private static string FolderPath => AssetsPath+FolderName+"/";
        private static string SettingsFilePath => FolderPath+SettingsFileName;
    }
}
