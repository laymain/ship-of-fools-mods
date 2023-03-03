using ParagonMod.Patch;

namespace ParagonMod;

public class ParagonSaveGameManager
{
    private const string SaveLevelKey = "eaec083c-ef76-49a3-a531-1f0796fdeb6b";

    private readonly ParagonState _state;

    public ParagonSaveGameManager(ParagonState state)
    {
        _state = state;
        ExtendedPersistenceManager.AfterGameLoad += Load;
        ExtendedPersistenceManager.BeforeGameSave += Save;
    }

    private void Load(GameData data)
    {
        _state.Level = 1;
        _state.Unlocked = false;
        if (data != null)
        {
            if (data.Unlocked?.ContainsKey(SaveLevelKey) == true)
                _state.Level = data.Unlocked[SaveLevelKey];
            _state.Unlocked = ((Progression)data.Progression).HasProgressed(Progression.EyeOfTheStormDefeated);
        }
        Plugin.DefaultLogger.LogDebug("GameData loaded");
    }

    private void Save(GameData data)
    {
        if (data != null && _state.Level != 1)
            data.Unlocked[SaveLevelKey] = _state.Level;
        Plugin.DefaultLogger.LogDebug("GameData saved");
    }
}
