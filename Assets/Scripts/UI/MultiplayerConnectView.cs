using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Netcode;
using System;
public class MultiplayerConnectView : MonoBehaviour
{
    VisualElement _root;
    Button _clientStart;
    Button _hostStart;
    Button _serverStart;
    Button _disconnect;
    
    Label _healthLabel;
    Label _serverId;
    Label _clientId;
    ulong localClientId;

    GameObject localPlayer;
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        _root = document.rootVisualElement;
        _clientStart = _root.Q<Button>("StartClient");
        _hostStart = _root.Q<Button>("StartHost");
        _serverStart = _root.Q<Button>("StartServer");
        _disconnect = _root.Q<Button>("Disconnect");

        _healthLabel = _root.Q<Label>("PlayerHealth");
        _serverId = _root.Q<Label>("ServerId");
        _clientId = _root.Q<Label>("ClientId");

        NetworkManager networkManager = NetworkManager.Singleton;
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        SetupHost(networkManager);
        SetupClient(networkManager);
        SetupDisconnect(networkManager);
        SetupServer(networkManager);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        PlayerNetworkHealth networkHealth = localPlayer.GetComponent<PlayerNetworkHealth>();
        if (networkHealth != null)
        {
            networkHealth.OnHealthChanged -= OnPlayerHealthChanged;
        }

        _serverId.text = "Disconnected";
        _clientId.text = "Disconnected";
    }

    private void OnDestroy()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        networkManager.OnClientConnectedCallback -= OnClientConnect;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        _clientStart.style.display = DisplayStyle.Flex;
        _hostStart.style.display = DisplayStyle.Flex;
        _serverStart.style.display = DisplayStyle.Flex;
        _disconnect.style.display = DisplayStyle.None;
    }
    void OnClientConnect(ulong clientID)
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        IReadOnlyDictionary<ulong, NetworkClient> clients = networkManager.ConnectedClients;
        NetworkClient client = clients[clientID];

        if (client.PlayerObject.IsLocalPlayer)
        {
            localClientId = client.PlayerObject.OwnerClientId;
            _clientId.text = localClientId.ToString();
            _serverId.text = networkManager.ConnectedHostname;

            SetupPlayerHealth(client);
            _clientStart.style.display = DisplayStyle.None;
            _hostStart.style.display = DisplayStyle.None;
            _serverStart.style.display = DisplayStyle.None;
            _disconnect.style.display = DisplayStyle.Flex;
        }
    }
    private void SetupPlayerHealth(NetworkClient client)
    {
        localPlayer = client.PlayerObject.gameObject;
        PlayerNetworkHealth networkHealth = localPlayer.GetComponent<PlayerNetworkHealth>();
        if (networkHealth != null)
        {
            _healthLabel.text = networkHealth.GetHealth().ToString();
            networkHealth.OnHealthChanged += OnPlayerHealthChanged;
        }
    }
    private void SetupClient(NetworkManager networkManager)
    {
        _clientStart.clicked += () => networkManager.StartClient();
    }
    private void SetupHost(NetworkManager networkManager)
    {

        _hostStart.clicked += () => networkManager.StartHost();
        _serverStart.clicked += () => networkManager.StartServer();
    }

    private void SetupServer(NetworkManager networkManager)
    {
        _serverStart.clicked += () => networkManager.StartServer();
    }
    private void SetupDisconnect(NetworkManager networkManager) 
    {
        _disconnect.clicked += () => networkManager.Shutdown();
    }
    private void OnPlayerHealthChanged(float newHealthValue)
    {
        _healthLabel.text = newHealthValue.ToString();
    }
}
