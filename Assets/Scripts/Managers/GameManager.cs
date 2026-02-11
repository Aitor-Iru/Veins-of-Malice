using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Gameplay,
        Pause,
        GameOver
    }

    public GameState CurrentState { get; private set; }

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
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Game State Changed to: {newState}");
        
        switch (newState)
        {
            case GameState.MainMenu:
                // Handle Main Menu logic
                break;
            case GameState.Gameplay:
                // Handle Gameplay logic
                Time.timeScale = 1f;
                break;
            case GameState.Pause:
                // Handle Pause logic
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                // Handle Game Over logic
                break;
        }
    }
}
