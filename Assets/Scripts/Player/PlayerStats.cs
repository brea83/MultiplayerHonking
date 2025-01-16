using UnityEngine;
using System.Collections.Generic;

using Unity.Netcode;

namespace TagGame
{
    public class PlayerStats : NetworkBehaviour
    {
        [SerializeField]
        public TeamData Team { get; private set; }
        public NetworkVariable<bool> IsTeam1 = new NetworkVariable<bool>();
        public NetworkVariable<bool> IsIt = new NetworkVariable<bool>();
        //public NetworkVariable<string> Name = new NetworkVariable<string>();
        public List<Material> DyableMaterials = new List<Material>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            IsTeam1.OnValueChanged += UpdateTeam;
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
            Debug.Log("gameManager instance is: " + game);
            
            if(game != null && game.Teams.Count > 1)
            {
                TeamData newTeam = game.LogInToTeam( NetworkManager.Singleton.LocalClientId);
                Debug.Log ("found team: " + newTeam.ToString());
                ChangeTeamRpc(newTeam.isTeam1);
            }

        }
        public override void OnNetworkDespawn()
        {
            IsTeam1.OnValueChanged -= UpdateTeam;
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
                    other.ChangeTeamRpc( this.Team.isTeam1);
                    
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
        public void ChangeTeamRpc(bool isTeam1)
        {
            
            IsTeam1.Value = isTeam1;
        }
        public void UpdateTeam(bool oldTeamIsTeam1, bool newTeamIsTeam1)
        {
            GameManager manager = GameManager.Instance;
            TeamData oldTeam = null;
            if (newTeamIsTeam1)
            {
                Team = manager.Team1;
                oldTeam = manager.Team2;
            }
            else
            {
                Team = manager.Team2;
                oldTeam = manager.Team1;
            }
            Debug.Log(name + "'s Team changed from old ID: " + oldTeam + ", to new ID: " + Team);
            /*
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