using Il2Cpp;

namespace ParagonMod;

public class ParagonState
{
    #region Unlocked

    public delegate void OnUnlockStateChangedDelegate(bool unlocked);

    public event OnUnlockStateChangedDelegate OnUnlockStateChanged;

    private bool _unlocked;

    public bool Unlocked
    {
        get => _unlocked;
        set
        {
            if (_unlocked != value)
            {
                _unlocked = value;
                OnUnlockStateChanged?.Invoke(_unlocked);
            }
        }
    }

    #endregion

    #region RunType

    public enum RunType
    {
        DEFAULT,
        PARAGON,
        ENDLESS
    }

    public delegate void OnRunTypeChangedDelegate(RunType runType);

    public event OnRunTypeChangedDelegate OnRunTypeChanged;

    private RunType _currentRunType = RunType.DEFAULT;

    public RunType CurrentRunType
    {
        get => _currentRunType;
        set
        {
            if (_currentRunType != value)
            {
                _currentRunType = value;
                OnRunTypeChanged?.Invoke(_currentRunType);
            }
        }
    }

    #endregion

    #region Paragon

    public delegate void OnParagonLevelChangedDelegate(int level);

    public event OnParagonLevelChangedDelegate OnParagonLevelChanged;

    private int _paragonLevel = 1;

    public int ParagonLevel
    {
        get => _paragonLevel;
        set
        {
            if (_paragonLevel != value)
            {
                _paragonLevel = value;
                OnParagonLevelChanged?.Invoke(_paragonLevel);
            }
        }
    }

    #endregion

    #region Difficulty

    public int DifficultyModifier => _currentRunType switch
    {
        RunType.PARAGON => _paragonLevel,
        RunType.ENDLESS => _endlessLevel,
        _ => 0
    } + CurrentDifficulty.GetDifficultyModifier();

    public ParagonDifficulty CurrentDifficulty { get; set; } = ParagonDifficulty.DEFAULT;

    #endregion

    #region Endless

    public delegate void OnEndlessLevelChangedDelegate(int level);

    public event OnEndlessLevelChangedDelegate OnEndlessLevelChanged;

    private int _endlessLevel = 0;

    public int EndlessLevel
    {
        get => _endlessLevel;
        set
        {
            if (_endlessLevel != value)
            {
                _endlessLevel = value;
                OnEndlessLevelChanged?.Invoke(_endlessLevel);
            }
        }
    }

    #endregion

    #region Events

    public void OnRunEnded(bool victory, GameState gameState)
    {
        Mod.DefaultLogger.Msg($"Run ended: victory = {victory}");
        if (CurrentRunType == RunType.ENDLESS)
            EndlessLevel = 0;
        if (victory)
        {
            if (!Unlocked)
            {
                Mod.DefaultLogger.Msg("Paragon unlocked");
                Unlocked = true;
            }

            if (CurrentRunType == RunType.PARAGON)
            {
                Mod.DefaultLogger.Msg("Paragon level up");
                ParagonLevel++;
                gameState.OnPersistentStateChanged.Emit(true);
            }
        }
    }

    public void OnEncounterCompleted()
    {
        if (CurrentRunType == RunType.ENDLESS)
        {
            Mod.DefaultLogger.Msg("Endless level up");
            EndlessLevel++;
        }
    }

    #endregion
}
