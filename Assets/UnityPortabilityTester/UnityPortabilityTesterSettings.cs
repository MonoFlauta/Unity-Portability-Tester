using System;
using UnityEngine;

namespace UnityPortabilityTester
{
    [Serializable]
    [CreateAssetMenu(fileName = "UnityPortabilityTesterSettings", menuName = "Unity Portability Tester/Create Settings", order = 0)]
    public class UnityPortabilityTesterSettings : ScriptableObject
    {
        [Header("General Settings")] 
        public string[] externalResourcesPaths;
        [Header("UI Settings")]
        public bool checkText = true;
        public bool checkTextMeshPro = true;
        public bool checkImages = true;
        public bool checkRawImages = true;
        public bool checkButtons = true;
        public bool checkToggles = true;
        public bool checkSpriteSwap = true;
        public bool checkDropdowns = true;
        public bool checkInputFields = true;
        [Header("Prefab Settings")]
        public bool checkPrefabs = true;
        [Header("2D")]
        public bool checkSpriteRenderers = true;
        public bool checkSpriteMasks = true;
        public bool checkCollider2D = true;
        [Header("3D")]
        public bool checkMeshFilter = true;
        public bool checkMeshRenderer = true;
        public bool checkColliders = true;
        [Header("Others")]
        public bool checkAnimators = true;
    }
}