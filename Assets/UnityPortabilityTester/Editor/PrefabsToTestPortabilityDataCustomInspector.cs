using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityPortabilityTester.Editor
{
    [CustomEditor(typeof(PrefabsToTestPortabilityData))]
    public class PrefabsToTestPortabilityDataCustomInspector : UnityEditor.Editor
    {
        private PrefabsToTestPortabilityData _data;
        
        private void OnEnable()
        {
            _data = (PrefabsToTestPortabilityData)target;
        }

        public override void OnInspectorGUI()
        {
            var total = EditorGUILayout.IntField("Prefab Amount", _data.PrefabToTests.Length);

            if (total > _data.PrefabToTests.Length)
                AddElementsATotalOf(total - _data.PrefabToTests.Length);
            if (total < _data.PrefabToTests.Length)
                KeepItemsTill(total);

            ShowOptions();

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void ShowOptions()
        {
            for (var i = 0; i < _data.PrefabToTests.Length; i++)
            {
                EditorGUILayout.LabelField("Prefab Number "+i);
                _data.PrefabToTests[i].Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", _data.PrefabToTests[i].Prefab, typeof(GameObject), false);

                if (_data.PrefabToTests[i].ConstraintPath == "" && _data.PrefabToTests[i].Prefab != null)
                    _data.PrefabToTests[i].ConstraintPath = GetStartConstraintPathFrom(_data.PrefabToTests[i].Prefab);
                _data.PrefabToTests[i].ConstraintPath = EditorGUILayout.TextField("Constraint Path", _data.PrefabToTests[i].ConstraintPath);
            }
        }

        private string GetStartConstraintPathFrom(GameObject prefab)
        {
            var paths = AssetDatabase.GetAssetPath(prefab).Split('/');
            return paths.Take(paths.Length-1).Aggregate("", (result, stepPath) => result+stepPath+"/");
        }

        private void AddElementsATotalOf(int totalToAdd)
        {
            _data.PrefabToTests = _data.PrefabToTests.Concat(new PrefabToTest[totalToAdd]).ToArray();
            for (var i = _data.PrefabToTests.Length - 1; i >= 0; i--)
            {
                if(_data.PrefabToTests[i] == null)
                    _data.PrefabToTests[i] = new PrefabToTest();
            }
        }

        private void KeepItemsTill(int total) 
            => _data.PrefabToTests = _data.PrefabToTests.Take(total).ToArray();
    }
}
