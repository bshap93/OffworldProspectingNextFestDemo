using System.Collections.Generic;
using Digger.Modules.Core.Sources;
using Unity.Mathematics;
using UnityEngine;

namespace Domains.Scripts_that_Need_Sorting
{
    public class TerrainLayerDetector : MonoBehaviour
    {
        private static readonly List<Vector4> uvs = new();

        [Header("Targeted texture (filled during play)")]
        public string texture = "";

        public int textureIndex = -1;

        [SerializeField] private LayerMask playerMask;


        private void Start()
        {
            FindFirstObjectByType<DiggerMaster>();
        }

        // private void Update()
        // {
        //     var notPlayerMask = ~playerMask;
        //     if (Physics.Raycast(transform.position, transform.forward, out var hit, 500f, notPlayerMask))
        //     {
        //         UnityEngine.Debug.DrawLine(transform.position, hit.point, Color.green);
        //
        //         Terrain terrain;
        //         var index = GetTextureIndex(hit, out terrain);
        //
        //         if (index >= 0)
        //         {
        //             if (terrain)
        //             {
        //                 var layers = terrain.terrainData.terrainLayers;
        //                 if (index < layers.Length)
        //                 {
        //                     texture = $"name: {layers[index].name} | index: {index}";
        //                     textureIndex = index;
        //                 }
        //                 else
        //                 {
        //                     texture = $"Terrain index out of bounds: {index}";
        //                     textureIndex = -1;
        //                 }
        //             }
        //             else
        //             {
        //                 texture = $"Chunk texture index: {index}";
        //                 textureIndex = index;
        //             }
        //         }
        //         else
        //         {
        //             texture = "No valid texture hit";
        //             textureIndex = -1;
        //         }
        //     }
        // }

        public int GetTextureIndex(RaycastHit hit, out Terrain terrain)
        {
            terrain = null;

            var chunk = hit.collider.GetComponent<ChunkObject>();
            if (chunk)
            {
                terrain = chunk.Terrain;
                var mesh = chunk.Mesh;
                var triangles = mesh.triangles;
                var triArrayIndex = hit.triangleIndex * 3;

                if (triArrayIndex < 0 || triArrayIndex + 2 >= triangles.Length)
                    return -1;

                var baseVertexIndex = triangles[triArrayIndex];

                var texcoords = new float4[4];
                var valid =
                    TryGetTexcoord(mesh, baseVertexIndex, 1, out texcoords[0]) &
                    TryGetTexcoord(mesh, baseVertexIndex, 2, out texcoords[1]) &
                    TryGetTexcoord(mesh, baseVertexIndex, 3, out texcoords[2]);

                if (baseVertexIndex >= 0 && baseVertexIndex < mesh.tangents.Length)
                    texcoords[3] = mesh.tangents[baseVertexIndex];
                else
                    return -1;

                if (!valid)
                    return -1;

                return GetMeshTextureIndex(texcoords);
            }

            terrain = hit.collider.GetComponent<Terrain>();
            if (terrain)
                return GetTerrainTextureIndex(hit.point, terrain);

            return -1;
        }

        private static bool TryGetTexcoord(Mesh mesh, int baseVertexIndex, int channel, out float4 coord)
        {
            uvs.Clear();
            mesh.GetUVs(channel, uvs);
            if (baseVertexIndex < 0 || baseVertexIndex >= uvs.Count)
            {
                coord = float4.zero;
                return false;
            }

            coord = uvs[baseVertexIndex];
            return true;
        }

        private static float[] GetTextureMix(Vector3 worldPos, Terrain terrain)
        {
            var terrainData = terrain.terrainData;
            var terrainPos = terrain.transform.position;

            var mapX = (int)((worldPos.x - terrainPos.x) / terrainData.size.x * terrainData.alphamapWidth);
            var mapZ = (int)((worldPos.z - terrainPos.z) / terrainData.size.z * terrainData.alphamapHeight);

            var splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            var cellMix = new float[splatmapData.GetUpperBound(2) + 1];

            for (var n = 0; n < cellMix.Length; n++)
                cellMix[n] = splatmapData[0, 0, n];

            return cellMix;
        }

        private static int GetTerrainTextureIndex(Vector3 worldPos, Terrain terrain)
        {
            var mix = GetTextureMix(worldPos, terrain);
            var maxIndex = 0;
            var maxMix = 0f;

            for (var i = 0; i < mix.Length; i++)
                if (mix[i] > maxMix)
                {
                    maxMix = mix[i];
                    maxIndex = i;
                }

            return maxIndex;
        }

        public void UpdateFromHit(RaycastHit hit)
        {
            Terrain terrain;
            var index = GetTextureIndex(hit, out terrain);

            if (index >= 0)
            {
                if (terrain)
                {
                    var layers = terrain.terrainData.terrainLayers;
                    texture = index < layers.Length
                        ? $"name: {layers[index].name} | index: {index}"
                        : $"Terrain index out of bounds: {index}";
                }
                else
                {
                    texture = $"Chunk texture index: {index}";
                }

                textureIndex = index;
            }
            else
            {
                texture = "No valid texture hit";
                textureIndex = -1;
            }
        }

        public static int GetMeshTextureIndex(float4[] controls)
        {
            var index = -1;
            var max = -1f;

            for (var dc = 0; dc < controls.Length; dc++)
            {
                var tex = controls[dc];
                for (var df = 0; df < 4; df++)
                    if (tex[df] > max)
                    {
                        max = tex[df];
                        index = dc * 4 + df;
                    }
            }

            return index;
        }
    }
}