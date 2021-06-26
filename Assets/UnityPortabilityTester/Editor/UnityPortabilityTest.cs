using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
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
        private const string PrefabsToTestFileName = "PrefabsToTestPortabilityData.asset";
        private const string SettingsFileName = "Settings.asset";

        private UnityPortabilityTesterSettings _settings;
        private PrefabsToTestPortabilityData _prefabsToTest;

        [SetUp]
        public void SetUp()
        {
            _settings = AssetDatabase.LoadAssetAtPath<UnityPortabilityTesterSettings>(SettingsFilePath);
            if (_settings == null)
                _settings = CreateScriptableObjectOf<UnityPortabilityTesterSettings>(SettingsFilePath);
            
            _prefabsToTest = AssetDatabase.LoadAssetAtPath<PrefabsToTestPortabilityData>(PrefabsToTestFilePath);
            if (_prefabsToTest == null)
                _prefabsToTest = CreateScriptableObjectOf<PrefabsToTestPortabilityData>(PrefabsToTestFilePath);
        }

        [Test]
        public void CheckPortabilityOnPrefabs()
        {
            for (var i = _prefabsToTest.prefabToTests.Length - 1; i >= 0; i--)
            {
                CheckPortabilityFor(_prefabsToTest.prefabToTests[i]);
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

        private void CheckPortabilityFor(PrefabToTest prefabToTest)
        {
            if (_settings.checkText) CheckTextComponentFor(prefabToTest);
        }

        private void CheckTextComponentFor(PrefabToTest prefabToTest)
        {
            var texts = prefabToTest.prefab.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                CheckPathFor<Text>(text.font, prefabToTest);
                if(text.material != null)
                    CheckPathFor<Material>(text.material, prefabToTest);
            }
        }

        private void CheckPathFor<T>(Object asset, PrefabToTest prefabToTest)
        {
            if(!IsPathValid(AssetDatabase.GetAssetPath(asset), prefabToTest.constraintPath))
                Assert.Fail("The component "+typeof(T)+" inside prefab "+prefabToTest.prefab.name+" that is located at "+AssetDatabase.GetAssetPath(prefabToTest.prefab)+" has an external dependency." +
                            Environment.NewLine+"The dependency is "+asset.name+" and it should be located inside "+prefabToTest.constraintPath+" or at a external resource path."+
                            Environment.NewLine+"The dependency is located at "+AssetDatabase.GetAssetPath(asset)+" for now");
        }

        private bool IsPathValid(string path, string constraintPath) 
            => path.Contains(constraintPath) || _settings.externalResourcesPaths.Any(path.Contains);

        private static string FolderPath => AssetsPath+FolderName+"/";
        private static string SettingsFilePath => FolderPath+SettingsFileName;
        private static string PrefabsToTestFilePath => FolderPath+PrefabsToTestFileName;
    }
}
