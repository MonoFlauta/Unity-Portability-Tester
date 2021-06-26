﻿using UnityEngine;

namespace UnityPortabilityTester
{
    [CreateAssetMenu(fileName = "UnityPortabilityTesterSettings", menuName = "Unity Portability Tester/Create Settings", order = 0)]
    public class UnityPortabilityTesterSettings : ScriptableObject
    {
        [Header("General Settings")] 
        public string[] externalResourcesPaths;
        [Header("Text Components Settings")]
        public bool checkText = true;
        public bool checkTextMeshPro = true;
        public bool checkImages = true;
        public bool checkRawImages = true;
        public bool checkButtons = true;
        public bool checkToggles = true;
        public bool checkSpriteSwap = true;
        public bool checkDropdowns = true;
        public bool checkInputFields = true;
    }
}