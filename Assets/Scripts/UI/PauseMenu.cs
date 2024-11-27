using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SpaceGame.UI
{

    public class PauseMenu : MonoBehaviour
    {

        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private InputActionProperty pauseInputAction;

        private InputActionMap playerInputMap;
        private bool isPaused = false;

        private void Start()
        {
            pauseInputAction.action.Enable();
            pauseInputAction.action.performed += OnPause;
        }

        private void OnDestroy()
        {
            pauseInputAction.action.Disable();
        }

        void ActivateMenu()
        {
            Cursor.visible = true;
            Time.timeScale = 0;
            AudioListener.pause = true;
            pauseMenu.SetActive(true);
            isPaused = true;
        }

        void DeactivateMenu()
        {
            Cursor.visible = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            pauseMenu.SetActive(false);
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

        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }
    }
}
