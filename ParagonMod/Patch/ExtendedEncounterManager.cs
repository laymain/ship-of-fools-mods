using HarmonyLib;
using Il2Cpp;

namespace ParagonMod.Patch;

public class ExtendedEncounterManager
{
    public delegate Encounter OnSelectEncounterDelegate(EncounterManager encounterManager, MapNode node);
    public static event OnSelectEncounterDelegate OnSelectEncounter;

    [HarmonyPatch(typeof(EncounterManager), nameof(SelectEncounterFor), typeof(MapNode))]
    private static class SelectEncounterFor
    {
        private static bool Prefix(EncounterManager __instance, MapNode node, ref Encounter __result)
        {
            Encounter encounter = OnSelectEncounter?.Invoke(__instance, node);
            if (encounter != null)
            {
                __result = encounter;
                return false;
            }
            return true;
        }
    }
}
