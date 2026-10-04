using UnityEngine;
using Mirror;

public class ClientConnectionHandler : MonoBehaviour{

    public GameObject networkObject;
   
    private void Start()
    {
        ComprobarComponente();
        IniciarConexionServidor();
    }

    private static void IniciarConexionServidor()
    {
        if (NetworkManager.singleton != null)
        {
            Debug.Log("Iniciando la conexión al servidor...");
            NetworkManager.singleton.StartClient();  // Iniciar la conexión del cliente al servidor
        }
        else
        {
            Debug.LogError("NetworkManager es nulo!");
        }
    }

    private void ComprobarComponente()
    {
        // Asegurarse de que el GameObject 'Network' esté asignado
        if (networkObject != null)
        {
            // Desactivar solo el NetworkHUD (sin afectar el NetworkManager)
            GameObject networkHUD = networkObject.transform.Find("NetworkHUD")?.gameObject;
            if (networkHUD != null)
            {
                networkHUD.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("El GameObject 'Network' no está asignado.");
        }
    }
}
