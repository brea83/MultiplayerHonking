using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEditor;
using TagGame;
using UnityEngine.Events;

public class MultiplayerConnectView : MonoBehaviour
{
    VisualElement _root;
    Button _clientStart;
    Button _hostStart;
    Button _serverStart;
    Button _disconnect;
    Button _quit;
    
    Label _healthLabel;
    Label _points;
    Label _clientId;
   
    PlayerStats _playerStats;

    GameObject localPlayer;
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        _root = document.rootVisualElement;
        _clientStart = _root.Q<Button>("StartClient");
        _hostStart = _root.Q<Button>("StartHost");
        _serverStart = _root.Q<Button>("StartServer");
        _disconnect = _root.Q<Button>("Disconnect");
        _quit = _root.Q<Button>("Quit");

        _healthLabel = _root.Q<Label>("PlayerHealth");
        _points = _root.Q<Label>("Points");
        _clientId = _root.Q<Label>("ClientId");

        NetworkManager networkManager = NetworkManager.Singleton;
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        SetupHost(networkManager);
        SetupClient(networkManager);
        SetupDisconnect(networkManager);
        SetupServer(networkManager);
        SetupQuit(networkManager);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(!IsLocalClient(clientId))
        {
            return;
        }

        _healthLabel.text = "Disconnected";
        
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

        // NetworkObject playerObject = WaitToGetPlayerObject(networkManager, 3);
        NetworkObject playerObject = networkManager.SpawnManager.GetLocalPlayerObject();
        _clientId.text = clientId.ToString();
        
        _playerStats = playerObject.GetComponent<PlayerStats>();
        _playerStats.OnTeamChanged += OnPlayerTeamChange;

        _points.text = "Currently Disabled";

        SetupPlayerHealth(playerObject);
        _clientStart.Flex(false);
        _hostStart.Flex(false);
        _serverStart.Flex(false);
        _disconnect.Flex(true);
    }
    private void OnPlayerTeamChange(TeamData oldTeam, TeamData newTeam)
    {
        Debug.Log("Local ui has registered the local player's team change from " + oldTeam + " to " + newTeam);
        /*
         *      turning off points read out until I read more about why this is sending so many events and how I can properly prevent adding too many points
        if(oldTeam != null)
        {
            oldTeam.Points.OnValueChanged -= UpdatePoints;
        }
        newTeam.Points.OnValueChanged += UpdatePoints;
        */
    }
    private void UpdatePoints(int oldPoints, int newPoints)
    {
        _points.text = newPoints.ToString();
    }
    private PlayerNetworkHealth PlayerHealthSubUnSub(NetworkObject playerObject, bool subscribe) 
    {
        PlayerNetworkHealth networkHealth = playerObject.GetComponent<PlayerNetworkHealth>();
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
    private void SetupQuit(NetworkManager networkManager)
    {
        _quit.clicked += () => DoShutdown(networkManager);
    }

    private void DoShutdown(NetworkManager networkManager)
    {
        networkManager.Shutdown();
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif

    }
    private void OnPlayerHealthChanged(float newHealthValue)
    {
        _healthLabel.text = newHealthValue.ToString();
    }

}
