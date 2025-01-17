using UnityEngine;
//using System.Collections.Generic;
using Unity.Netcode;

namespace TagGame
{
    public class PlayerStats : NetworkBehaviour
    {
        [SerializeField]
        public TeamData Team { get; private set; }
        public NetworkVariable<bool> IsTeam1 = new NetworkVariable<bool>();
        public NetworkVariable<bool> IsIt = new NetworkVariable<bool>();
        [SerializeField]
        private SkinnedMeshRenderer _mesh;
       

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
            //start is played after on network spawn for objects that are spawned in dynamically. the opposite order is true for objects place in the scene
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
        public void SetItStatusRpc(bool validity)
        {
            IsIt.Value = validity;
        }

        [Rpc(SendTo.Server)]
        public void ChangeTeamRpc(bool isTeam1)
        {
            Debug.Log("sending team bool change to server, IsOwner == " + IsOwner.ToString());
            if(isTeam1 == IsTeam1.Value)
            {
                //this should only happen on initialization for players on team 2, because everyon's isTeam1 bool initializes at false
                UpdateTeam(isTeam1, isTeam1);
            }
            IsTeam1.Value = isTeam1;
        }
        public void UpdateTeam(bool oldIsTeam1, bool newIsTeam1)
        {
            GameManager manager = GameManager.Instance;
            TeamData oldTeam = Team;
            if(oldTeam != null)
            {
                //we only want to remove from the old team if there Was a previous team
                oldTeam.RemovePlayerRpc(this.OwnerClientId);
            }
            
            Team = newIsTeam1 ? manager.Team1 : manager.Team2;
            Team.AddPlayerRpc(this.OwnerClientId);

            _mesh.materials = Team.Materials.ToArray();

            Debug.Log(name + "'s Team changed from: " + oldTeam.name + ", to: " + Team.name);
        }
        public void InitializeTeam(bool isStartTeam1)
        {
            GameManager manager = GameManager.Instance;

            if(isStartTeam1 != IsTeam1.Value)
            {
                ChangeTeamRpc(isStartTeam1);
            }
            else
            {
                Team = isStartTeam1 ? manager.Team1 : manager.Team2;
                Team.AddPlayerRpc(this.OwnerClientId);
            }

            //Material[] newMaterials = Team.Materials.ToArray();
            _mesh.materials = Team.Materials.ToArray();
            Debug.Log(name + "'s Team changed from null, to " + Team.name);

        }
    }

    
}