using UnityEngine;

namespace UnityPortabilityTester
{
    [CreateAssetMenu(fileName = "FoldersOfPrefabsToTestPortability", menuName = "Unity Portability Tester/Create FoldersOfPrefabsToTestPortability", order = 2)]
    public class FoldersOfPrefabsToTestPortabilityData : ScriptableObject
    {
        public string[] paths;
    }
}