using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityPortabilityTester.Editor
{
    public class UnityPortabilityTest
    {
        private const string AssetsPath = "Assets/";
        private const string FolderName = "UnityPortabilityTesterData";
        private const string SettingsFileName = "Settings.asset";

        private UnityPortabilityTesterSettings _settings;
        private PortabilityChecker _portabilityChecker;

        [SetUp]
        public void SetUp()
        {
            _settings = AssetDatabase.LoadAssetAtPath<UnityPortabilityTesterSettings>(SettingsFilePath);
            if (_settings == null)
                _settings = CreateScriptableObjectOf<UnityPortabilityTesterSettings>(SettingsFilePath);

            _portabilityChecker = new PortabilityChecker(_settings, AssertPrefabFailFor, AssertComponentFailFor);
        }

        [Test]
        public void CheckPortabilityOnPrefabs()
        {
            foreach (var prefabsToTest in FindAssetsByType<PrefabsToTestPortabilityData>())
                for (var i = prefabsToTest.prefabToTests.Length - 1; i >= 0; i--)
                    _portabilityChecker.CheckPortabilityFor(prefabsToTest.prefabToTests[i]);
        }

        [Test]
        public void CheckPortabilityOnPrefabsByFolder()
        {
            foreach (var folderPathData in FindAssetsByType<FoldersOfPrefabsToTestPortabilityData>())
            {
                 var prefabToTests = AssetDatabase.GetAllAssetPaths()
                    .Where(assetPath => folderPathData.paths.Any(assetPath.Contains))
                    .Select(path => new Tuple<Object, string>(AssetDatabase.LoadAssetAtPath<Object>(path), path))
                    .Where(t => t.Item1 is GameObject)
                    .Select(t => new PrefabToTest((GameObject)t.Item1, t.Item2)).ToList();

                 foreach (var prefabToTest in prefabToTests) 
                     _portabilityChecker.CheckPortabilityFor(prefabToTest);
            }
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

        private static void AssertPrefabFailFor(PrefabToTest prefabToTest, GameObject gameObject, string path) =>
            Assert.Fail("The prefab " + prefabToTest.prefab.name + " at " + AssetDatabase.GetAssetPath(prefabToTest.prefab) +
                        " has a prefab outside the location." +
                        Environment.NewLine + "The GameObject " + gameObject.name + " is located at " + path);

        private static void AssertComponentFailFor(Object asset, PrefabToTest prefabToTest, Type type) =>
            Assert.Fail("The component "+type+" inside prefab "+prefabToTest.prefab.name+" that is located at "+AssetDatabase.GetAssetPath(prefabToTest.prefab)+" has an external dependency." +
                        Environment.NewLine+"The dependency is "+asset.name+" and it should be located inside "+prefabToTest.constraintPath+" or at a external resource path."+
                        Environment.NewLine+"The dependency is located at "+AssetDatabase.GetAssetPath(asset)+" for now");

        private T[] FindAssetsByType<T>() where T : Object =>
            AssetDatabase.FindAssets($"t:{typeof(T)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .Where(asset => asset != null)
                .ToArray();

        private static string FolderPath => AssetsPath+FolderName+"/";
        private static string SettingsFilePath => FolderPath+SettingsFileName;
    }
}
