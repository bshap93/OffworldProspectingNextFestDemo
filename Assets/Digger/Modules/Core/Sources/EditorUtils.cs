using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace Digger.Modules.Core.Sources
{
    public static class EditorUtils
    {
        public static bool MicroSplatExists(Terrain terrain)
        {
            return false;
        }

        public static T CreateOrReplaceAsset<T>(T asset, string path) where T : Object
        {
            if (!AssetDatabase.Contains(asset)) AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        public static Mesh CreateOrUpdateMeshAsset(Mesh asset, string path)
        {
            if (!AssetDatabase.Contains(asset)) AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static bool AreMeshesEqual(Mesh mesh1, Mesh mesh2)
        {
            return mesh1 != null && mesh2 != null &&
                   mesh1.vertexCount == mesh2.vertexCount &&
                   mesh1.subMeshCount == mesh2.subMeshCount &&
                   mesh1.vertices.SequenceEqual(mesh2.vertices) &&
                   mesh1.colors.SequenceEqual(mesh2.colors) &&
                   mesh1.uv.SequenceEqual(mesh2.uv) &&
                   mesh1.uv2.SequenceEqual(mesh2.uv2) &&
                   mesh1.uv3.SequenceEqual(mesh2.uv3) &&
                   mesh1.uv4.SequenceEqual(mesh2.uv4) &&
                   mesh1.normals.SequenceEqual(mesh2.normals) &&
                   mesh1.tangents.SequenceEqual(mesh2.tangents);
        }

        public static int AspectSelectionGrid(int selected, Texture[] textures, int approxSize, GUIStyle style,
            GUIContent errorMessage)
        {
            GUILayout.BeginVertical("box", GUILayout.MinHeight(approxSize));
            var newSelected = 0;

            if (textures != null && textures.Length != 0)
            {
                var columns = Mathf.Max((int)(EditorGUIUtility.currentViewWidth - 150) / approxSize, 1);
                // ReSharper disable once PossibleLossOfFraction
                var rows = Mathf.Max((int)Mathf.Ceil((textures.Length + columns - 1) / columns), 1);
                var r = GUILayoutUtility.GetAspectRect(columns / (float)rows);

                var texturesPreview = new Texture[textures.Length];
                for (var i = 0; i < textures.Length; ++i)
                    texturesPreview[i] = textures[i]
                        ? AssetPreview.GetAssetPreview(textures[i]) ?? textures[i]
                        : EditorGUIUtility.whiteTexture;

                newSelected = GUI.SelectionGrid(r, Math.Min(selected, texturesPreview.Length - 1), texturesPreview,
                    columns, style);
            }
            else
            {
                GUILayout.Label(errorMessage);
            }

            GUILayout.EndVertical();
            return newSelected;
        }
    }
}

#endif