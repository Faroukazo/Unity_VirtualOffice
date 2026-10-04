using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class EnviarDatosBBDD // Cambiado a static
{
    public static void EnviarDatosBaseDatos(MonoBehaviour caller, string nombreUsuario, string nombreDestinatario, string asunto, string url, string nombreOriginal, string nombreservidor)
    {
        caller.StartCoroutine(EnviarAlServidor(nombreUsuario, nombreDestinatario, asunto, url,nombreOriginal,nombreservidor));
    }

    private static IEnumerator EnviarAlServidor(string nombreUsuario, string nombreDestinatario, string asunto, string url, string nombreOriginal, string nombreservidor)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */); // Token secreto
        form.AddField("nombreUsuario", nombreUsuario);
        form.AddField("nombreDestinatario", nombreDestinatario);
        form.AddField("asunto", asunto);
        form.AddField("url", url);
        form.AddField("nombreOriginal", nombreOriginal);
        form.AddField("nombreservidor", nombreservidor);

        UnityWebRequest www = UnityWebRequest.Post("https://www.oficinavirtuales.com/uploadNotification.php", form);
        yield return www.SendWebRequest();
        RespuestaServidor(www);
    }

    private static void RespuestaServidor(UnityWebRequest www)
    {
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Datos enviados correctamente: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al enviar datos: " + www.error);
        }
    }
}
