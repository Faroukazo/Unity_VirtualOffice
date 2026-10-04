using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BotonVerNotificaciones : MonoBehaviour
{
    public Button button;
    private Image buttonImage;
    public Sprite iconoAlerta;
    public Sprite iconoNormal;

    private bool hayNoLeidas = false;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();
    }

    public void ActualizarIcono(bool tieneNoLeidas)
    {
        hayNoLeidas = tieneNoLeidas;
        if (hayNoLeidas)
        {
            buttonImage.sprite = iconoAlerta;
        }
        else
        {
            buttonImage.sprite = iconoNormal;
        }
    }

    public void OnClickButton()
    {
        if (hayNoLeidas)
        {
            buttonImage.sprite = iconoNormal;
            StartCoroutine(ActualizarDatosBBDD());
            hayNoLeidas = false;
        }
    }   
    private IEnumerator ActualizarDatosBBDD() {
        Debug.Log("Actualizando datos en la base de datos...");
        string nombreUser = PlayerPrefs.GetString("username");
        WWWForm form = new WWWForm();
        form.AddField("token", Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */);
        form.AddField("username", nombreUser);
        using (UnityWebRequest www = UnityWebRequest.Post("https://www.oficinavirtuales.com/updateNotification.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error en la conexión: " + www.error);
            }
            else
            {
                Debug.Log("Notificaciones actualizadas correctamente.");
            }
            Debug.Log("Respuesta del servidor: " + www.downloadHandler.text);
        }
    }      
}
