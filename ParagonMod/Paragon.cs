using System;
using ParagonMod.Patch;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ParagonMod;

internal class Paragon : MonoBehaviour
{
    private readonly ParagonState _state = new();
    private readonly ParagonEnemyManager _enemyManager;
    private readonly ParagonSaveGameManager _saveGameManager;
    private readonly ParagonScoreManager _scoreManager;
    private readonly ParagonMapManager _mapManager;

    private string _currentSceneName;
    private SceneController _sceneController;

    private WeakReference<GameState> _gameState = new(null);

    public Paragon()
    {
        _saveGameManager = new ParagonSaveGameManager(_state);
        _enemyManager = new ParagonEnemyManager(_state);
        _scoreManager = new ParagonScoreManager(_state);
        _mapManager = new ParagonMapManager(_state, _gameState);
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ExtendedOptions.OnOptionsStarted += options => ParagonUI.InjectGeneralOptions(_state, options);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;
        if (_currentSceneName == SceneUtils.GameScene || _currentSceneName == SceneUtils.TestScene)
        {
            var gameState = FindObjectOfType<GameState>();
            gameState.gameEvents.OnEnemySpawn += _enemyManager.OnEnemySpawn;
            gameState.gameEvents.OnRunEnded += victory => _state.OnRunEnded(victory, gameState);
            gameState.gameEvents.OnEncounterCompleted += _state.OnEncounterCompleted;
            gameState.gameEvents.OnSectorChanged += _mapManager.OnSectorChanged;
            gameState.gameEvents.OnRunEnded += _ => _mapManager.Reset();
            _sceneController = FindObjectOfType<SceneController>();
            _sceneController.gameObject.AddComponent<ParagonUI>().Inject(_state);
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

    private void Update()
    {
        if (CanEnableParagon())
        {
            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.P))
                _state.CurrentRunType = _state.CurrentRunType != ParagonState.RunType.PARAGON ? ParagonState.RunType.PARAGON : ParagonState.RunType.DEFAULT;
            else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.L))
                _state.CurrentRunType = _state.CurrentRunType != ParagonState.RunType.ENDLESS ? ParagonState.RunType.ENDLESS : ParagonState.RunType.DEFAULT;
        }
        #if DEBUG
        if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadPlus))
            _state.ParagonLevel += ReInput.controllers.Keyboard.GetModifierKey(ModifierKey.Shift) ? 10 : 1;
        else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadMinus))
            _state.ParagonLevel = Math.Max(1, _state.ParagonLevel - (ReInput.controllers.Keyboard.GetModifierKey(ModifierKey.Shift) ? 10 : 1));
        else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadMultiply))
            _state.EndlessLevel += ReInput.controllers.Keyboard.GetModifierKey(ModifierKey.Shift) ? 10 : 1;
        else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadDivide))
            _state.EndlessLevel = Math.Max(0, _state.EndlessLevel - (ReInput.controllers.Keyboard.GetModifierKey(ModifierKey.Shift) ? 10 : 1));
        else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadEnter))
            FindObjectOfType<GameState>()?.OnPersistentStateChanged.Emit(true);
        #endif
    }
}
