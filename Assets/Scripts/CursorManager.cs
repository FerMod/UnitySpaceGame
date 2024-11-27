using UnityEngine;

namespace SpaceGame
{
    public class CursorManager : MonoBehaviour
    {
        public CursorLockMode cursorLockMode = CursorLockMode.Confined;
        public bool visible = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            Cursor.lockState = cursorLockMode;
            Cursor.visible = visible;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Cursor.lockState = cursorLockMode;
                Cursor.visible = visible;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
