using SpaceGame.UI;
using UnityEngine;

namespace SpaceGame
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorLockMode lockMode = CursorLockMode.Confined;
        public static bool visible = false;

        public static bool IsInPauseMenu { get; set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            Cursor.lockState = lockMode;
            Cursor.visible = visible;
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

        public static void CaptureMouse()
        {
            Cursor.lockState = lockMode;
            Cursor.visible = visible;
        }

        public static void FreeMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public static void EnableMenuCursor()
        {
            IsInPauseMenu = false;
            FreeMouse();
        }

        public static void DisableMenuCursor()
        {
            IsInPauseMenu = false;
            CaptureMouse();
        }
    }
}
