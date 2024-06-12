using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using ParagonMod.Patch;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ParagonMod;

[RegisterTypeInIl2Cpp]
internal class Paragon : MonoBehaviour
{
    public static readonly ParagonState _state = new();
    private readonly ParagonEnemyManager _enemyManager;
    private readonly ParagonSaveGameManager _saveGameManager;
    private readonly ParagonScoreManager _scoreManager;
    private readonly ParagonMapManager _mapManager;

    private string _currentSceneName;
    private SceneController _sceneController;

    private WeakReference<GameState> _gameState = new(null);

    public Paragon(IntPtr ptr) : base(ptr)
    {
        _saveGameManager = new ParagonSaveGameManager(_state);
        _enemyManager = new ParagonEnemyManager(_state);
        _scoreManager = new ParagonScoreManager(_state);
        _mapManager = new ParagonMapManager(_state, _gameState);
    }

    public Paragon() : base(ClassInjector.DerivedConstructorPointer<Paragon>())
    {
        ClassInjector.DerivedConstructorBody(this);
        _saveGameManager = new ParagonSaveGameManager(_state);
        _enemyManager = new ParagonEnemyManager(_state);
        _scoreManager = new ParagonScoreManager(_state);
        _mapManager = new ParagonMapManager(_state, _gameState);
    }

    private void Awake()
    {
        SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(OnSceneLoaded));
        ExtendedOptions.OnOptionsStarted += options => ParagonUI.InjectGeneralOptions(_state, options);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;
        if (_currentSceneName == SceneUtils.GameScene || _currentSceneName == SceneUtils.TestScene)
        {
            var gameState = FindObjectOfType<GameState>();
            gameState.gameEvents.add_OnEnemySpawn(new Action<Enemy>(_enemyManager.OnEnemySpawn));
            gameState.gameEvents.add_OnRunEnded(new Action<bool>(victory => _state.OnRunEnded(victory, gameState)));
            gameState.gameEvents.add_OnEncounterCompleted(new Action(_state.OnEncounterCompleted));
            gameState.gameEvents.add_OnSectorChanged(new Action<Sector>(_mapManager.OnSectorChanged));
            gameState.gameEvents.add_OnRunEnded(new Action<bool>(_ => _mapManager.Reset()));
            _sceneController = FindObjectOfType<SceneController>();
            _sceneController.gameObject.AddComponent<ParagonUI>().Inject();
            _gameState.SetTarget(gameState);
        }
        else
        {
            _sceneController = null;
            _gameState.SetTarget(null);
        }
    }

    private bool CanEnableParagon()
    {
#if DEBUG
        return true;
#else
        return _state.Unlocked && _currentSceneName == SceneUtils.GameScene && _sceneController.State == SceneController.GameState.InHub;
#endif
    }

    public void ToggleParagonMode()
    {
        if (CanEnableParagon())
            _state.CurrentRunType = _state.CurrentRunType != ParagonState.RunType.PARAGON ? ParagonState.RunType.PARAGON : ParagonState.RunType.DEFAULT;
    }

    public void ToggleEndlessMode()
    {
        if (CanEnableParagon())
            _state.CurrentRunType = _state.CurrentRunType != ParagonState.RunType.ENDLESS ? ParagonState.RunType.ENDLESS : ParagonState.RunType.DEFAULT;
    }

}
