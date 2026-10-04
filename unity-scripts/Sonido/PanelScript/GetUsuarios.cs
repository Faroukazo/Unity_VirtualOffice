using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class GetUsuarios : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI lobbyText;
    public TextMeshProUGUI oficinaText;
    public TextMeshProUGUI canalPrivadoText;
    
    private string url = "https://www.oficinavirtuales.com/obtenerUsuariosEnSala.php";
    private string tokenSecreto = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;
    public float tiempoRefresco = 2f;

    void Start()
    {
        StartCoroutine(ActualizarPeriodicamente());
    }

    IEnumerator ActualizarPeriodicamente()
    {
        while (true)
        {
            yield return StartCoroutine(GetUsuario());
            yield return new WaitForSeconds(tiempoRefresco);
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (lobbyText != null) lobbyText.text = "";
        if (oficinaText != null) oficinaText.text = "";
        if (canalPrivadoText != null) canalPrivadoText.text = "";
    }
    IEnumerator GetUsuario()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", tokenSecreto);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Respuesta: " + www.downloadHandler.text);
                ProcesarRespuesta(www.downloadHandler.text);
            }
        }
    }

    void ProcesarRespuesta(string json)
    {
        UsuarioRespuesta respuesta = JsonUtility.FromJson<UsuarioRespuesta>(json);

        if (respuesta.status == "success")
        {
            string lobbyTexto = "";
            string oficinaTexto = "";
            string canalPrivadoTexto = "";

            foreach (Usuario u in respuesta.usuarios)
            {
                if (u.sala == "Lobby")
                {
                    lobbyTexto += $"{u.nombre_usuario}\n";
                }
                else if (u.sala == "Oficina")
                {
                    oficinaTexto += $"{u.nombre_usuario}\n";
                }
                else
                {
                    canalPrivadoTexto += $"{u.nombre_usuario} ({u.sala})\n";
                }
            }

            lobbyText.text = lobbyTexto;
            oficinaText.text = oficinaTexto;
            canalPrivadoText.text = canalPrivadoTexto;
        }
        else
        {
            Debug.LogWarning($"Error al recibir usuarios: {respuesta.message}");
        }
    }
}
