using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GestorClientes : MonoBehaviour
{
    private string url = "https://www.oficinavirtuales.com/userName.php";
    private string tokenSecreto = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;

    [Header("Prefab para escribir msg")]
    public GameObject notificacionPrefab;
    [Header("Content del ScrollView")]
    public Transform contentPanel;

    void Start()
    {
        InvokeRepeating("ChekearClienes", 2f, 30f);
    }
    
    void ChekearClienes()
    {
        StartCoroutine(ObtenerDatos());
    }
    IEnumerator ObtenerDatos()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", tokenSecreto);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error en la conexión: " + www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Respuesta JSON: " + jsonResponse);
                NotificationDataCliente[] notificaciones = JsonHelper.FromJson<NotificationDataCliente>(jsonResponse);
                foreach (Transform child in contentPanel)
                {
                    Destroy(child.gameObject);
                }
                foreach (var notificacion in notificaciones)
                {
                    CrearNotificacion(notificacion.usuario, notificacion.nombre);
                }
            }
        }
    }
    void CrearNotificacion(string usuario, string nombre)
    {
        GameObject nuevaNotificacion = Instantiate(notificacionPrefab, contentPanel);
        ItemNotificationCliente itemUI = nuevaNotificacion.GetComponent<ItemNotificationCliente>();

        if (itemUI != null)
        {
            itemUI.Inicializar(usuario, nombre);
        }
        else
        {
            Debug.LogError("No se encontró el componente ItemNotificacionCliente en el prefab.");
        }
    }
}
