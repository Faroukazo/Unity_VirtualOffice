using UnityEngine;
using Mirror;

public class AutoStartServer : MonoBehaviour
{
    //Este script es para que el programa inicie en el servidor automaticamente
    void Start()
    {
        Debug.Log(">>> Iniciando servidor automáticamente...");
        NetworkManager.singleton.StartServer();
    }
}
