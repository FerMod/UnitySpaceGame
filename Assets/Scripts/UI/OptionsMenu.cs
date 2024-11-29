using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace SpaceGame.UI
{

    public class OptionsMenu : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject optionsMenu;

        public void OnBackPressed()
        {
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }
}
