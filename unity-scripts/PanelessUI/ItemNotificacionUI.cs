using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotificacionUI : MonoBehaviour
{
    [Header("Componentes de UI Notificaciones")]
    public TMP_Text nombreText;
    public TMP_Text asuntoText;
    public Button botonDescargar;

    private string tokenSecreto = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;

    public void Inicializar(string nombre, string asunto, string archivoUrl)
    {
        nombreText.text = nombre;
        asuntoText.text = asunto;
        Debug.Log("Archivo URL: " + archivoUrl);

        // Extraer solo el nombre del archivo de la URL
        string nombreArchivo = archivoUrl.Substring(archivoUrl.LastIndexOf("/") + 1);
        Debug.Log("Nombre del archivo: " + nombreArchivo);

        string tokenCodificado = System.Uri.EscapeDataString(tokenSecreto);
        string urlDescarga = "https://www.oficinavirtuales.com/downloadDocs.php?file=" + System.Uri.EscapeDataString(nombreArchivo) + "&token=" + tokenCodificado;

        botonDescargar.onClick.AddListener(() => Application.OpenURL(urlDescarga));
    }
}
