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

    #region Enabled

    public delegate void OnEnableStateChangedDelegate(bool enabled);
    public event OnEnableStateChangedDelegate OnEnableStateChanged;

    private bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                OnEnableStateChanged?.Invoke(_enabled);
            }
        }
    }

    #endregion

    #region Level

    public delegate void OnLevelChangedDelegate(int level);
    public event OnLevelChangedDelegate OnLevelChanged;

    private int _level;
    public int Level
    {
        get => _level;
        set
        {
            if (_level != value)
            {
                _level = value;
                OnLevelChanged?.Invoke(_level);
            }
        }
    }

    #endregion

    #region Events

    public void OnRunEnded(bool victory)
    {
        if (victory)
        {
            Unlocked = true;
            if (Enabled)
                Level++;
        }
    }

    #endregion

}
