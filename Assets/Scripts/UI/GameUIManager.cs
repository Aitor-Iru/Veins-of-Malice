using UnityEngine;

namespace VeinsOfMalice.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject upgradesPanel;

        private void Start()
        {
            // Asegurar que al inicio están ocultos
            if (pausePanel) pausePanel.SetActive(false);
            if (gameOverPanel) gameOverPanel.SetActive(false);
            if (upgradesPanel) upgradesPanel.SetActive(false);
            
            // Si empezamos directamente en la escena y el estado ya es GameOver o Pause,
            // sincronizar con el estado actual
            if (GameManager.Instance != null)
            {
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
        }

        private void OnEnable()
        {
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.Gameplay:
                    if (pausePanel) pausePanel.SetActive(false);
                    if (gameOverPanel) gameOverPanel.SetActive(false);
                    if (upgradesPanel) upgradesPanel.SetActive(false);
                    break;
                case GameManager.GameState.Pause:
                    if (pausePanel) pausePanel.SetActive(true);
                    if (gameOverPanel) gameOverPanel.SetActive(false);
                    if (upgradesPanel) upgradesPanel.SetActive(false); // Siempre empieza en el menú principal de pausa
                    break;
                case GameManager.GameState.GameOver:
                    if (pausePanel) pausePanel.SetActive(false);
                    if (gameOverPanel) gameOverPanel.SetActive(true);
                    if (upgradesPanel) upgradesPanel.SetActive(false);
                    break;
                case GameManager.GameState.MainMenu:
                    if (pausePanel) pausePanel.SetActive(false);
                    if (gameOverPanel) gameOverPanel.SetActive(false);
                    if (upgradesPanel) upgradesPanel.SetActive(false);
                    break;
            }
        }

        // ── Metodos públicos para los Botones UI ───────────────────────────────

        public void ResumeGame()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Pause)
            {
                GameManager.Instance.TogglePause();
            }
        }

        public void OpenUpgradesMenu()
        {
            if (pausePanel) pausePanel.SetActive(false);
            if (upgradesPanel) upgradesPanel.SetActive(true);
        }

        public void CloseUpgradesMenu()
        {
            if (upgradesPanel) upgradesPanel.SetActive(false);
            if (pausePanel) pausePanel.SetActive(true);
        }

        public void RestartGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        public void GoToMainMenu()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToMainMenu();
            }
        }

        public void QuitGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }
    }
}
