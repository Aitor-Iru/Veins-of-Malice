using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── State Machine ─────────────────────────────────────────────────────────
    public enum GameState
    {
        MainMenu,
        Gameplay,
        Pause,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    // ── Events (UI and other systems subscribe to these) ──────────────────────
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    public static event Action OnGameOver;

    // ── Scene Names ───────────────────────────────────────────────────────────
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameplayScene  = "GreyboxTest";

    // ── Player Reference ──────────────────────────────────────────────────────
    private PlayerHealth playerHealth;

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Subscribe to scene load to re-find the player each time
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Determine initial state based on which scene we're in
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == mainMenuScene)
            ChangeState(GameState.MainMenu);
        else
            ChangeState(GameState.Gameplay);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeFromPlayer();
    }

    // ── Scene Management ──────────────────────────────────────────────────────

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find player after scene load
        FindAndSubscribePlayer();

        if (scene.name == mainMenuScene)
            ChangeState(GameState.MainMenu);
        else
            ChangeState(GameState.Gameplay);
    }

    // ── State Machine ─────────────────────────────────────────────────────────

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"[GameManager] State → {newState}");

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;

            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;

            case GameState.Pause:
                Time.timeScale = 0f;
                OnGamePaused?.Invoke();
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                OnGameOver?.Invoke();
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Pausa / reanuda el juego. Llamar desde botón de pausa o tecla Escape.</summary>
    public void TogglePause()
    {
        if (CurrentState == GameState.Gameplay)
        {
            ChangeState(GameState.Pause);
        }
        else if (CurrentState == GameState.Pause)
        {
            ChangeState(GameState.Gameplay);
            OnGameResumed?.Invoke();
        }
    }

    /// <summary>Reinicia la escena de gameplay actual.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>Carga la escena de gameplay principal.</summary>
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplayScene);
    }

    /// <summary>Vuelve al menú principal.</summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>Cierra la aplicación.</summary>
    public void QuitGame()
    {
        Debug.Log("[GameManager] Quit.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── Player Health Integration ─────────────────────────────────────────────

    private void FindAndSubscribePlayer()
    {
        UnsubscribeFromPlayer();

        playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnDeath += HandlePlayerDeath;
            Debug.Log("[GameManager] Subscribed to PlayerHealth.OnDeath.");
        }
    }

    private void UnsubscribeFromPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
            playerHealth = null;
        }
    }

    private void HandlePlayerDeath()
    {
        ChangeState(GameState.GameOver);
    }

    // ── Input (Pause via Escape) ──────────────────────────────────────────────

    private void Update()
    {
        // New Input System — Keyboard.current is null if no keyboard is connected
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (CurrentState == GameState.Gameplay || CurrentState == GameState.Pause)
                TogglePause();
        }
    }
}
