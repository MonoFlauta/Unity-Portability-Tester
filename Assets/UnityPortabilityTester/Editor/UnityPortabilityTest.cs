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

        [SetUp]
        public void SetUp()
        {
            _settings = AssetDatabase.LoadAssetAtPath<UnityPortabilityTesterSettings>(SettingsFilePath);
            if (_settings == null)
                _settings = CreateScriptableObjectOf<UnityPortabilityTesterSettings>(SettingsFilePath);
        }

        [Test]
        public void CheckPortabilityOnPrefabs()
        {
            foreach (var prefabsToTest in FindAssetsByType<PrefabsToTestPortabilityData>())
                for (var i = prefabsToTest.prefabToTests.Length - 1; i >= 0; i--)
                    CheckPortabilityFor(prefabsToTest.prefabToTests[i]);
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
                     CheckPortabilityFor(prefabToTest);
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
            if (_settings.checkTextMeshProUI) CheckTextMeshProUIComponentsFor(prefabToTest);
            if (_settings.checkImages) CheckImageComponentsFor(prefabToTest);
            if (_settings.checkRawImages) CheckRawImageComponentsFor(prefabToTest);
            if (_settings.checkButtons) CheckButtonComponentsFor(prefabToTest);
            if (_settings.checkToggles) CheckToggleComponentsFor(prefabToTest);
            if (_settings.checkSpriteSwap) CheckScrollBarComponentsFor(prefabToTest);
            if (_settings.checkDropdowns) CheckDropdownComponentsFor(prefabToTest);
            if (_settings.checkInputFields) CheckInputFieldComponentsFor(prefabToTest);

            if (_settings.checkPrefabs) CheckPrefabsFor(prefabToTest);

            if (_settings.checkAnimators) CheckAnimatorComponentsFor(prefabToTest);

            if (_settings.checkSpriteRenderers) CheckSpriteRendererComponentsFor(prefabToTest);
            if (_settings.checkSpriteMasks) CheckSpriteMaskComponentsFor(prefabToTest);
            if (_settings.checkCollider2D) CheckColliders2DComponentsFor(prefabToTest);

            if (_settings.checkMeshFilter) CheckMeshFilterComponentsFor(prefabToTest);
            if (_settings.checkMeshRenderer) CheckMeshRendererComponentsFor(prefabToTest);
            if (_settings.checkColliders) CheckCollidersComponentsFor(prefabToTest);
            if (_settings.checkTextMeshPro) CheckTextMeshProComponentsFor(prefabToTest);
        }

        private void CheckTextMeshProComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var textMesh in prefabToTest.prefab.GetComponentsInChildren<TextMeshPro>())
            {
                CheckPathFor<TextMeshPro>(textMesh.mesh, prefabToTest);
                CheckPathFor<TextMeshPro>(textMesh.font, prefabToTest);
            }
        }

        private void CheckColliders2DComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var collider in prefabToTest.prefab.GetComponentsInChildren<Collider2D>())
            {
                CheckPathFor<Collider2D>(collider.sharedMaterial, prefabToTest);
            }
        }

        private void CheckCollidersComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var collider in prefabToTest.prefab.GetComponentsInChildren<Collider>())
            {
                CheckPathFor<Collider>(collider.material, prefabToTest);
                CheckPathFor<Collider>(collider.sharedMaterial, prefabToTest);
            }
        }

        private void CheckMeshRendererComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var meshRenderer in prefabToTest.prefab.GetComponentsInChildren<MeshRenderer>())
            {
                CheckPathFor<MeshRenderer>(meshRenderer.material, prefabToTest);
                CheckPathFor<MeshRenderer>(meshRenderer.sharedMaterial, prefabToTest);
                foreach (var material in meshRenderer.materials)
                    CheckPathFor<MeshRenderer>(material, prefabToTest);

                foreach (var sharedMaterial in meshRenderer.sharedMaterials)
                    CheckPathFor<MeshRenderer>(sharedMaterial, prefabToTest);
            }
        }

        private void CheckMeshFilterComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var meshFilter in prefabToTest.prefab.GetComponentsInChildren<MeshFilter>())
            {
                CheckPathFor<MeshFilter>(meshFilter.mesh, prefabToTest);
                CheckPathFor<MeshFilter>(meshFilter.sharedMesh, prefabToTest);
            }
        }

        private void CheckSpriteMaskComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var spriteMask in prefabToTest.prefab.GetComponentsInChildren<SpriteMask>())
                CheckPathFor<SpriteMask>(spriteMask.sprite, prefabToTest);
        }

        private void CheckSpriteRendererComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var spriteRenderer in prefabToTest.prefab.GetComponentsInChildren<SpriteRenderer>())
            {
                CheckPathFor<SpriteRenderer>(spriteRenderer.sprite, prefabToTest);
                CheckPathFor<SpriteRenderer>(spriteRenderer.sharedMaterial, prefabToTest);
                foreach (var sharedMaterial in spriteRenderer.sharedMaterials)
                    CheckPathFor<SpriteRenderer>(sharedMaterial, prefabToTest);
            }
        }

        private void CheckAnimatorComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var animator in prefabToTest.prefab.GetComponentsInChildren<Animator>())
            {
                CheckPathFor<Animator>(animator.runtimeAnimatorController, prefabToTest);
                CheckPathFor<Animator>(animator.avatar, prefabToTest);
                CheckAnimatorController(animator.runtimeAnimatorController, prefabToTest);
            }
        }

        private void CheckAnimatorController(RuntimeAnimatorController controller, PrefabToTest prefabToTest)
        {
            foreach (var clip in controller.animationClips)
                CheckPathFor<AnimationClip>(clip, prefabToTest);
        }

        private void CheckPrefabsFor(PrefabToTest prefabToTest)
        {
            foreach (var gameObject in prefabToTest.prefab.GetComponentsInChildren<Transform>().Select(x => x.gameObject))
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
            foreach (var inputField in prefabToTest.prefab.GetComponentsInChildren<InputField>())
            {
                if (inputField.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor<InputField>(inputField.spriteState.highlightedSprite, prefabToTest);
                CheckPathFor<InputField>(inputField.spriteState.pressedSprite, prefabToTest);
                CheckPathFor<InputField>(inputField.spriteState.selectedSprite, prefabToTest);
                CheckPathFor<InputField>(inputField.spriteState.disabledSprite, prefabToTest);
            }
        }

        private void CheckDropdownComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var dropdown in prefabToTest.prefab.GetComponentsInChildren<Dropdown>())
            {
                if (dropdown.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor<Dropdown>(dropdown.spriteState.highlightedSprite, prefabToTest);
                CheckPathFor<Dropdown>(dropdown.spriteState.pressedSprite, prefabToTest);
                CheckPathFor<Dropdown>(dropdown.spriteState.selectedSprite, prefabToTest);
                CheckPathFor<Dropdown>(dropdown.spriteState.disabledSprite, prefabToTest);
            }
        }

        private void CheckScrollBarComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var scrollbar in prefabToTest.prefab.GetComponentsInChildren<Scrollbar>())
            {
                if (scrollbar.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor<Scrollbar>(scrollbar.spriteState.highlightedSprite, prefabToTest);
                CheckPathFor<Scrollbar>(scrollbar.spriteState.pressedSprite, prefabToTest);
                CheckPathFor<Scrollbar>(scrollbar.spriteState.selectedSprite, prefabToTest);
                CheckPathFor<Scrollbar>(scrollbar.spriteState.disabledSprite, prefabToTest);
            }
        }

        private void CheckToggleComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var toggle in prefabToTest.prefab.GetComponentsInChildren<Toggle>())
            {
                if (toggle.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor<Toggle>(toggle.spriteState.highlightedSprite, prefabToTest);
                CheckPathFor<Toggle>(toggle.spriteState.pressedSprite, prefabToTest);
                CheckPathFor<Toggle>(toggle.spriteState.selectedSprite, prefabToTest);
                CheckPathFor<Toggle>(toggle.spriteState.disabledSprite, prefabToTest);
            }
        }

        private void CheckButtonComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var button in prefabToTest.prefab.GetComponentsInChildren<Button>())
            {
                if (button.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor<Button>(button.spriteState.highlightedSprite, prefabToTest);
                CheckPathFor<Button>(button.spriteState.pressedSprite, prefabToTest);
                CheckPathFor<Button>(button.spriteState.selectedSprite, prefabToTest);
                CheckPathFor<Button>(button.spriteState.disabledSprite, prefabToTest);
            }
        }

        private void CheckRawImageComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var image in prefabToTest.prefab.GetComponentsInChildren<RawImage>())
                CheckPathFor<RawImage>(image.texture, prefabToTest);
        }

        private void CheckImageComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var image in prefabToTest.prefab.GetComponentsInChildren<Image>())
                CheckPathFor<Image>(image.sprite, prefabToTest);
        }

        private void CheckTextMeshProUIComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var text in prefabToTest.prefab.GetComponentsInChildren<TextMeshProUGUI>())
                CheckPathFor<TextMeshProUGUI>(text.font, prefabToTest);
        }

        private void CheckTextComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var text in prefabToTest.prefab.GetComponentsInChildren<Text>())
                CheckPathFor<Text>(text.font, prefabToTest);
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
