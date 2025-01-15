using Unity.Netcode;
//using System.Collections.Generic;
using UnityEngine;

namespace TagGame { 
    public class TeamData : NetworkBehaviour
    {
        public int Id { get; private set; }
        public Color Color;

        public NetworkVariable<int> PlayerCount = new NetworkVariable<int>();

        private void Awake()
        {
            Id = name.GetHashCode();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //enabled = IsClient;

            Debug.Log(this.name + ", ID: " + this.Id + ", has spawned into the network");
        }

        [Rpc(SendTo.Server)]
        public void AddPlayerRpc(ulong playerId)
        {
            PlayerCount.Value++;
        }
        [Rpc(SendTo.Server)]
        public void RemovePlayerRpc(ulong playerId)
        {
            PlayerCount.Value--;
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
