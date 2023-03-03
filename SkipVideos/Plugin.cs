using System;
using BepInEx;
using HarmonyLib;

namespace SkipVideos;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        try
        {
            new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} by Laymain (version {MyPluginInfo.PLUGIN_VERSION}) loaded.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
}
