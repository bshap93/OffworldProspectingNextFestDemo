using Domains.Scripts_that_Need_Sorting;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static int CurrentTextureIndex;
    private TerrainLayerDetector _terrainLayerDetector;

    private void Awake()
    {
        _terrainLayerDetector = FindFirstObjectByType<TerrainLayerDetector>();
        CurrentTextureIndex = -1;
    }
}