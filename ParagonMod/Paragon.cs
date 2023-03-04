using System;
using ParagonMod.UI;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ParagonMod;

internal class Paragon : MonoBehaviour
{
    private const string GameSceneName = "GameScene";

    private readonly ParagonState _state = new();
    private readonly ParagonEnemyManager _enemyManager;
    private readonly ParagonSaveGameManager _saveGameManager;
    private readonly ParagonScoreManager _scoreManager;

    private string _currentSceneName;
    private SceneController _sceneController;

    public Paragon()
    {
        _saveGameManager = new ParagonSaveGameManager(_state);
        _enemyManager = new ParagonEnemyManager(_state);
        _scoreManager = new ParagonScoreManager(_state);
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;
        if (_currentSceneName == GameSceneName)
        {
            var gameState = FindObjectOfType<GameState>();
            gameState.gameEvents.OnEnemySpawn += _enemyManager.OnEnemySpawn;
            gameState.gameEvents.OnRunEnded += victory => _state.OnRunEnded(victory, gameState);
            _sceneController = FindObjectOfType<SceneController>();
            _sceneController.gameObject.AddComponent<ParagonUI>().Inject(_state);
        }
        else
        {
            _sceneController = null;
        }
    }

    private bool CanEnableParagon()
    {
    #if DEBUG
        return true;
    #else
        return _state.Unlocked && _currentSceneName == GameSceneName && _sceneController.State == SceneController.GameState.InHub;
    #endif
    }

    private void Update()
    {
        if (CanEnableParagon())
        {
            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.P))
                _state.Enabled = !_state.Enabled;
        }
        #if DEBUG
        if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadPlus))
            _state.Level += ReInput.controllers.Keyboard.GetModifierKey(ModifierKey.Shift) ? 10 : 1;
        else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadMinus))
            _state.Level = Math.Max(1, _state.Level - (ReInput.controllers.Keyboard.GetModifierKey(ModifierKey.Shift) ? 10 : 1));
        else if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.KeypadEnter))
            FindObjectOfType<GameState>()?.OnPersistentStateChanged.Emit(true);
        #endif
    }
}
