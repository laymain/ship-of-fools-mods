using HarmonyLib;
using Il2Cpp;

namespace ParagonMod.Patch;

public static class ExtendedPersistenceManager
{
    public delegate void AfterGameLoadDelegate(GameData gameData);
    public static event AfterGameLoadDelegate AfterGameLoad;

    public delegate void BeforeGameSaveDelegate(GameData data);
    public static event BeforeGameSaveDelegate BeforeGameSave;

    [HarmonyPatch(typeof(PersistenceManager), nameof(PersistenceManager.LoadGame))]
    private static class LoadGame
    {
        private static void Postfix(Il2CppSystem.Threading.Tasks.Task<GameData> __result)
        {
            __result.GetAwaiter().OnCompleted(new Action(() => AfterGameLoad?.Invoke(__result.Result)));
        }
    }

    [HarmonyPatch(typeof(PersistenceManager), nameof(PersistenceManager.SaveGame))]
    private static class SaveGame
    {
        private static void Prefix(GameData data)
        {
            BeforeGameSave?.Invoke(data);
        }
    }
}
