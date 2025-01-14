using UnityEngine;
using System.Collections.Generic;

using Unity.Netcode;

namespace TagGame
{
    public class PlayerStats : NetworkBehaviour
    {
        [SerializeField]
        public TeamData Team { get; private set; }
        public NetworkVariable<int> NetworkTeamId = new NetworkVariable<int>();
        public NetworkVariable<bool> IsIt = new NetworkVariable<bool>();
        //public NetworkVariable<string> Name = new NetworkVariable<string>();
        public List<Material> DyableMaterials = new List<Material>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkTeamId.OnValueChanged += UpdateTeam;
            if (!IsOwner)
            {
                Debug.Log("Player OnNetworkSpawn that wasn't Owner, network instance ID; " + NetworkObject.GetInstanceID());
                return;
            }
            Debug.Log(this.name + ", has spawned into the network");
            
        }

        public void Start()
        {
            //only use this for rpc calls to things that were placed in scene when THIS script is spawned in dynamically
            //Debug.Log("Looking for game manager");
            if(!IsOwner)
            {
                return;
            }
            GameManager game = GameManager.Instance;
            
            if(game != null && game.Teams.Count > 1)
            {
                TeamData newTeam = game.LogInToTeam();
                Debug.Log ("found team: " + newTeam.ToString());
                /*
                if(newTeam.PlayerCount.Value == 0)
                {
                    SetItStatusRpc(true);
                }
                else
                {
                    SetItStatusRpc(false);
                }
                */
                ChangeTeamRpc(newTeam.Id);
            }

        }
        public override void OnNetworkDespawn()
        {
            NetworkTeamId.OnValueChanged -= UpdateTeam;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if(IsIt.Value)
            {
                Collider other = collision.collider;
                PlayerStats otherPlayer = other.GetComponent<PlayerStats>();
                if (otherPlayer != null)
                {
                    Tag(otherPlayer);
                }
            }
        }
        public void Tag(PlayerStats other)
        {
            if (!other.IsIt.Value)
            {
                if ( other.Team != this.Team)
                {
                    other.ChangeTeamRpc( this.Team.Id);
                    
                }

                this.SetItStatusRpc(false);
                other.SetItStatusRpc(true);
            }
        }

        [Rpc(SendTo.Server)]
        public void SetNameRpc(string newName)
        {
           // Name.Value = newName;
        }
        [Rpc(SendTo.Server)]
        public void SetItStatusRpc(bool validity)
        {
            IsIt.Value = validity;
        }
        [Rpc(SendTo.Server)]
        public void ChangeTeamRpc(int teamId)
        {
            
            NetworkTeamId.Value = teamId;
        }
        public void UpdateTeam(int oldId, int newId)
        {
            Team = GameManager.Instance.GetTeamById(newId);
            
            /*
            TeamData oldTeam = GameManager.Instance.GetTeamById(oldId);
            ulong playerId = this.NetworkObject.OwnerClientId;
            
            if (oldTeam != null)
            {
                oldTeam.RemovePlayerRpc(playerId);
            }

            Team.AddPlayerRpc(playerId);
            */
            foreach (Material material in DyableMaterials)
            {
                if (material != null)
                {
                    material.SetColor("_BaseColor", Team.Color);
                }
            }
        }

        public void UpdateMainColor(Color color)
        {
            foreach (Material material in DyableMaterials)
            {
                if (material != null)
                {
                    material.SetColor("_BaseColor", Team.Color);
                }
            }
        }
    }

    
}