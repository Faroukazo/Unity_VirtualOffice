using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GestorNotificaciones : MonoBehaviour
{
    private string url = "https://www.oficinavirtuales.com/getNotification.php";
    private string tokenSecreto = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;
    public BotonVerNotificaciones botonVerNotificaciones;

    public GameObject notificacionPrefab;
    public Transform contentPanel;

    void Start()
    {
        InvokeRepeating("ChekearNotificaciones", 2f, 30f);
    }

    void ChekearNotificaciones()
    {
        StartCoroutine(ObtenerDatos());
    }

    IEnumerator ObtenerDatos()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", tokenSecreto);
        form.AddField("username", PlayerPrefs.GetString("username"));

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
                NotificacionData[] notificaciones = JsonHelper.FromJson<NotificacionData>(jsonResponse);
                foreach (Transform child in contentPanel)
                {
                    Destroy(child.gameObject);
                }
                foreach (var notificacion in notificaciones)
                {
                    CrearNotificacion(notificacion.nombre, notificacion.asunto, notificacion.url);
                }
                ChekearNotificaciones(notificaciones);
            }
        }
    }

    private void ChekearNotificaciones(NotificacionData[] notificaciones)
    {
        int noLeidasCount = 0;
        foreach (var noti in notificaciones)
        {
            if (!noti.leido)
            {
                noLeidasCount++;
            }
        }
        botonVerNotificaciones.ActualizarIcono(noLeidasCount > 0);
    }

    void CrearNotificacion(string nombre, string asunto, string url)
    {
        GameObject nuevaNotificacion = Instantiate(notificacionPrefab, contentPanel);
        ItemNotificacionUI itemUI = nuevaNotificacion.GetComponent<ItemNotificacionUI>();

        if (itemUI != null)
        {
            itemUI.Inicializar(nombre, asunto, url);
        }
        else
        {
            Debug.LogError("No se encontró el componente ItemNotificacionUI en el prefab.");
        }
    }
}
