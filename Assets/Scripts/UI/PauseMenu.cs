using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SpaceGame.UI
{

    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject optionsMenu;
        public GameObject inGameHud;

        public InputActionAsset inputActionAsset;
        public InputActionProperty pauseInputAction;
        private InputActionMap playerInputMap;

        private bool isPaused = false;

        private void Start()
        {
            playerInputMap = inputActionAsset.FindActionMap("Plane");

            pauseInputAction.action.Enable();
            pauseInputAction.action.performed += OnPause;
        }

        private void OnDestroy()
        {
            pauseInputAction.action.Disable();
            pauseInputAction.action.performed -= OnPause;
        }

        void SetMenuActive(bool active)
        {
            SetGamePaused(active);

            pauseMenu.SetActive(active);
            inGameHud.SetActive(!active);

            if (active)
            {
                playerInputMap.Disable();
            }
            else
            {
                playerInputMap.Enable();
            }

            isPaused = active;
        }

        private void SetGamePaused(bool paused)
        {
            if (paused)
            {
                CursorManager.EnableMenuCursor();
            }
            else
            {
                CursorManager.DisableMenuCursor();
            }

            Time.timeScale = paused ? 0 : 1;
            AudioListener.pause = paused;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (optionsMenu.activeSelf)
            {
                DisableOptionsMenu();
                return;
            }

            isPaused = !isPaused;
            SetMenuActive(isPaused);
        }

        public void OnResumePressed()
        {
            SetMenuActive(false);
        }

        public void OnRestartPressed()
        {
            SceneManager.LoadScene("MainScene");
            SetMenuActive(false);
        }

        public void OnOptionsPressed()
        {
            EnableOptionsMenu();
        }

        public void OnMainMenuPressed()
        {
            SceneManager.LoadScene("StartMenu");
            SetMenuActive(false);
        }

        public void OnQuitPressed()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #region Options Menu
        public void EnableOptionsMenu()
        {
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }

        public void DisableOptionsMenu()
        {
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
        #endregion
    }
}
