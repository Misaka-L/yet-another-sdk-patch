using HarmonyLib;
using YesPatchFrameworkForVRChatSdk.PatchApi;

#if !YAP4VRC_VRCFURY_EXIST
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEditor.Animations;
using UnityEngine;
using YesPatchFrameworkForVRChatSdk.PatchApi.Logging;
#endif

namespace YetAnotherPatchForVRChatSdk.Avatars.Patches;

using YesPatchFrameworkForVRChatSdk.PatchApi.Extensions;

[HarmonyPatch]
internal sealed class ParameterDriverEditorAlwaysSpawnAnimatorWindowPatch : YesPatchBase
{
    public override string Id =>
        "xyz.misakal.vpm.yet-another-sdk-patch.avatars.parameter-driver-editor-always-spawn-animator-window";

    public override string Category => "Avatars SDK bugs fixes";

    public override string Description =>
        "Fix a issues that exit PlayMode when parameter driver editor visible may result in animator window always be spawn after reload domain.";

    public override string DisplayName => "Fix Parameter Driver Editor Always Spawn Animator Window";

#if !YAP4VRC_VRCFURY_EXIST
    private readonly Harmony _harmony =
        new("xyz.misakal.vpm.yet-another-sdk-patch.avatars.parameter-driver-editor-always-spawn-animator-window");

    private static readonly YesLogger Logger = new(nameof(ParameterDriverEditorAlwaysSpawnAnimatorWindowPatch));

    private static Type? _animatorControllerToolType;
    private static PropertyInfo? _animatorControllerPropertyInfo;

    public override void Patch()
    {
        _harmony.PatchAll(typeof(ParameterDriverEditorAlwaysSpawnAnimatorWindowPatch));
        _animatorControllerToolType ??= AccessTools.TypeByName("UnityEditor.Graphs.AnimatorControllerTool");
        _animatorControllerPropertyInfo ??= AccessTools.Property(_animatorControllerToolType, "animatorController");
    }

    public override void UnPatch()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPatch(typeof(AvatarParameterDriverEditor), "GetCurrentController")]
    [HarmonyPrefix]
    private static bool GetCurrentControllerPrefix([MaybeNull] ref AnimatorController __result)
    {
        if (_animatorControllerToolType is null)
        {
            Logger.LogError(
                "UnityEditor.Graphs.AnimatorControllerTool type not found, cannot patch ParameterDriverEditor.");
            return true;
        }

        if (_animatorControllerPropertyInfo is null)
        {
            Logger.LogError(
                "UnityEditor.Graphs.AnimatorControllerTool property not found, cannot patch ParameterDriverEditor.");
            return true;
        }

        var animatorWindows = Resources.FindObjectsOfTypeAll(_animatorControllerToolType);
        if (animatorWindows is null or { Length: 0 })
        {
            Logger.LogTrace("No animator window found.");
            return false;
        }

        __result = _animatorControllerPropertyInfo.GetValue(animatorWindows.First(), null) as AnimatorController;
        return false;
    }

#else

    public override void Patch()
    {
    }

    public override void UnPatch()
    {
    }

#endif
}