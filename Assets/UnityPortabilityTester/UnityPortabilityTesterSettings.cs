using UnityEngine;

namespace UnityPortabilityTester
{
    [CreateAssetMenu(fileName = "UnityPortabilityTesterSettings", menuName = "Unity Portability Tester/Create Settings", order = 0)]
    public class UnityPortabilityTesterSettings : ScriptableObject
    {
        [Header("General Settings")] 
        public string[] externalResourcesPaths;
        [Header("Text Components Settings")]
        public bool checkText = true;
    }
}