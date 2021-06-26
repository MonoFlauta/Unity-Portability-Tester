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
            if (_settings.checkText) CheckTextComponentsFor(prefabToTest);
            if (_settings.checkTextMeshPro) CheckTextMeshProComponentsFor(prefabToTest);
            if (_settings.checkImages) CheckImageComponentsFor(prefabToTest);
            if (_settings.checkRawImages) CheckRawImageComponentsFor(prefabToTest);
            if (_settings.checkButtons) CheckButtonComponentsFor(prefabToTest);
            if (_settings.checkToggles) CheckToggleComponentsFor(prefabToTest);
            if (_settings.checkSpriteSwap) CheckScrollBarComponentsFor(prefabToTest);
            if (_settings.checkDropdowns) CheckDropdownComponentsFor(prefabToTest);
            if (_settings.checkInputFields) CheckInputFieldComponentsFor(prefabToTest);

            if (_settings.checkPrefabs) CheckPrefabsFor(prefabToTest);
        }

        private void CheckPrefabsFor(PrefabToTest prefabToTest)
        {
            var gameObjects = prefabToTest.prefab.GetComponentsInChildren<Transform>().Select(x => x.gameObject);
            foreach (var gameObject in gameObjects)
            {
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(gameObject)) continue;
                var path = AssetDatabase.GetAssetPath(gameObject);
                if (!string.IsNullOrEmpty(path) && (!path.Contains(prefabToTest.constraintPath) && !_settings.externalResourcesPaths.Any(x => path.Contains(x))))
                    Assert.Fail("The prefab "+prefabToTest.prefab.name+" at "+AssetDatabase.GetAssetPath(prefabToTest.prefab)+" has a prefab outside the location."+
                                Environment.NewLine+"The GameObject "+gameObject.name+" is located at "+path);
            }
        }

        private void CheckInputFieldComponentsFor(PrefabToTest prefabToTest)
        {
            var inputFields = prefabToTest.prefab.GetComponentsInChildren<InputField>();
            foreach (var inputField in inputFields)
            {
                if (inputField.transition == Selectable.Transition.SpriteSwap)
                {
                    CheckPathFor<InputField>(inputField.spriteState.highlightedSprite, prefabToTest);
                    CheckPathFor<InputField>(inputField.spriteState.pressedSprite, prefabToTest);
                    CheckPathFor<InputField>(inputField.spriteState.selectedSprite, prefabToTest);
                    CheckPathFor<InputField>(inputField.spriteState.disabledSprite, prefabToTest);
                }
            }
        }

        private void CheckDropdownComponentsFor(PrefabToTest prefabToTest)
        {
            var dropdowns = prefabToTest.prefab.GetComponentsInChildren<Dropdown>();
            foreach (var dropdown in dropdowns)
            {
                if (dropdown.transition == Selectable.Transition.SpriteSwap)
                {
                    CheckPathFor<Dropdown>(dropdown.spriteState.highlightedSprite, prefabToTest);
                    CheckPathFor<Dropdown>(dropdown.spriteState.pressedSprite, prefabToTest);
                    CheckPathFor<Dropdown>(dropdown.spriteState.selectedSprite, prefabToTest);
                    CheckPathFor<Dropdown>(dropdown.spriteState.disabledSprite, prefabToTest);
                }
            }
        }

        private void CheckScrollBarComponentsFor(PrefabToTest prefabToTest)
        {
            var scrollBars = prefabToTest.prefab.GetComponentsInChildren<Scrollbar>();
            foreach (var scrollbar in scrollBars)
            {
                if (scrollbar.transition == Selectable.Transition.SpriteSwap)
                {
                    CheckPathFor<Scrollbar>(scrollbar.spriteState.highlightedSprite, prefabToTest);
                    CheckPathFor<Scrollbar>(scrollbar.spriteState.pressedSprite, prefabToTest);
                    CheckPathFor<Scrollbar>(scrollbar.spriteState.selectedSprite, prefabToTest);
                    CheckPathFor<Scrollbar>(scrollbar.spriteState.disabledSprite, prefabToTest);
                }
            }
        }

        private void CheckToggleComponentsFor(PrefabToTest prefabToTest)
        {
            var toggles = prefabToTest.prefab.GetComponentsInChildren<Toggle>();
            foreach (var toggle in toggles)
            {
                if (toggle.transition == Selectable.Transition.SpriteSwap)
                {
                    CheckPathFor<Toggle>(toggle.spriteState.highlightedSprite, prefabToTest);
                    CheckPathFor<Toggle>(toggle.spriteState.pressedSprite, prefabToTest);
                    CheckPathFor<Toggle>(toggle.spriteState.selectedSprite, prefabToTest);
                    CheckPathFor<Toggle>(toggle.spriteState.disabledSprite, prefabToTest);
                }
            }
        }

        private void CheckButtonComponentsFor(PrefabToTest prefabToTest)
        {
            var buttons = prefabToTest.prefab.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if (button.transition == Selectable.Transition.SpriteSwap)
                {
                    CheckPathFor<Button>(button.spriteState.highlightedSprite, prefabToTest);
                    CheckPathFor<Button>(button.spriteState.pressedSprite, prefabToTest);
                    CheckPathFor<Button>(button.spriteState.selectedSprite, prefabToTest);
                    CheckPathFor<Button>(button.spriteState.disabledSprite, prefabToTest);
                }
            }
        }

        private void CheckRawImageComponentsFor(PrefabToTest prefabToTest)
        {
            var images = prefabToTest.prefab.GetComponentsInChildren<RawImage>();
            foreach (var image in images)
            {
                CheckPathFor<RawImage>(image.texture, prefabToTest);
            }
        }

        private void CheckImageComponentsFor(PrefabToTest prefabToTest)
        {
            var images = prefabToTest.prefab.GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                CheckPathFor<Image>(image.sprite, prefabToTest);
            }
        }

        private void CheckTextMeshProComponentsFor(PrefabToTest prefabToTest)
        {
            var texts = prefabToTest.prefab.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                CheckPathFor<TextMeshProUGUI>(text.font, prefabToTest);
            }
        }

        private void CheckTextComponentsFor(PrefabToTest prefabToTest)
        {
            var texts = prefabToTest.prefab.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                CheckPathFor<Text>(text.font, prefabToTest);
            }
        }

        private void CheckPathFor<T>(Object asset, PrefabToTest prefabToTest)
        {
            if(asset != null 
               && (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset).Trim()) || !IsPathValid(AssetDatabase.GetAssetPath(asset), prefabToTest.constraintPath)))
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
