using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

namespace TagGame { 
    public class TeamData : NetworkBehaviour
    {
        [SerializeField]
        public bool isTeam1 = false;
        public Color Color;
        public List<Material> Materials = new List<Material>();
        public NetworkVariable<int> PlayerCount = new NetworkVariable<int>();
        public NetworkVariable<int> Points = new NetworkVariable<int>();

        private void Awake()
        {
            //Id = name.GetHashCode();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //enabled = IsClient;

            Debug.Log(this.name + ", has spawned into the network");
        }

        [Rpc(SendTo.Server)]
        public void AddPlayerRpc(ulong playerId)
        {
            PlayerCount.Value++;
        }
        [Rpc(SendTo.Server)]
        public void RemovePlayerRpc(ulong playerId)
        {
            if(PlayerCount.Value > 0)
            {
                PlayerCount.Value--;
            }
        }
        [Rpc(SendTo.Server)]
        public void AddPointRpc(ulong playerId)
        {
            Points.Value++;
        }
        [Rpc(SendTo.Server)]
        public void RemovePointRpc(ulong playerId)
        {
            if (Points.Value > 0)
            {
                Points.Value--;
            }
        }
        /*
        public NetworkList<ulong> NetworkPlayersList = new NetworkList<ulong>();
        //public NetworkVariable<List<ulong>> NetworkPlayersList = new NetworkVariable<List<ulong>>();

        [Rpc(SendTo.Server)]
        public void AddPlayerRpc(ulong playerId)
        {
            NetworkPlayersList.Add(playerId);//.Value.Add(playerId);
        }
        [Rpc(SendTo.Server)]
        public void RemovePlayerRpc(ulong playerId)
        {
            NetworkPlayersList.Remove(playerId);//.Value.Remove(playerId);
        }
        */
    }
}
