using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceGame.UI
{

    public class MultiplayerMenu : MonoBehaviour
    {
        public GameObject menu;
        public GameObject previousMenu;
        public string mainScene = "MainScene";

        [Space]
        public UnityTransport unityTransport;

        private NetworkManager NetworkManager => NetworkManager.Singleton;

        public void OnHostPressed()
        {
            if (NetworkManager.StartHost())
            {
                //LoadScene(mainScene);
            }
        }

        public void OnServerPressed()
        {
            if (NetworkManager.StartServer())
            {
                //LoadScene(mainScene);
            }
        }

        public void OnClientPressed()
        {
            if (NetworkManager.StartClient())
            {
                //LoadScene(mainScene);
            }
        }

        public void OnIpChanged(string value)
        {
            unityTransport.ConnectionData.Address = value;
        }

        public void OnBackPressed()
        {
            menu.SetActive(false);
            previousMenu.SetActive(true);
        }

        private void LoadScene(string sceneName)
        {
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            CursorManager.Instance.CaptureMouse();
        }
    }

}
