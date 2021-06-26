using System;
using UnityEngine;

namespace UnityPortabilityTester
{
    [Serializable]
    [CreateAssetMenu(fileName = "PrefabsToTestPortability", menuName = "Unity Portability Tester/Create PrefabsToTestPortability", order = 1)]
    public class PrefabsToTestPortabilityData : ScriptableObject
    {
        [SerializeField]
        public PrefabToTest[] prefabToTests = new PrefabToTest[0];
    }

    [Serializable]
    public struct PrefabToTest
    {
        [SerializeField]
        public GameObject prefab;
        [SerializeField]
        public string constraintPath;
    }
}