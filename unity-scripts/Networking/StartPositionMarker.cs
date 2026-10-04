using UnityEngine;
using Mirror;

public class StartPositionMarker : MonoBehaviour
{
    public string nombre;

    void Awake()
    {
        NetworkStartPosition pos = GetComponent<NetworkStartPosition>();
        if (pos != null)
        {
            if (StartPositionRegistry.instancia != null)
            {
                StartPositionRegistry.instancia.Registrar(nombre, pos);
            }
        }
    }
}
