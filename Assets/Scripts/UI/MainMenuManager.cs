using UnityEngine;

namespace VeinsOfMalice.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public void OnPlayClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
            else
            {
                Debug.LogError("[MainMenuManager] GameManager.Instance is null! Asegúrate de que existe un GameManager en la escena.");
            }
        }

        public void OnOptionsClicked()
        {
            Debug.Log("[MainMenuManager] Options clicked (Not yet implemented)");
        }

        public void OnQuitClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
            else
            {
                Debug.Log("[MainMenuManager] Quit Game fallback.");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
