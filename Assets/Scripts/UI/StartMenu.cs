using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceGame.UI
{

    public class StartMenu : MonoBehaviour
    {

        private void Awake()
        {
            CursorManager.FreeMouse();
        }

        public void OnStartGamePressed()
        {
            SceneManager.LoadScene("MainScene");
            CursorManager.CaptureMouse();
        }

        public void OnOptionsPressed()
        {
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
