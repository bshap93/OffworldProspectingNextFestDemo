using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Scene.Terrain.Scripts
{
    [Serializable]
    public class TerrainLayerChoices
    {
        public int terrainLayerIndex;
        public int terrainToUseInstead; // or whatever type you need
    }

    [Serializable]
    public class TerrainDigParticlePrefab
    {
        public int terrainLayerIndex;
        [FormerlySerializedAs("prefab")] public GameObject primaryPrefab;
        [CanBeNull] public GameObject secondaryPrefab;
    }

    [Serializable]
    public class DefaultLayerAboveDepth
    {
        public float playerDepth;
        public int defaultLayerIndex;
        public int[] alternateAcceptableLayerIndices;
    }

    [CreateAssetMenu(fileName = "TerrainBehavior", menuName = "Scriptable Objects/TerrainBehavior")]
    public class TerrainBehavior : ScriptableObject
    {
        [Header("Terrain Layer Choices")] [FormerlySerializedAs("statEntries")]
        public List<TerrainLayerChoices> terrainChoices;

        public List<DefaultLayerAboveDepth> defaultLayerAboveDepths;

        [Header("Dig Particle Prefabs")] public List<TerrainDigParticlePrefab> terrainDigParticlePrefabs;
        public GameObject defaultDigParticlePrefab;
    }
}