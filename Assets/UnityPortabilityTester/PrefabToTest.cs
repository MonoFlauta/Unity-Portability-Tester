using System;
using UnityEngine;

namespace UnityPortabilityTester
{
    [Serializable]
    public class PrefabToTest
    {
        public GameObject prefab;
        public string constraintPath;

        public PrefabToTest(GameObject prefab, string constraintPath)
        {
            this.prefab = prefab;
            this.constraintPath = constraintPath;
        }
    }
}