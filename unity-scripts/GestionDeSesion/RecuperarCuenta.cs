using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RecuperarCuenta : MonoBehaviour
{
    public TMP_InputField nombreInput;
    public TMP_InputField apellidoInput;
    public TMP_InputField usuarioInput;
    public TMP_InputField correoInput;
    public TextMeshProUGUI logTexto;
    public GameObject panelAMostrar;
    private UIGestionador gestionadorPaneles;

    private string urlRecuperar = "https://www.oficinavirtuales.com/recoverAccount.php";

    void Start()
    {
        logTexto.gameObject.SetActive(false);
        gestionadorPaneles = FindObjectOfType<UIGestionador>();
    }

    public void GetDatoRecuperarCuenta(GameObject panel)
    {
        string nombre = nombreInput.text.Trim();
        string apellido = apellidoInput.text.Trim();
        string usuario = usuarioInput.text.Trim();
        string correo = correoInput.text.Trim();
        panelAMostrar = panel;

        if (nombre == "" || apellido == "" || usuario == "" || correo == "")
        {
            GestionarCamposVacios();
        }
        else
        {
            StartCoroutine(VerificarDatos(nombre, apellido, usuario, correo));
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
    IEnumerator VerificarDatos(string nombre, string apellido, string usuario, string correo)
    {
        WWWForm form = new WWWForm();
        form.AddField("nombre", nombre);
        form.AddField("apellido", apellido);
        form.AddField("usuario", usuario);
        form.AddField("correo", correo);
        form.AddField("token", tokenSecreto);

        UnityWebRequest www = UnityWebRequest.Post(urlRecuperar, form);
        www.certificateHandler = new NoSSLValidation();  // Usamos el manejador personalizado
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            logTexto.text = "Error al conectar con el servidor";
            logTexto.color = Color.red;
            logTexto.gameObject.SetActive(true);
            Debug.Log("Error: " + www.error);
        }
        else
        {
            string respuestaJson = www.downloadHandler.text;
            Debug.Log("Respuesta servidor: " + respuestaJson);

            RecuperarResponse respuesta = JsonUtility.FromJson<RecuperarResponse>(respuestaJson);

            logTexto.gameObject.SetActive(true);
            logTexto.text = respuesta.message;

            if (respuesta.status == "success")
            {
                logTexto.color = Color.green;
                gestionadorPaneles.MostrarPanel(panelAMostrar);
                CambiarPassword.nombreUsuario = usuario;
                VaciarCampos();
            }
            else
            {
                logTexto.color = Color.red;
            }
        }
    }


    private void GestionarCamposVacios()
    {
        logTexto.text = "Todos los campos son obligatorios";
        logTexto.color = Color.red;
        logTexto.gameObject.SetActive(true);
        StartCoroutine(EsconderMensajeError());
        Debug.Log("Campos vacíos");
    }

    IEnumerator EsconderMensajeError()
    {
        yield return new WaitForSeconds(3);
        logTexto.gameObject.SetActive(false);
    }

    private void VaciarCampos()
    {
        nombreInput.text = "";
        apellidoInput.text = "";
        usuarioInput.text = "";
        correoInput.text = "";
    }

    [System.Serializable]
    public class RecuperarResponse
    {
        public string status;
        public string message;
    }
}
