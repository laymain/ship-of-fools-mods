using HarmonyLib;
using Il2Cpp;

namespace ParagonMod.Patch;

public class ExtendedBoatHelm
{
    public delegate bool OnActivateDelegate();
    public static event OnActivateDelegate OnActivate;

    [HarmonyPatch(typeof(BoatHelm), nameof(BoatHelm.Activate), typeof(PlayerController))]
    private static class Activate
    {
        private static bool Prefix(PlayerController player)
        {
            return OnActivate?.Invoke() ?? true;
        }
    }
}
