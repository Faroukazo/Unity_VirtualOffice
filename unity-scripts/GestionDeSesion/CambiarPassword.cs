using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CambiarPassword : MonoBehaviour
{
    public TMP_InputField contrasenhaInput;
    public TMP_InputField contrasenhaInput2;
    public TextMeshProUGUI logTexto;
    public GameObject panelAMostrar;
    private UIGestionador gestionadorPaneles;

    public static string nombreUsuario;
    private string urlCambiarPassword = "https://www.oficinavirtuales.com/changePassword.php"; 

    void Start()
    {
        logTexto.gameObject.SetActive(false);
        gestionadorPaneles = FindObjectOfType<UIGestionador>();
    }

    public void GetContrasenhas(GameObject panel)
    {
        string contrasenha = contrasenhaInput.text.Trim();
        string contrasenha2 = contrasenhaInput2.text.Trim();

        panelAMostrar = panel;
        comprobarContrasenha(contrasenha, contrasenha2);
    }

    private void comprobarContrasenha(string contra1, string contra2)
    {
        if (string.IsNullOrEmpty(contra1) || string.IsNullOrEmpty(contra2))
        {
            GestionarCamposVacios();
        }
        else if (contra1 == contra2)
        {
            StartCoroutine(EnviarCambioContrasena(contra1));
        }
        else
        {
            logTexto.text = "Las contraseñas no coinciden";
            logTexto.color = Color.red;
            logTexto.gameObject.SetActive(true);
        }
    }
    public void VolverPanelAnterior(GameObject panel)
    {
        gestionadorPaneles.MostrarPanel(panel);
        VaciarCampos();
    }
    public class NoSSLValidation : CertificateHandler
    {
        // Esto siempre devuelve 'true', es decir, acepta cualquier certificado.
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // Ignora la validación del certificado
        }
    }
    private string tokenSecreto = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;
    IEnumerator EnviarCambioContrasena(string nuevaContrasena)
    {
        WWWForm form = new WWWForm();
        form.AddField("usuario", nombreUsuario);
        form.AddField("nueva_contrasena", nuevaContrasena);
        form.AddField("token", tokenSecreto); 

        UnityWebRequest www = UnityWebRequest.Post(urlCambiarPassword, form);
        www.certificateHandler = new NoSSLValidation();  // Usamos el manejador personalizado
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            logTexto.text = "Error de conexión: " + www.error;
            logTexto.color = Color.red;
            logTexto.gameObject.SetActive(true);
        }
        else
        {
            string respuesta = www.downloadHandler.text;
            Debug.Log("Respuesta servidor: " + respuesta);

            CambioPassResponse res = JsonUtility.FromJson<CambioPassResponse>(respuesta);
            logTexto.text = res.message;
            logTexto.color = (res.status == "success") ? Color.green : Color.red;
            logTexto.gameObject.SetActive(true);

            if (res.status == "success")
            {
                gestionadorPaneles.MostrarPanel(panelAMostrar);
                VaciarCampos();
            }
        }
    }


    private void GestionarCamposVacios()
    {
        logTexto.text = "Todos los campos son obligatorios";
        logTexto.color = Color.red;
        logTexto.gameObject.SetActive(true);
        StartCoroutine(EsconderMensajeError());
    }

    IEnumerator EsconderMensajeError()
    {
        yield return new WaitForSeconds(3);
        logTexto.gameObject.SetActive(false);
    }

    private void VaciarCampos()
    {
        contrasenhaInput.text = "";
        contrasenhaInput2.text = "";
    }

    [System.Serializable]
    public class CambioPassResponse
    {
        public string status;
        public string message;
    }
}
