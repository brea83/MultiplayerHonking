using UnityEngine;
using Unity.Netcode;
using System.Collections;
public class PlayerHonk : NetworkBehaviour
{
    [SerializeField]
    private AudioClip _honk;
    [Range(0, 1)]
    public float HonkVolume = 0.5f;
    [SerializeField]
    private float _honkTextDisplayTime = 1f;
    [SerializeField]
    private GameObject _honkText;

    private void Awake()
    {
        _honkText.SetActive(false);
    }
    private void OnHonk()
    {
        if(_honk != null)
        {
            PlayHonkRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayHonkRpc ()
    {
        PlayHonk(OwnerClientId);
    }

    private void PlayHonk(ulong clientId)
    {
        Debug.Log("player, "+ clientId + ", used honk button");
        NetworkManager networkManager = NetworkManager.Singleton;
        
        NetworkObject sourceObject = networkManager.SpawnManager.GetPlayerNetworkObject(clientId);
        sourceObject.GetComponent<PlayerHonk>().ShowHonkText();

        AudioSource.PlayClipAtPoint(_honk, transform.TransformPoint(sourceObject.transform.position), HonkVolume);

    }
    public void ShowHonkText()
    {
        StopAllCoroutines();
        StartCoroutine(HonkTextCoroutine());
    }
    public IEnumerator HonkTextCoroutine()
    {
        _honkText.SetActive(true);
        yield return new WaitForSeconds(_honkTextDisplayTime);

        _honkText.SetActive(false);
    }

    private void Update()
    {
        _honkText.transform.rotation = Camera.main.transform.rotation;
    }
}
