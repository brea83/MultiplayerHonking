using Unity.Netcode;

using UnityEngine;
namespace TagGame
{
    public class ClaimableArea : NetworkBehaviour
    {
        public NetworkVariable<Color> NetworkColor = new NetworkVariable<Color>(Color.white);
        public NetworkVariable<bool> NetworkIsCurrentTeam1 = new NetworkVariable<bool>();

        private Material _cubeMaterial;
        private Material _childMaterial;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkColor.OnValueChanged += OnColorChanged;
            NetworkIsCurrentTeam1.OnValueChanged += OnTeamChanged;

            MeshRenderer cubeMesh = GetComponent<MeshRenderer>();
            if (cubeMesh != null)
            {
                _cubeMaterial = cubeMesh.material;
            }
            MeshRenderer[] childMeshes = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer child in childMeshes)
            {
                if (child != cubeMesh)
                {
                    _childMaterial = child.material;
                }
            }
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

        private void OnTeamChanged(bool oldTeam, bool newTeam)
        {
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
                    if (team.isTeam1 != NetworkIsCurrentTeam1.Value)
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

            Color newColor = (playerId % 2 == 0) ? new Color(0, 0, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
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