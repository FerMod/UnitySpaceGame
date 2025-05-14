using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceGame.UI
{

    public class StartMenu : MonoBehaviour
    {

        public GameObject mainMenu;
        public GameObject optionsMenu;

        private void Start()
        {
            CursorManager.Instance.FreeMouse();
        }

        public void OnStartGamePressed()
        {
            SceneManager.LoadScene("MainScene");
            CursorManager.Instance.CaptureMouse();
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
