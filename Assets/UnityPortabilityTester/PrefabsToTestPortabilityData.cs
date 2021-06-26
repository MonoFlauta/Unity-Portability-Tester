﻿using System;
using UnityEngine;

namespace UnityPortabilityTester
{
    [CreateAssetMenu(fileName = "PrefabsToTestPortability", menuName = "Unity Portability Tester/Create PrefabsToTestPortability", order = 1)]
    public class PrefabsToTestPortabilityData : ScriptableObject
    {
        [SerializeField]
        public PrefabToTest[] prefabToTests = new PrefabToTest[0];
    }

    [Serializable]
    public struct PrefabToTest
    {
        public GameObject prefab;
        public string constraintPath;
    }
}