using System;
using Digger.Modules.Core.Sources.Polygonizers;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Digger.Modules.Core.Sources.Jobs
{
    public class NativeCollectionsPool : IDisposable
    {
        private static NativeCollectionsPool instance;

        public static NativeCollectionsPool Instance {
            get {
                if (instance == null)
                    instance = new NativeCollectionsPool();
                return instance;
            }
        }

        private NativeArray<int>? mcEdgeTable;
        private NativeArray<int>? mcTriTable;
        private NativeArray<float3>? mcCorners;

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (mcEdgeTable.HasValue) {
                mcEdgeTable.Value.Dispose();
                mcEdgeTable = null;
            }

            if (mcTriTable.HasValue) {
                mcTriTable.Value.Dispose();
                mcTriTable = null;
            }

            if (mcCorners.HasValue) {
                mcCorners.Value.Dispose();
                mcCorners = null;
            }
        }

        public NativeArray<int> GetMCEdgeTable()
        {
            if (!mcEdgeTable.HasValue) {
                mcEdgeTable = new NativeArray<int>(MarchingCubesTables.ConstEdgeTable, Allocator.Persistent);
            }

            return mcEdgeTable.Value;
        }

        public NativeArray<int> GetMCTriTable()
        {
            if (!mcTriTable.HasValue) {
                mcTriTable = new NativeArray<int>(MarchingCubesTables.ConstTriTable, Allocator.Persistent);
            }

            return mcTriTable.Value;
        }

        public NativeArray<float3> GetMCCorners()
        {
            if (!mcCorners.HasValue) {
                mcCorners = new NativeArray<float3>(MarchingCubesTables.ConstCorners, Allocator.Persistent);
            }

            return mcCorners.Value;
        }
    }
}