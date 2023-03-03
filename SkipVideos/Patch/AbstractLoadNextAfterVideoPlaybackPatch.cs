using HarmonyLib;

namespace SkipVideos.Patch;

public static class AbstractLoadNextAfterVideoPlaybackPatch
{
    [HarmonyPatch(typeof(AbstractLoadNextAfterVideoPlayback), nameof(AbstractLoadNextAfterVideoPlayback.Awake))]
    private static class Awake
    {
        private static void Postfix(AbstractLoadNextAfterVideoPlayback __instance)
        {
            __instance.videoSkipped = true;
        }
    }
}
