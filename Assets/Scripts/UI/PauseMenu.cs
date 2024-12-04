using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

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

        void ActivateMenu()
        {
            CursorManager.EnableMenuCursor();
            Time.timeScale = 0;
            AudioListener.pause = true;
            pauseMenu.SetActive(true);
            inGameHud.SetActive(false);
            playerInputMap.Disable();
            isPaused = true;
        }

        void DeactivateMenu()
        {
            CursorManager.DisableMenuCursor();
            Time.timeScale = 1;
            AudioListener.pause = false;
            pauseMenu.SetActive(false);
            inGameHud.SetActive(true);
            playerInputMap.Enable();
            isPaused = false;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (optionsMenu.activeSelf)
            {
                DisableOptionsMenu();
                return;
            }

            isPaused = !isPaused;
            if (isPaused)
            {
                ActivateMenu();
            }
            else
            {
                DeactivateMenu();
            }

        }

        public void OnResumePressed()
        {
            DeactivateMenu();
        }

        public void OnOptionsPressed()
        {
            EnableOptionsMenu();
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
