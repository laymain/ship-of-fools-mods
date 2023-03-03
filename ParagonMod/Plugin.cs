using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ParagonMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource DefaultLogger;

    private void Awake()
    {
        try
        {
            DefaultLogger = Logger;

            var paragon = new GameObject().AddComponent<Paragon>();
            paragon.name = nameof(Paragon);
            DontDestroyOnLoad(paragon.gameObject);

            new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} by Laymain (version {MyPluginInfo.PLUGIN_VERSION}) loaded.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
}
