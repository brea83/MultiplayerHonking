using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
namespace TagGame {
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        [SerializeField]
        public List<TeamData> Teams = new List<TeamData>();
        public NetworkVariable<int> NextTeamIndex = new NetworkVariable<int>();
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
            }
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            

        }
        public TeamData LogInToTeam()
        {
            TeamData nextTeam = Teams[NextTeamIndex.Value];
            IncrementNextTeamIndexRpc();
      
            return nextTeam;
        }

        [Rpc(SendTo.Server)]
        public void IncrementNextTeamIndexRpc()
        {
            NextTeamIndex.Value = (NextTeamIndex.Value + 1) % Teams.Count;
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