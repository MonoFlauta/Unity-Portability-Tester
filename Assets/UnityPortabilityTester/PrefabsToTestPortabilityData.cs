using UnityEngine;

namespace UnityPortabilityTester
{
    [CreateAssetMenu(fileName = "PrefabsToTestPortability", menuName = "Unity Portability Tester/Create PrefabsToTestPortability", order = 1)]
    public class PrefabsToTestPortabilityData : ScriptableObject
    {
        public PrefabToTest[] prefabToTests = new PrefabToTest[0];
    }
}