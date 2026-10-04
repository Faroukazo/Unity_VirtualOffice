using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkIdentityRegistry : MonoBehaviour
{
    public static NetworkIdentityRegistry instancia;

    private readonly List<NetworkIdentity> registrados = new();

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Registrar(NetworkIdentity netId)
    {
        if (!registrados.Contains(netId))
        {
            registrados.Add(netId);
        }
    }

    public void ActivarTodos()
    {
        foreach (var netId in registrados)
        {
            if (netId != null)
            {
                netId.enabled = true;
            }
        }
    }

    public void Limpiar()
    {
        registrados.Clear();
    }
}
