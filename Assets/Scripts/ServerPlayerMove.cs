using Unity.Netcode;
using UnityEngine;
namespace NetcodeDemo
{
    [DefaultExecutionOrder(0)]
    public class ServerPlayerMove : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if(!IsServer)
            {
                enabled = false;
                return;
            }
            SpawnPlayer();
            base.OnNetworkSpawn();
        }

        void SpawnPlayer()
        {
            GameObject spawnPoint = ServerPlayerSpawnPoints.Instance.GetRandomSpawnPoint();
            Vector3 spawnPosition = spawnPoint ? spawnPoint.transform.position : Vector3.zero;
            transform.position = spawnPosition;
        }
    }
}