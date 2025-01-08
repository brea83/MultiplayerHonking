using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using Unity.Netcode;
namespace NetcodeDemo
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField] CharacterController _characterController;
        [SerializeField] ThirdPersonController _thirdPersonController;
        [SerializeField] PlayerInput _playerInput;

        //[SerializeField] Transform _cameraFollow;

        private void Awake()
        {
            _playerInput.enabled = false;
            _thirdPersonController.enabled = false;
            _characterController.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            enabled = IsClient;

            if (!IsOwner)
            {
                //if not the owner disable
                enabled = false;
                _playerInput.enabled = false;
                _thirdPersonController.enabled = false;
                _characterController.enabled = false;
                return;
            }

            //otherwise it is the owner and do enable
            _playerInput.enabled = true;
            _thirdPersonController.enabled = true;
            _characterController.enabled = true;
        }
    }
}