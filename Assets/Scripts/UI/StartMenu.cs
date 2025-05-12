using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceGame.UI
{

    public class StartMenu : MonoBehaviour
    {

        public GameObject mainMenu;
        public GameObject multiplayerMenu;
        public GameObject optionsMenu;

        private void Start()
        {
            CursorManager.FreeMouse();
        }

        public void OnSinglePlayerPressed()
        {
            SceneManager.LoadScene("MainScene");
            CursorManager.CaptureMouse();
        }
        public void OnMultiplayerPressed()
        {
            mainMenu.SetActive(false);
            multiplayerMenu.SetActive(true);
        }

        public void OnOptionsPressed()
        {
            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }

        public void OnQuitPressed()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}
