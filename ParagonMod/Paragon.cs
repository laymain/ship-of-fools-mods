using ParagonMod.UI;
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
        if (scene.name == "GameScene")
        {
            var gameState = FindObjectOfType<GameState>();
            gameState.gameEvents.OnEnemySpawn += _enemyManager.OnEnemySpawn;
            gameState.gameEvents.OnEnemyDeath += _enemyManager.OnEnemyDeath;
            gameState.gameEvents.OnRunEnded += _enemyManager.OnRunEnded;
            gameState.gameEvents.OnRunEnded += victory => _state.OnRunEnded(victory, gameState);
            _sceneController = FindObjectOfType<SceneController>();
            _sceneController.gameObject.AddComponent<ParagonUI>().Inject(_state);
        }
    }

    private void Update()
    {
        if (_state.Unlocked && ReInput.controllers.Keyboard.GetKeyDown(KeyCode.P) && _sceneController != null && _sceneController.State == SceneController.GameState.InHub)
            _state.Enabled = !_state.Enabled;
    }

}
