using System.Threading.Tasks;
using HarmonyLib;

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
        private static async void Postfix(Task<GameData> __result)
        {
            GameData data = await __result;
            AfterGameLoad?.Invoke(data);
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
