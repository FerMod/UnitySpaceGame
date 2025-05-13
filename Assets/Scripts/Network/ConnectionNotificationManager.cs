using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;


/// <summary>
/// Clase para guardar los datos del jugador
/// </summary>
[Serializable]
public class PlayerData
{
    public int playerPrefab = 0;
    public string playerName;
    public GameObject uiPanel;
}

/// <summary>
/// Only attach this example component to the NetworkManager GameObject.
/// This will provide you with a single location to register for client
/// connect and disconnect events.  
/// </summary>
public class ConnectionNotificationManager : MonoBehaviour
{
    public GameObject[] playerPrefabs;

    public int maxPlayers = 2;

    public GameObject PanelPlayers;
    public GameObject PrefabPlayerUI;

    public Dictionary<ulong, PlayerData> DictPlayerUI = new();

    public static ConnectionNotificationManager Singleton { get; internal set; }

    public enum ConnectionStatus
    {
        Connected,
        Disconnected
    }

    /// <summary>
    /// This action is invoked whenever a client connects or disconnects from the game.
    ///   The first parameter is the ID of the client (ulong).
    ///   The second parameter is whether that client is connecting or disconnecting.
    /// </summary>
    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;

    private void Awake()
    {
        if (Singleton != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(ConnectionNotificationManager)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;
    }

    private void Start()
    {
        if (Singleton != this)
        {
            return; // so things don't get even more broken if this is a duplicate >:(
        }

        if (NetworkManager.Singleton == null)
        {
            // Can't listen to something that doesn't exist >:(
            throw new Exception($"There is no {nameof(NetworkManager)} for the {nameof(ConnectionNotificationManager)} to do stuff with! " +
                $"Please add a {nameof(NetworkManager)} to the scene.");
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        //NetworkManager.Singleton.OnConnectionEvent += OnConnectionEventCallback;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

    }

    private void OnDestroy()
    {
        // Since the NetworkManager can potentially be destroyed before this component, only
        // remove the subscriptions if that singleton still exists.
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("ApprovalCheck: id:" + request.ClientNetworkId + " count:" + NetworkManager.Singleton.ConnectedClients.Count);
        // Verificar si ya hemos alcanzado el límite
        if (NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            response.Approved = false;
            response.Reason = "El servidor está lleno";
            return;
        }

        //request.ClientNetworkId 

        // Additional connection data defined by user code
        byte[] connectionData = request.Payload;
        string sData = System.Text.Encoding.ASCII.GetString(connectionData);

        Debug.Log("connectionData:" + sData);
        PlayerData pd = JsonUtility.FromJson<PlayerData>(sData);
        DictPlayerUI.Add(request.ClientNetworkId, pd);
        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        // response.PlayerPrefabHash = playerPrefabs[pd.playerPrefab].GetComponent<NetworkObject>().PrefabIdHash;
        //response.PlayerPrefabHash = null;
        response.PlayerPrefabHash = playerPrefabs[request.ClientNetworkId % 2].GetComponent<NetworkObject>().PrefabIdHash;
        // Position to spawn the player object (if null it uses default of Vector3.zero)
        //response.Position = Vector3.zero;
        response.Position = new Vector3(request.ClientNetworkId * 15, 0, 0);
        //response.Position = new Vector3(0, 0, request.ClientNetworkId * 50);

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        //response.Rotation = Quaternion.identity;
    }

    // Descoonectar al jugador en este evento hace que aparaezca por un segundo
    // Es mejor hacerlo en ApprovalCheck para prevenir que se pueda conectar
    /* private void OnConnectionEventCallback(NetworkManager manager,ConnectionEventData data)
    {
        if (!manager.IsServer) return;

       if (manager.ConnectedClients.Count > 2)
        {
            Debug.Log("cliente desconectado por superar el limite");
            manager.DisconnectClient(data.ClientId);
        }

    }*/

    private void OnClientConnectedCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Connected);

        var totalClients = NetworkManager.Singleton.ConnectedClients.Count;
        Debug.Log("OnClientConnectedCallback: id " + clientId + " count:" + totalClients);

        var o = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject; // accedemos al prefab del player

        if (NetworkManager.Singleton.IsServer)// SI SOMOS SERVER 
        {
            // g.GetComponentInChildren<TMP_Text>().text = DictPlayerUI[clientId].playerName;
            // o.GetComponentInChildren<PlayerDataNET>().playerName.Value = DictPlayerUI[clientId].playerName; //GUARDAMOS EL NOMBRE DEL PLAYER
            // ActualizarUIConectadosServ();
        }

    }




    private void OnClientDisconnectCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Disconnected);

        Debug.Log("OnClientDisconnectCallback:" + clientId);

        /// Borramos el cliente de la UI
        if (DictPlayerUI.ContainsKey(clientId))
        {
            var uiPanel = DictPlayerUI[clientId].uiPanel;
            if (uiPanel != null)
            {
                Destroy(uiPanel);
            }
        }

    }

    void DisconnectPlayer(NetworkObject player)
    {
        Debug.Log("DisconnectPlayer:" + player.OwnerClientId);
        // Note: If a client invokes this method, it will throw an exception.
        NetworkManager.Singleton.DisconnectClient(player.OwnerClientId);
    }
}
