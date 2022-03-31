using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityPortabilityTester.Editor
{
    public class PortabilityChecker
    {
        private readonly UnityPortabilityTesterSettings _settings;
        private readonly Action<PrefabToTest, GameObject, string> _onPrefabFail;
        private readonly Action<Object, PrefabToTest, Type> _onComponentFail;

        public PortabilityChecker(
            UnityPortabilityTesterSettings settings, 
            Action<PrefabToTest, GameObject, string> onPrefabFail, 
            Action<Object, PrefabToTest, Type> onComponentFail
            )
        {
            _settings = settings;
            _onPrefabFail = onPrefabFail;
            _onComponentFail = onComponentFail;
        }

        public void CheckPortabilityFor(PrefabToTest prefabToTest)
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
                CheckPathFor(textMesh.mesh, prefabToTest, typeof(TextMeshPro));
                CheckPathFor(textMesh.font, prefabToTest, typeof(TextMeshPro));
            }
        }

        private void CheckColliders2DComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var collider in prefabToTest.prefab.GetComponentsInChildren<Collider2D>())
            {
                CheckPathFor(collider.sharedMaterial, prefabToTest, typeof(Collider2D));
            }
        }

        private void CheckCollidersComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var collider in prefabToTest.prefab.GetComponentsInChildren<Collider>())
            {
                CheckPathFor(collider.material, prefabToTest, typeof(Collider));
                CheckPathFor(collider.sharedMaterial, prefabToTest, typeof(Collider));
            }
        }

        private void CheckMeshRendererComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var meshRenderer in prefabToTest.prefab.GetComponentsInChildren<MeshRenderer>())
            {
                CheckPathFor(meshRenderer.material, prefabToTest, typeof(MeshRenderer));
                CheckPathFor(meshRenderer.sharedMaterial, prefabToTest, typeof(MeshRenderer));
                foreach (var material in meshRenderer.materials)
                    CheckPathFor(material, prefabToTest, typeof(MeshRenderer));

                foreach (var sharedMaterial in meshRenderer.sharedMaterials)
                    CheckPathFor(sharedMaterial, prefabToTest, typeof(MeshRenderer));
            }
        }

        private void CheckMeshFilterComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var meshFilter in prefabToTest.prefab.GetComponentsInChildren<MeshFilter>())
            {
                CheckPathFor(meshFilter.mesh, prefabToTest, typeof(MeshFilter));
                CheckPathFor(meshFilter.sharedMesh, prefabToTest, typeof(MeshFilter));
            }
        }

        private void CheckSpriteMaskComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var spriteMask in prefabToTest.prefab.GetComponentsInChildren<SpriteMask>())
                CheckPathFor(spriteMask.sprite, prefabToTest, typeof(SpriteMask));
        }

        private void CheckSpriteRendererComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var spriteRenderer in prefabToTest.prefab.GetComponentsInChildren<SpriteRenderer>())
            {
                CheckPathFor(spriteRenderer.sprite, prefabToTest, typeof(SpriteRenderer));
                CheckPathFor(spriteRenderer.sharedMaterial, prefabToTest, typeof(SpriteRenderer));
                foreach (var sharedMaterial in spriteRenderer.sharedMaterials)
                    CheckPathFor(sharedMaterial, prefabToTest, typeof(SpriteRenderer));
            }
        }

        private void CheckAnimatorComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var animator in prefabToTest.prefab.GetComponentsInChildren<Animator>())
            {
                if (animator.runtimeAnimatorController != null)
                {
                    CheckPathFor(animator.runtimeAnimatorController, prefabToTest, typeof(Animator));
                    CheckAnimatorController(animator.runtimeAnimatorController, prefabToTest);
                }
                CheckPathFor(animator.avatar, prefabToTest, typeof(Animator));
            }
        }

        private void CheckAnimatorController(RuntimeAnimatorController controller, PrefabToTest prefabToTest)
        {
            foreach (var clip in controller.animationClips)
                CheckPathFor(clip, prefabToTest, typeof(AnimationClip));
        }

        private void CheckInputFieldComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var inputField in prefabToTest.prefab.GetComponentsInChildren<InputField>())
            {
                if (inputField.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor(inputField.spriteState.highlightedSprite, prefabToTest, typeof(InputField));
                CheckPathFor(inputField.spriteState.pressedSprite, prefabToTest, typeof(InputField));
                CheckPathFor(inputField.spriteState.selectedSprite, prefabToTest, typeof(InputField));
                CheckPathFor(inputField.spriteState.disabledSprite, prefabToTest, typeof(InputField));
            }
        }

        private void CheckDropdownComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var dropdown in prefabToTest.prefab.GetComponentsInChildren<Dropdown>())
            {
                if (dropdown.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor(dropdown.spriteState.highlightedSprite, prefabToTest, typeof(Dropdown));
                CheckPathFor(dropdown.spriteState.pressedSprite, prefabToTest, typeof(Dropdown));
                CheckPathFor(dropdown.spriteState.selectedSprite, prefabToTest, typeof(Dropdown));
                CheckPathFor(dropdown.spriteState.disabledSprite, prefabToTest, typeof(Dropdown));
            }
        }

        private void CheckScrollBarComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var scrollbar in prefabToTest.prefab.GetComponentsInChildren<Scrollbar>())
            {
                if (scrollbar.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor(scrollbar.spriteState.highlightedSprite, prefabToTest, typeof(Scrollbar));
                CheckPathFor(scrollbar.spriteState.pressedSprite, prefabToTest, typeof(Scrollbar));
                CheckPathFor(scrollbar.spriteState.selectedSprite, prefabToTest, typeof(Scrollbar));
                CheckPathFor(scrollbar.spriteState.disabledSprite, prefabToTest, typeof(Scrollbar));
            }
        }

        private void CheckToggleComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var toggle in prefabToTest.prefab.GetComponentsInChildren<Toggle>())
            {
                if (toggle.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor(toggle.spriteState.highlightedSprite, prefabToTest, typeof(Toggle));
                CheckPathFor(toggle.spriteState.pressedSprite, prefabToTest, typeof(Toggle));
                CheckPathFor(toggle.spriteState.selectedSprite, prefabToTest, typeof(Toggle));
                CheckPathFor(toggle.spriteState.disabledSprite, prefabToTest, typeof(Toggle));
            }
        }

        private void CheckButtonComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var button in prefabToTest.prefab.GetComponentsInChildren<Button>())
            {
                if (button.transition != Selectable.Transition.SpriteSwap) continue;
                CheckPathFor(button.spriteState.highlightedSprite, prefabToTest, typeof(Button));
                CheckPathFor(button.spriteState.pressedSprite, prefabToTest, typeof(Button));
                CheckPathFor(button.spriteState.selectedSprite, prefabToTest, typeof(Button));
                CheckPathFor(button.spriteState.disabledSprite, prefabToTest, typeof(Button));
            }
        }

        private void CheckRawImageComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var image in prefabToTest.prefab.GetComponentsInChildren<RawImage>())
                CheckPathFor(image.texture, prefabToTest, typeof(RawImage));
        }

        private void CheckImageComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var image in prefabToTest.prefab.GetComponentsInChildren<Image>())
                CheckPathFor(image.sprite, prefabToTest, typeof(Image));
        }

        private void CheckTextMeshProUIComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var text in prefabToTest.prefab.GetComponentsInChildren<TextMeshProUGUI>())
                CheckPathFor(text.font, prefabToTest, typeof(TextMeshProUGUI));
        }

        private void CheckTextComponentsFor(PrefabToTest prefabToTest)
        {
            foreach (var text in prefabToTest.prefab.GetComponentsInChildren<Text>())
                CheckPathFor(text.font, prefabToTest, typeof(Text));
        }

        private void CheckPrefabsFor(PrefabToTest prefabToTest)
        {
            foreach (var gameObject in prefabToTest.prefab.GetComponentsInChildren<Transform>().Select(x => x.gameObject))
            {
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(gameObject)) continue;
                var path = AssetDatabase.GetAssetPath(gameObject);
                if (!string.IsNullOrEmpty(path) && !path.Contains(prefabToTest.constraintPath) && !_settings.externalResourcesPaths.Any(x => path.Contains(x))) 
                    _onPrefabFail(prefabToTest, gameObject, path);
            }
        }

        private void CheckPathFor(Object asset, PrefabToTest prefabToTest, Type type)
        {
            if(asset != null 
               && (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset).Trim()) || !IsPathValid(AssetDatabase.GetAssetPath(asset), prefabToTest.constraintPath)))
                _onComponentFail(asset, prefabToTest, type);
        }
        
        private bool IsPathValid(string path, string constraintPath) 
            => path.Contains(constraintPath) || _settings.externalResourcesPaths.Any(path.Contains);
    }
}
