using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEditor.PackageManager;
namespace TagGame
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        [SerializeField]
        public List<TeamData> Teams = new List<TeamData>();
        public TeamData Team1 {  get; private set; }
        public TeamData Team2 { get; private set; }
        //public NetworkList<int> TeamIds = new NetworkList<int>();
        //public List<PlayerStats> Players {get; private set;}
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
    

        }
        public TeamData LogInToTeam(ulong clientId)
        {
            bool isEven = clientId % 2 == 0;
            if(isEven)
            {
                return Team1;
            }
            else
            {
                return Team2;
            }
        }
        /*
        private void OnClientConnect(ulong clientId)
        {
            if (!IsLocalClient(clientId))
            {
                return;
            }
            NetworkManager networkManager = NetworkManager.Singleton;
            

            NetworkObject localPlayer = networkManager.SpawnManager.GetLocalPlayerObject();
            PlayerStats player = localPlayer.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.ChangeTeamRpc(LogInToTeam().Id);
            }

        }
        */
        private bool IsLocalClient(ulong clientId)
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            return networkManager.LocalClientId == clientId;
        }

    }
}