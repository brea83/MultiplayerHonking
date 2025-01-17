using Unity.Netcode;

using UnityEngine;
namespace TagGame
{
    public class ClaimableArea : NetworkBehaviour
    {
        public NetworkVariable<Color> NetworkColor = new NetworkVariable<Color>(Color.white);
        public NetworkVariable<bool> NetworkIsCurrentTeam1 = new NetworkVariable<bool>();

        [SerializeField]
        private Material _cubeMaterial;
        [SerializeField]
        private Material _childMaterial;

        private void Awake()
        {
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsHost)
            {
                UpdateMaterialColor(Color.white);
            } 
            else
            {
                UpdateMaterialColor(NetworkColor.Value);
            }

            NetworkColor.OnValueChanged += OnColorChanged;
            NetworkIsCurrentTeam1.OnValueChanged += OnTeamChanged;
           
        }
        public override void OnNetworkDespawn()
        {
            NetworkColor.OnValueChanged -= OnColorChanged;
            NetworkIsCurrentTeam1.OnValueChanged -= OnTeamChanged;
        }
        private void OnColorChanged(Color oldColor, Color newColor)
        {
            UpdateMaterialColor(newColor);
        }

        private void OnTeamChanged(bool oldIsTeam1, bool newIsTeam1)
        {
            GameManager manager = GameManager.Instance;
            TeamData oldTeam = oldIsTeam1 ? manager.Team1 : manager.Team2;
            TeamData newTeam = newIsTeam1 ? manager.Team1 : manager.Team2;
            /*
             * turning off points  until I read more about why this is sending so many events and how I can properly prevent adding too many points
            oldTeam.RemovePointRpc(OwnerClientId);
            newTeam.AddPointRpc(OwnerClientId);
            */
            Debug.Log(name +"'s Team changed from old ID: " + oldTeam + ", to new ID: " + newTeam);
        }
        private void UpdateMaterialColor(Color newColor)
        {
            if (_cubeMaterial != null)
            {
                _cubeMaterial.SetColor("_BaseColor", newColor);
            }

            if (_childMaterial != null)
            {
                _childMaterial.SetColor("_BaseColor", newColor);
                _childMaterial.SetColor("_EmissionColor", newColor);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();
            if (IsClient && networkObject != null && networkObject.IsOwner)
            {
                PlayerStats player = other.GetComponent<PlayerStats>();
                if (player != null)
                {
                    TeamData team = player.Team;
                    if (team.isTeam1 != NetworkIsCurrentTeam1.Value || NetworkColor.Value == Color.white)
                    {
                        ChangeTeamServerRpc(team.isTeam1, team.Color);
                    }
                }
                else
                {
                    ChangeColorServerRpc(networkObject.OwnerClientId);
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void ChangeColorServerRpc(ulong playerId)
        {
            // blue for even red for odd

            Color newColor =  new Color(1, 0, 0, 0.5f);
            NetworkColor.Value = newColor;
        }
        [Rpc(SendTo.Server)]
        private void ChangeTeamServerRpc(bool isTeam1, Color newColor)
        {
            NetworkIsCurrentTeam1.Value = isTeam1;
            NetworkColor.Value = newColor;
        }
    }
}