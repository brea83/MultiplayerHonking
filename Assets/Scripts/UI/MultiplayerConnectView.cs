using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using Unity.Netcode;
public class MultiplayerConnectView : MonoBehaviour
{
    VisualElement _root;
    Button _clientStart;
    Button _hostStart;
    Button _serverStart;
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        _root = document.rootVisualElement;
        _clientStart = _root.Q<Button>("StartClient");
        _hostStart = _root.Q<Button>("StartHost");
        _serverStart = _root.Q<Button>("StartServer");

        NetworkManager networkManager = NetworkManager.Singleton;
        SetupHost(networkManager);
        SetupClient(networkManager);
        
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
}
