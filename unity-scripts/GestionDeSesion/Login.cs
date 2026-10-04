using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Logging : MonoBehaviour
{
    public TMP_InputField usuarioInput;
    public TMP_InputField contrasenhaInput;
    public TextMeshProUGUI logTexto;
    private UIGestionador gestionadorPaneles;

    private string urlLogin = "https://www.oficinavirtuales.com/login.php";

    void Start()
    {
        logTexto.gameObject.SetActive(false);
        gestionadorPaneles = FindObjectOfType<UIGestionador>();
    }

    public void GetDatosLogin()
    {
        string usuario = usuarioInput.text.Trim();
        string contrasenha = contrasenhaInput.text.Trim();

        if (usuario == "" || contrasenha == "")
        {
            GestionarCamposVacios();
            return;
        }

        StartCoroutine(LoginRequest(usuario, contrasenha));
    }

    public class NoSSLValidation : CertificateHandler
    {
        // Esto siempre devuelve 'true', es decir, acepta cualquier certificado.
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // Ignora la validaci?n del certificado
        }
    }

    private string tokenSecreto = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */; 

    IEnumerator LoginRequest(string usuario, string contrasenha)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usuario);
        form.AddField("password", contrasenha);
        form.AddField("token", tokenSecreto); //  Agregamos el token

        UnityWebRequest www = UnityWebRequest.Post(urlLogin, form);
        www.certificateHandler = new NoSSLValidation();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            logTexto.text = "Error de conexión con el servidor";
            logTexto.color = Color.red;
            logTexto.gameObject.SetActive(true);
            Debug.Log("Error: " + www.error);
        }
        else
        {
            GestionarRespuesta(usuario, www);
        }
    }

    private void GestionarRespuesta(string usuario, UnityWebRequest www)
    {
        string json = www.downloadHandler.text;
        Debug.Log("Respuesta del servidor: " + json);

        LoginResponse respuesta = JsonUtility.FromJson<LoginResponse>(json);

        if (respuesta.status == "success")
        {
            logTexto.text = "Inicio de sesión exitoso";
            logTexto.color = Color.green;
            logTexto.gameObject.SetActive(true);

            SceneManager.LoadScene("OfflineScene");
            PlayerPrefs.SetString("username", usuario);
            Debug.Log("Usuario guardado: " + usuario);
        }
        else
        {
            logTexto.text = respuesta.message;
            logTexto.color = Color.red;
            logTexto.gameObject.SetActive(true);
        }
    }

    private void GestionarCamposVacios()
    {
        logTexto.text = "Todos los campos son obligatorios";
        logTexto.color = Color.red;
        logTexto.gameObject.SetActive(true);
        Debug.Log("Faltan campos en el login");
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string status;
        public string message;
    }
}
