using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace SkipVideos.Patch;

public static  class AbstractLoadNextAfterVideoPlaybackPatch
{
    [HarmonyPatch(typeof(AbstractLoadNextAfterVideoPlayback), nameof(AbstractLoadNextAfterVideoPlayback.Awake))]
    private static class Awake
    {
        private static void Postfix(AbstractLoadNextAfterVideoPlayback __instance)
        {
            __instance.videoSkipped = true;
            Melon<SkipVideosMod>.Logger.Msg($"Skipping video: {__instance.name}");
        }
    }
}
