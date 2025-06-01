using Unity.Burst;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Scripting;

namespace Domains.Debug.Config
{
    [Preserve]
    internal static class BurstGuardRuntime
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DisableBurstForNow()
        {
            return;

            // Grab the one global options object Unity creates
            var options = BurstCompiler.Options;
            options.EnableBurstCompilation = false; // <- instance property

            // Optional: also tell the Job System not to use any cached Burst funcs
            JobsUtility.JobCompilerEnabled = false;

            UnityEngine.Debug.Log("<color=orange>[BurstGuard]</color> Burst disabled via global options.");
        }
    }
}