using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
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
        if(!IsLocalClient(clientId))
        {
            return;
        }

        _healthLabel.text = "Disconnected";
        _serverId.text = "Disconnected";
        _clientId.text = "Disconnected";
        _clientStart.Flex(true);
        _hostStart.Flex(true);
        _serverStart.Flex(true);
        _disconnect.Flex(false);
    }

    private void OnDestroy()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager != null) 
        {
            networkManager.OnClientConnectedCallback -= OnClientConnect;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    void OnClientConnect(ulong clientId)
    {

        if (!IsLocalClient(clientId))
        {
            return;
        }
        NetworkManager networkManager = NetworkManager.Singleton;
        NetworkObject playerObject = networkManager.SpawnManager.GetLocalPlayerObject(); 
            
        _clientId.text = clientId.ToString();
        _serverId.text = networkManager.ConnectedHostname;

        SetupPlayerHealth(playerObject);
        _clientStart.Flex(false);
        _hostStart.Flex(false);
        _serverStart.Flex(false);
        _disconnect.Flex(true);
    }
        
    private PlayerNetworkHealth PlayerHealthSubUnSub(NetworkObject playerObject, bool subscribe) 
    {
        PlayerNetworkHealth networkHealth = localPlayer.GetComponent<PlayerNetworkHealth>();
        if (networkHealth != null)
        {
            if (subscribe)
            {
                networkHealth.OnHealthChanged += OnPlayerHealthChanged;
            }
            else
            {
                networkHealth.OnHealthChanged -= OnPlayerHealthChanged;
            }
        }
        return networkHealth;
    }

    private bool IsLocalClient(ulong clientId)
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        return networkManager.LocalClientId == clientId;
    }
    private void SetupPlayerHealth(NetworkObject localPlayer)
    {
        
        PlayerNetworkHealth networkHealth = PlayerHealthSubUnSub(localPlayer, true);
        if (networkHealth != null)
        {
            _healthLabel.text = networkHealth.GetHealth().ToString();
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
