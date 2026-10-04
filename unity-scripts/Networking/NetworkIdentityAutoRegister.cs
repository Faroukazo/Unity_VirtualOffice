using UnityEngine;
using Mirror;

[DisallowMultipleComponent]
public class NetworkIdentityAutoRegister : MonoBehaviour
{
    void Awake()
    {
        if (TryGetComponent(out NetworkIdentity netId))
        {
            if (NetworkIdentityRegistry.instancia != null)
            {
                NetworkIdentityRegistry.instancia.Registrar(netId);
            }
        }
    }
}
