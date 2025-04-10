using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

namespace SpaceGame.UI
{

    public class MultiplayerMenu : MonoBehaviour
    {
        public GameObject menu;
        public GameObject previousMenu;

        [Space]
        public UnityTransport unityTransport;

        private NetworkManager NetworkManager => NetworkManager.Singleton;

        public void OnHostPressed()
        {
            NetworkManager.StartHost();
        }

        public void OnServerPressed()
        {
            NetworkManager.StartServer();
        }

        public void OnClientPressed()
        {
            NetworkManager.StartClient();
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
    }

}
