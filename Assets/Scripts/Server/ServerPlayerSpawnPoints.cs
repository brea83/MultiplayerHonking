using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerSpawnPoints : MonoBehaviour
{
    public static ServerPlayerSpawnPoints Instance;
    [SerializeField] private List<GameObject> _spawnPoints;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("two player spawn point collections awoke. destroying the second one");
            Destroy(this);
        }
    }
    public GameObject GetRandomSpawnPoint()
    {
        if(_spawnPoints.Count == 0) return null;
        return _spawnPoints[Random.Range(0,_spawnPoints.Count)];
    }
}
