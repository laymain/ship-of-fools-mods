using ParagonMod.Patch;

namespace ParagonMod;

public class ParagonSaveGameManager
{
    private const string ParagonLevelKey = "eaec083c-ef76-49a3-a531-1f0796fdeb6b";

    private readonly ParagonState _state;

    public ParagonSaveGameManager(ParagonState state)
    {
        _state = state;
        ExtendedPersistenceManager.AfterGameLoad += Load;
        ExtendedPersistenceManager.BeforeGameSave += Save;
    }

    private void Load(GameData data)
    {
        if (data != null)
        {
            if (data.Unlocked?.ContainsKey(ParagonLevelKey) == true)
                _state.ParagonLevel = data.Unlocked[ParagonLevelKey];
            _state.Unlocked = ((Progression)data.Progression).HasProgressed(Progression.EyeOfTheStormDefeated);
        }
        Plugin.DefaultLogger.LogDebug("GameData loaded");
    }

    private void Save(GameData data)
    {
        if (data?.Unlocked != null)
        {
            if (_state.ParagonLevel > 1)
                data.Unlocked[ParagonLevelKey] = _state.ParagonLevel;
        }

        Plugin.DefaultLogger.LogDebug("GameData saved");
    }
}
