using HarmonyLib;
using Il2Cpp;

namespace ParagonMod.Patch;

public static class ExtendedRunStatsManager
{
    [HarmonyPatch(typeof(RunStatsManager), nameof(RunStatsManager.Awake))]
    private static class Awake
    {
        private static void Postfix(RunStatsManager __instance)
        {
            // Increase max score limits
            __instance.ScoreThresholds = new[] { 100, 500, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 15000, 20000, 30000, 50000, 100000, 1000000 };
        }
    }

    public delegate int OnApplyScoreModifierDelegate(string name, int score);
    public static event OnApplyScoreModifierDelegate OnApplyScoreModifier;

    [HarmonyPatch(typeof(RunStatsManager), nameof(RunStatsManager.ItemScore), MethodType.Getter)]
    private static class ItemScoreGetter
    {
        public static void Postfix(ref int __result)
        {
            if (OnApplyScoreModifier != null)
                __result = OnApplyScoreModifier(nameof(RunStatsManager.ItemScore), __result);
        }
    }

    [HarmonyPatch(typeof(RunStatsManager), nameof(RunStatsManager.EnemiesDestroyedScore), MethodType.Getter)]
    private static class EnemiesDestroyedScoreGetter
    {
        public static void Postfix(ref int __result)
        {
            if (OnApplyScoreModifier != null)
                __result = OnApplyScoreModifier(nameof(RunStatsManager.EnemiesDestroyedScore), __result);
        }
    }

    [HarmonyPatch(typeof(RunStatsManager), nameof(RunStatsManager.CashScore), MethodType.Getter)]
    private static class CashScoreGetter
    {
        public static void Postfix(ref int __result)
        {
            if (OnApplyScoreModifier != null)
                __result = OnApplyScoreModifier(nameof(RunStatsManager.CashScore), __result);
        }
    }
}
