using Unity.Netcode;

using UnityEngine;
namespace TagGame
{
    public class ClaimableArea : NetworkBehaviour
    {
        public NetworkVariable<Color> NetworkColor = new NetworkVariable<Color>(Color.white);
        public NetworkVariable<int> NetworkCurrentTeamId = new NetworkVariable<int>();

        private Material _cubeMaterial;
        private Material _childMaterial;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkColor.OnValueChanged += OnColorChanged;
            NetworkCurrentTeamId.OnValueChanged += OnTeamChanged;

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
            NetworkCurrentTeamId.OnValueChanged -= OnTeamChanged;
        }
        private void OnColorChanged(Color oldColor, Color newColor)
        {
            UpdateMaterialColor(newColor);
        }

        private void OnTeamChanged(int oldTeam, int newTeam)
        {
            Debug.Log("Team changed from old ID: " + oldTeam + ", to new ID: " + newTeam);
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
                    if (team.Id != NetworkCurrentTeamId.Value)
                    {
                        ChangeTeamServerRpc(team.Id, team.Color);
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
        private void ChangeTeamServerRpc(int teamId, Color newColor)
        {
            NetworkCurrentTeamId.Value = teamId;
            NetworkColor.Value = newColor;
        }
    }
}