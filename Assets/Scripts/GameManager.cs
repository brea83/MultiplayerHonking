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
        private List<TeamData> Teams = new List<TeamData>();
        public NetworkList<int> TeamIds = new NetworkList<int>();
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
                //AddTeamIdRpc(child.Id);
                Debug.Log("adding " + child.name + ", Id: " + child.Id + ", to local list");
            }


        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(Teams.Count > 0)
            {
                foreach (TeamData team in Teams)
                {
                    AddTeamIdRpc(team.Id);
                    Debug.Log("adding " + team.name + ", Id: "+ team.Id+", to network list");
                }
            }

        }
        public TeamData LogInToTeam()
        {
            int teamId = TeamIds[0];
            TeamData nextTeam = GetTeamById(teamId);
            IncrementNextTeamIndexRpc(teamId);
      
            return nextTeam;
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

        [Rpc(SendTo.Server)]
        public void IncrementNextTeamIndexRpc(int oldTeamId)
        {
            if(TeamIds.Count > 0)
            {
                TeamIds.Remove(oldTeamId);
                TeamIds.Add(oldTeamId);
                //int newTeamID = TeamIds[0];
            }
        }
        [Rpc(SendTo.Server)]
        public void AddTeamIdRpc(int id)
        {
            if (TeamIds.Contains(id))
            {
                return;
            }
            TeamIds.Add(id);
        }

        public TeamData GetTeamById(int teamId)
        {
            foreach (TeamData team in Teams)
            {
                if(team.Id == teamId) return team;
            }
            return null;
        }

    }
}