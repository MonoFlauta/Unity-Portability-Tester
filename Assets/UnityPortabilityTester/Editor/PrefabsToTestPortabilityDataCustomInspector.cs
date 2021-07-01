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
            var total = EditorGUILayout.IntField("Prefab Amount", _data.prefabToTests.Length);

            if (total > _data.prefabToTests.Length)
                AddElementsATotalOf(total - _data.prefabToTests.Length);
            if (total < _data.prefabToTests.Length)
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
            for (var i = 0; i < _data.prefabToTests.Length; i++)
            {
                EditorGUILayout.LabelField("Prefab Number "+i);
                _data.prefabToTests[i].prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", _data.prefabToTests[i].prefab, typeof(GameObject), false);

                if (_data.prefabToTests[i].constraintPath == "" && _data.prefabToTests[i].prefab != null)
                    _data.prefabToTests[i].constraintPath = GetStartConstraintPathFrom(_data.prefabToTests[i].prefab);
                _data.prefabToTests[i].constraintPath = EditorGUILayout.TextField("Constraint Path", _data.prefabToTests[i].constraintPath);
            }
        }

        private string GetStartConstraintPathFrom(GameObject prefab)
        {
            var paths = AssetDatabase.GetAssetPath(prefab).Split('/');
            return paths.Take(paths.Length-1).Aggregate("", (result, stepPath) => result+stepPath+"/");
        }

        private void AddElementsATotalOf(int totalToAdd)
        {
            _data.prefabToTests = _data.prefabToTests.Concat(new PrefabToTest[totalToAdd]).ToArray();
            for (var i = _data.prefabToTests.Length - 1; i >= 0; i--)
            {
                if(_data.prefabToTests[i] == null)
                    _data.prefabToTests[i] = new PrefabToTest(null, "");
            }
        }

        private void KeepItemsTill(int total) 
            => _data.prefabToTests = _data.prefabToTests.Take(total).ToArray();
    }
}
