using System;
using UnityEngine;

namespace UnityPortabilityTester
{
    [CreateAssetMenu(fileName = "PrefabsToTestPortability", menuName = "Unity Portability Tester/Create PrefabsToTestPortability", order = 1)]
    public class PrefabsToTestPortabilityData : ScriptableObject
    {
        public PrefabToTest[] PrefabToTests = new PrefabToTest[0];
    }
    
    [Serializable]
    public class PrefabToTest
    {
        public GameObject Prefab;
        public string ConstraintPath;
    }
}