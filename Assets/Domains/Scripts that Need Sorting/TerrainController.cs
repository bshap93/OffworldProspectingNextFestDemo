using Domains.Scene.Terrain.Scripts;
using JetBrains.Annotations;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class TerrainController : MonoBehaviour
    {
        [SerializeField] public TerrainBehavior terrainBehavior;
        private GameObject currentDebrisPrefab;

        private GameObject previousDebrisPrefab;

        public static TerrainController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }


        public GameObject GetTerrainPrefab(int textureIndex)
        {
            if (terrainBehavior == null)
            {
                UnityEngine.Debug.LogError("TerrainBehavior is not assigned.");
                return null;
            }

            foreach (var terrain in terrainBehavior.terrainDigParticlePrefabs)
                if (terrain.terrainLayerIndex == textureIndex)
                    return terrain.primaryPrefab;


            return terrainBehavior.defaultDigParticlePrefab;
        }

        [CanBeNull]
        public GameObject GetSecondaryTerrainPrefab(int textureName)
        {
            if (terrainBehavior == null)
            {
                UnityEngine.Debug.LogError("TerrainBehavior is not assigned.");
                return null;
            }

            foreach (var terrain in terrainBehavior.terrainDigParticlePrefabs)
                if (terrain.terrainLayerIndex == textureName)
                    return terrain.secondaryPrefab;


            return null;
        }
    }
}