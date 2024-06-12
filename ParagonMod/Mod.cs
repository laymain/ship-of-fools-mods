using System.Reflection;
using MelonLoader;
using ParagonMod;
using Rewired;
using UnityEngine;

[assembly: AssemblyVersion(Mod.Version)]
[assembly: MelonInfo(typeof(Mod), Mod.Name, Mod.Version, Mod.Author)]
[assembly: MelonGame("Fika Productions", "ShipOfFools")]

namespace ParagonMod;

public class Mod : MelonMod
{
    internal const string Name = nameof(ParagonMod);
    internal const string Version = "0.0.8";
    internal const string Author = "Laymain";

    private Paragon _paragon;

    public static MelonLogger.Instance DefaultLogger => Melon<Mod>.Logger;

    public override void OnInitializeMelon()
    {
        try
        {
            _paragon = new GameObject().AddComponent<Paragon>();
            _paragon.name = nameof(Paragon);

            LoggerInstance.Msg($"Plugin {Name} by {Author} (version {Version}) loaded.");
        }
        catch (Exception ex)
        {
            LoggerInstance.Error("An unexpected error has occurred", ex);
        }
    }

    public override void OnUpdate()
    {
        try
        {
            if (!ReInput.isReady) return;
            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.P))
                _paragon.ToggleParagonMode();
            else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.L))
                _paragon.ToggleEndlessMode();
        }
        catch (Exception ex)
        {
            LoggerInstance.Error("Unexpected exception", ex);
        }
    }
}
