using UnityEngine;
using Unity.Netcode;
namespace NetcodeDemo
{
    public class ColorTrigger : NetworkBehaviour
    {
        public NetworkVariable<Color> _networkColor = new NetworkVariable<Color>(Color.white);

        private Material _cubeMaterial;
        private Material _childMaterial;

        public override void OnNetworkSpawn()
        {
            _networkColor.OnValueChanged += OnColorChanged;
            MeshRenderer cubeMesh = GetComponent<MeshRenderer>();
            if (cubeMesh != null)
            {
                _cubeMaterial = cubeMesh.material;
            }
            MeshRenderer childMesh = GetComponentInChildren<MeshRenderer>();
            if (childMesh != null) 
            { 
                _childMaterial = childMesh.material;
            } 
         
        }
        public override void OnNetworkDespawn()
        {
            _networkColor.OnValueChanged -= OnColorChanged;
        }
        private void OnColorChanged(Color oldColor, Color newColor) 
        {
            UpdateMaterialColor(newColor);
        }

        private void UpdateMaterialColor(Color newColor)
        {
            if(_cubeMaterial != null)
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
            if(IsClient && networkObject != null && networkObject.IsOwner)
            {
                ChangeColorServerRpc(networkObject.OwnerClientId);
            }
        }

        [Rpc(SendTo.Server)]
        private void ChangeColorServerRpc(ulong playerId)
        {
            // blue for even red for odd

            Color newColor = (playerId % 2 == 0) ? new Color(0, 0, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
            _networkColor.Value = newColor;
        }
    }
}