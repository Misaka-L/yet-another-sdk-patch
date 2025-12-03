using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using Unity.Profiling;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using YesPatchFrameworkForVRChatSdk.PatchApi;
using Object = UnityEngine.Object;

namespace YetAnotherPatchForVRChatSdk.Worlds.Patches;

internal sealed class UdonProfilerPatch : YesPatchBase
{
    public override string Id => "xyz.misakal.vpm.yet-another-sdk-patch.worlds.udon-profiler";
    public override string DisplayName => "Udon Profiler";

    public override string Description =>
        "Add more detail markers to Unity Profiler for Udon behaviours execution.";

    private readonly Harmony _harmony = new("xyz.misakal.vpm.yet-another-sdk-patch.worlds.udon-profiler");

    public override void Patch()
    {
        _harmony.PatchAll(typeof(UdonProfilerPatch));
    }

    public override void UnPatch()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPatch(typeof(UdonBehaviour), nameof(UdonBehaviour.RunProgram), typeof(uint))]
    [HarmonyPrefix]
    private static void UdonBehaviourRunProgramPrefix(
        uint entryPoint,
        UdonBehaviour __instance,
        ref IUdonProgram ____program,
        out ProfilerMarkerScope __state)
    {
        var marker = ProfilerMarkerScope.Auto(__instance, ____program, entryPoint);
        __state = marker;
    }

    [HarmonyPatch(typeof(UdonBehaviour), nameof(UdonBehaviour.RunProgram), typeof(uint))]
    [HarmonyPostfix]
    private static void UdonBehaviourRunProgramPostfix(ref ProfilerMarkerScope __state)
    {
        __state.Dispose();
    }

    private readonly struct ProfilerMarkerScope : IDisposable
    {
        private readonly ProfilerMarker _marker;

        private ProfilerMarkerScope(ProfilerMarker profilerMarker, Object obj)
        {
            _marker = profilerMarker;
            _marker.Begin(obj);
        }

        public void Dispose()
        {
            _marker.End();
        }

        private static readonly Dictionary<(int, uint), ProfilerMarker> ProfilerMarkerEventDict = new();

        [PublicAPI]
        public static ProfilerMarkerScope Auto(UdonBehaviour udonBehaviour, IUdonProgram udonProgram, uint entryPoint)
        {
            var programID = udonBehaviour.programSource.GetInstanceID();
            var key = (programID, entryPoint);
            if (!ProfilerMarkerEventDict.TryGetValue(key, out var profilerMarker))
            {
                var programName = udonBehaviour.programSource.name;
                if (!udonProgram.EntryPoints.TryGetSymbolFromAddress(entryPoint, out var symbolName))
                {
                    symbolName = "[UNKNOWN]";
                }

                profilerMarker = new ProfilerMarker($"Udon {programName}.{symbolName}");
                ProfilerMarkerEventDict[key] = profilerMarker;
            }

            return new ProfilerMarkerScope(profilerMarker, udonBehaviour.gameObject);
        }
    }
}