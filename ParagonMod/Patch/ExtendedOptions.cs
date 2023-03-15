using System.Linq;
using HarmonyLib;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ParagonMod.Patch;

public static class ExtendedOptions
{
    public delegate void OnOptionsStartedDelegate(Options options);
    public static event OnOptionsStartedDelegate OnOptionsStarted;

    [HarmonyPatch(typeof(Options), nameof(Options.Start))]
    private static class Start
    {
        private static void Postfix(Options __instance)
        {
            OnOptionsStarted?.Invoke(__instance);
        }
    }

    [HarmonyPatch(typeof(Options), nameof(Options.SetApplyButton), typeof(bool))]
    private static class SetApplyButton
    {
        // Fix SetApplyButton to set up automatically navigations
        private static bool Prefix(Options __instance, bool bOn)
        {
            Plugin.DefaultLogger.LogDebug("Setting UI navigations...");
            __instance.buttonApplyResolution.gameObject.SetActive(bOn);
            Selectable[] selectables = __instance.transform.Find("SubOptions/General")
                .GetComponentsInChildren<Selectable>(false)
                .Where(selectable => selectable.navigation.mode == Navigation.Mode.Explicit)
                .ToArray();
            for (var i = 0; i < selectables.Length; i++)
            {
                selectables[i].navigation = new Navigation
                {
                    mode = selectables[i].navigation.mode,
                    selectOnUp = selectables[i > 0 ? i - 1 : selectables.Length - 1],
                    selectOnDown = selectables[i < selectables.Length - 1 ? i + 1 : 0],
                    selectOnLeft = selectables[i].navigation.selectOnLeft,
                    selectOnRight = selectables[i].navigation.selectOnLeft
                };
            }
            if (!bOn && selectables.Length > 0)
            {
                EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
            }
            return false;
        }
    }
}
