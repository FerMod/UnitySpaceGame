using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace SpaceGame.UI
{

    public class PauseMenu : MonoBehaviour
    {

        public InputActionAsset inputActionAsset;
        public GameObject pauseMenu;
        public GameObject optionsMenu;
        public InputActionProperty pauseInputAction;

        public Hud hud;

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
        }

        void ActivateMenu()
        {
            CursorManager.EnableMenuCursor();
            Time.timeScale = 0;
            AudioListener.pause = true;
            pauseMenu.SetActive(true);
            playerInputMap.Disable();
            hud.IsFreezed = true;
            isPaused = true;
        }

        void DeactivateMenu()
        {
            CursorManager.DisableMenuCursor();
            Time.timeScale = 1;
            AudioListener.pause = false;
            pauseMenu.SetActive(false);
            playerInputMap.Enable();
            hud.IsFreezed = false;
            isPaused = false;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
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
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }

        /// <summary>
        /// Method to deactivate an action map
        /// </summary>
        /// <param name="actionMapName"></param>
        private void DeactivateActionMap(string actionMapName)
        {
            var actionMap = inputActionAsset.FindActionMap(actionMapName);
            if (actionMap != null)
            {
                actionMap.Disable();
                Debug.Log($"{actionMapName} has been deactivated.");
            }
            else
            {
                Debug.LogWarning($"Action map {actionMapName} not found.");
            }
        }

        // Method to activate an action map
        public void ActivateActionMap(string actionMapName)
        {
            var actionMap = inputActionAsset.FindActionMap(actionMapName);
            if (actionMap != null)
            {
                actionMap.Enable();
                Debug.Log($"{actionMapName} has been activated.");
            }
            else
            {
                Debug.LogWarning($"Action map {actionMapName} not found.");
            }
        }
    }
}
