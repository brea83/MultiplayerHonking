using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class PlayerNetworkHealth : NetworkBehaviour
{
    private NetworkVariable<float> _healthVar = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public UnityAction<float> OnHealthChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _healthVar.OnValueChanged += OnHealthValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        _healthVar.OnValueChanged -= OnHealthValueChanged;
    }

    void OnDebugTest()
    {
        _healthVar.Value = 42;
    }

    private void OnHealthValueChanged(float oldHealth, float newHealth)
    {
        OnHealthChanged?.Invoke(newHealth);
    }

    public float GetHealth()
    {
        return _healthVar.Value;
    }
}
