using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace TagGame
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        
        public List<TeamData> Teams = new List<TeamData>();
        [SerializeField]
        private List<ClaimableArea> claimableAreas = new List<ClaimableArea>();
        public TeamData Team1 {  get; private set; }
        public TeamData Team2 { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            //Teams = new List<TeamData>();
            TeamData[] children = GetComponentsInChildren<TeamData>();
            foreach (TeamData child in children)
            {
                Teams.Add(child);
                if (child.isTeam1)
                {
                    Team1 = child;
                }
                else
                {
                    Team2 = child;
                }
                Debug.Log("adding " + child.name + ", to local list");
            }


        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkManager network = NetworkManager.Singleton;
            network.OnClientConnectedCallback += OnClientConnect;
            network.OnClientDisconnectCallback += OnClientDisconnect;

        }
        
        public TeamData AssignTeam(ulong clientId)
        {
            bool isEven = clientId % 2 == 0;
            if(isEven)
            {
                Debug.Log("local clientId was even");
                return Team1;
            }
            else
            {
                Debug.Log("Local clientId was odd");
                return Team2;
            }
        }
        
        private void OnClientConnect(ulong clientId)
        {
            if (!IsLocalClient(clientId))
            {
                Debug.Log(clientId + ", is not local client, game manager onClientConnect Not performed");
                return;
            }
            Debug.Log(clientId + ", is local client, doing game manager onClientConnect");
            NetworkManager network = NetworkManager.Singleton;
            
            NetworkObject localPlayer = network.SpawnManager.GetLocalPlayerObject();
            PlayerStats player = localPlayer.GetComponent<PlayerStats>();
            if (player != null)
            {
                Debug.Log("local player network object found");
                TeamData team = AssignTeam(clientId);
                player.InitializeTeam(team.isTeam1);
            }
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            NetworkManager network = NetworkManager.Singleton;

            NetworkObject disconnectedPlayer = network.SpawnManager.GetPlayerNetworkObject(clientId);
            PlayerStats player = disconnectedPlayer.GetComponent<PlayerStats>();
            if (player != null)
            {
                TeamData team = AssignTeam(clientId);
                team.RemovePlayerRpc(clientId);
            }
        }

        private bool IsLocalClient(ulong clientId)
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            return networkManager.LocalClientId == clientId;
        }

    }
}