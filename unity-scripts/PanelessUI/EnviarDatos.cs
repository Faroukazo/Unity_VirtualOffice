using UnityEngine;
using TMPro;
using System.Collections;  
using UnityEngine.Networking;// Necesario para hacer solicitudes HTTP

public class EnviarDatosForm : MonoBehaviour
{
    [Header("Campos de entrada")]
    public TMP_InputField nombreInput;
    public TMP_InputField asuntoInput;
    public TMP_InputField urlInput;  

    [Header("Mensaje de error")]
    public TextMeshProUGUI mensajeError;

    public void EnviarDatos()
    {
        string nombre = nombreInput.text.Trim();
        string asunto = asuntoInput.text.Trim();
        string filePath = urlInput.text.Trim(); 

        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(asunto) || string.IsNullOrEmpty(filePath))
        {
            MostrarError("Todos los campos son obligatorios.");
            return;
        }

        Debug.Log("Nombre: " + nombre);
        Debug.Log("Asunto: " + asunto);
        Debug.Log("Ruta del archivo: " + filePath);

        StartCoroutine(ComprobarExistenciaDestinatario(nombre, asunto, filePath));
    }

    private IEnumerator ComprobarExistenciaDestinatario(string nombre, string asunto, string filePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("nombreUsuario", nombre);
        form.AddField("token", Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */);

        UnityWebRequest www = UnityWebRequest.Post("https://www.oficinavirtuales.com/checkUser.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al comprobar el usuario: " + www.error);
            MostrarError("Error al verificar el usuario.");
            yield break;
        }

        if (www.downloadHandler.text.Contains("El usuario no existe"))
        {
            MostrarError("El usuario no existe en la base de datos.");
            yield break;
        }

        StartCoroutine(SubirArchivoConProgreso(filePath, nombre, asunto));
    }

    private IEnumerator SubirArchivoConProgreso(string filePath, string nombre, string asunto)
    {
        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
        {
            MostrarError("El archivo no existe en la ruta especificada.");
            yield break;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        string fileName = System.IO.Path.GetFileName(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName);
        form.AddField("nombre", nombre);
        form.AddField("asunto", asunto);
        string token = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;
        form.AddField("token", token);

        UnityWebRequest www = UnityWebRequest.Post("https://www.oficinavirtuales.com/uploader.php", form);
        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            Debug.Log("Progreso de subida: " + (www.uploadProgress * 100) + "%");
            yield return null;
        }

        string nombreUsuario = PlayerPrefs.GetString("username");
        ComprobarDatosSubidos(nombre, nombreUsuario, asunto, www);
    }

    private void ComprobarDatosSubidos(string nombre, string nombreUsuario, string asunto, UnityWebRequest www)
    {
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Archivo subido correctamente.");

            // Deserializar la respuesta JSON
            string json = www.downloadHandler.text;
            RespuestaServidor respuesta = JsonUtility.FromJson<RespuestaServidor>(json);            
            Debug.Log("Nombre original: " + respuesta.nombreOriginal);
            Debug.Log("Nombre del servidor: " + respuesta.nombreServidor);
            Debug.Log("URL: " + respuesta.url);

            EnviarDatosBBDD.EnviarDatosBaseDatos(this, nombreUsuario, nombre, asunto, respuesta.url,respuesta.nombreOriginal,respuesta.nombreServidor);
            Debug.Log("Archivo subido. URL: " + respuesta.url);
            MostrarExito("Archivo subido correctamente.");
            BorrarCamposConDelay();
        }
        else
        {
            Debug.LogError("Error al subir archivo: " + www.error);
            MostrarError("Error al subir el archivo.");
        }
    }

    private IEnumerator BorrarCamposConDelay()
    {
        yield return new WaitForSeconds(2f);
        nombreInput.text = "";
        asuntoInput.text = "";
        urlInput.text = "";
        OcultarError();
    }

    private void MostrarError(string mensaje)
    {
        mensajeError.text = mensaje;
        mensajeError.color = Color.red;
        mensajeError.gameObject.SetActive(true);
    }

    private void MostrarExito(string mensaje)
    {
        mensajeError.text = mensaje;
        mensajeError.color = Color.green;
        mensajeError.gameObject.SetActive(true);
    }

    private void OcultarError()
    {
        mensajeError.gameObject.SetActive(false);
    }
}
