using UnityEngine;

namespace SpaceGame
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        [Header("Cursor Initial Settings")]
        public CursorLockMode LockMode = CursorLockMode.None;
        public bool Visible = true;

        [SerializeField]
        private bool _isInPauseMenu = false;
        public bool IsInPauseMenu { get => _isInPauseMenu; set => _isInPauseMenu = value; }


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            // DontDestroyOnLoad(gameObject); // Persist across scenes
        }

        private void Start()
        {
            Cursor.lockState = Instance.LockMode;
            Cursor.visible = Instance.Visible;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && !IsInPauseMenu)
            {
                CaptureMouse();
            }
            else
            {
                FreeMouse();
            }
        }

        public void CaptureMouse()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        public void FreeMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void EnableMenuCursor()
        {
            IsInPauseMenu = false;
            FreeMouse();
        }

        public void DisableMenuCursor()
        {
            IsInPauseMenu = false;
            CaptureMouse();
        }
    }
}
