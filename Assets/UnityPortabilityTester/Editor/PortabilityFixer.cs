using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityPortabilityTester.Editor
{
    public class PortabilityFixer : EditorWindow
    {
        private static UnityPortabilityTesterSettings _settings;
        private const string SettingsFileName = "Settings.asset";
        private const string AssetsPath = "Assets/";
        private const string FolderName = "UnityPortabilityTesterData";
        
        private GameObject _prefabToTest;
        private PortabilityChecker _portabilityChecker;
        private string _constraintPath;
        private List<Object> _outsideObjects;
        private string _targetPath;

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
            var scriptableObject = CreateInstance<T>();
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

            if (_prefabToTest == null) return;

            _constraintPath = EditorGUILayout.TextField("Constraint Path", _constraintPath);
            
            _targetPath = EditorGUILayout.TextField("Target Path", _targetPath);
            FixConstraintPath();

            if (string.IsNullOrEmpty(_constraintPath) || _constraintPath.Last() != '/' || !_constraintPath.StartsWith("Assets/"))
            {
                EditorGUILayout.HelpBox("Insert a valid constraint path. It must finish with / and start with Assets/", MessageType.Error);
                return;
            }
            
            if (string.IsNullOrEmpty(_targetPath) || _targetPath.Last() != '/' || !_targetPath.StartsWith(_constraintPath))
            {
                EditorGUILayout.HelpBox("Insert a valid target path. It must finish with / and start with the constraint path", MessageType.Error);
                return;
            }

            if (GUILayout.Button("Update Issues"))
                UpdateIssues();

            if (_outsideObjects == null) _outsideObjects = new List<Object>();

            for (var i = _outsideObjects.Count - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();
                ShowOutsideObjectFor(_outsideObjects[i]);
                GUILayout.EndHorizontal();
            }
        }

        private void FixConstraintPath()
        {
            if (_targetPath == null) _targetPath = "";
            if (_constraintPath != null && !_targetPath.Contains(_constraintPath)) _targetPath = _constraintPath;
        }

        private void ShowOutsideObjectFor(Object outsideObject)
        {
            EditorGUILayout.LabelField(outsideObject.name);
            EditorGUILayout.LabelField(GetPathFor(outsideObject));
            if (GUILayout.Button("Move Asset Inside Folder"))
                MoveAssetInsideFolder(outsideObject);
        }

        private void MoveAssetInsideFolder(Object outsideObject)
        {
            var startPath = GetPathFor(outsideObject);
            var resultPath = _targetPath + startPath.Split('/').Last();
            AssetDatabase.MoveAsset(
                startPath,
                resultPath);
            UpdateIssues();
        }

        private string GetPathFor(Object outsideObject) => 
            AssetDatabase.GetAssetPath(outsideObject);

        private void UpdateIssues()
        {
            _outsideObjects = new List<Object>();
            _portabilityChecker = new PortabilityChecker(_settings, OnPrefabFail, OnComponentFail);
            _portabilityChecker.CheckPortabilityFor(new PrefabToTest(_prefabToTest, _constraintPath));
        }

        private void OnComponentFail(Object asset, PrefabToTest testedPrefab, Type assetType)
        {
            if(!_outsideObjects.Contains(asset))
                _outsideObjects.Add(asset);
        }

        private void OnPrefabFail(PrefabToTest testedPrefab, GameObject outsideGameObject, string pathOfGameObject)
        {
            if(!_outsideObjects.Contains(outsideGameObject))
                _outsideObjects.Add(outsideGameObject);
        }

        private static string FolderPath => AssetsPath+FolderName+"/";
        private static string SettingsFilePath => FolderPath+SettingsFileName;
    }
}
