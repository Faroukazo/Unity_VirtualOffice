using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartPositionRegistry : MonoBehaviour
{
    public static StartPositionRegistry instancia;

    private Dictionary<string, NetworkStartPosition> posiciones = new();

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
            return;
        }
    }

    public void Registrar(string nombre, NetworkStartPosition pos)
    {
        if (!posiciones.ContainsKey(nombre))
        {
            posiciones.Add(nombre, pos);
        }
    }

    public NetworkStartPosition Obtener(string nombre)
    {
        posiciones.TryGetValue(nombre, out var pos);
        return pos;
    }
}
