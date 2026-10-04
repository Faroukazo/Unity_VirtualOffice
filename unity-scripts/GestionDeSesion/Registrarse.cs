using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Registrarse : MonoBehaviour
{
    public TMP_InputField nombreInput;
    public TMP_InputField apellidoInput;
    public TMP_InputField usuarioInput;
    public TMP_InputField correoInput;
    public TMP_InputField contrasenhaInput;
    public TextMeshProUGUI logTexto;
    public GameObject panelAMostrar;
    private UIGestionador gestionadorPaneles;

    private string urlRegistro = "https://www.oficinavirtuales.com/registrarse.php"; 

    void Start()
    {
        logTexto.gameObject.SetActive(false);
        gestionadorPaneles = FindObjectOfType<UIGestionador>();
    }

    public void GetDatosRegistrarse(GameObject panel)
    {
        string nombre = nombreInput.text.Trim();
        string apellido = apellidoInput.text.Trim();
        string usuario = usuarioInput.text.Trim();
        string correo = correoInput.text.Trim();
        string contrasena = contrasenhaInput.text.Trim();
        panelAMostrar = panel;

        if (nombre == "" || apellido == "" || usuario == "" || correo == "" || contrasena == "")
        {
            GestionarCamposVacios();
        }
        else
        {
            StartCoroutine(EnviarRegistro(nombre, apellido, usuario, correo, contrasena));
        }
    }
    public void VolverPanelAnterior(GameObject panel)
    {
        if (panel != null)
        {
            gestionadorPaneles.MostrarPanel(panel); 
            VaciarCampos(); 
        }
        else
        {
            Debug.LogWarning("El panel proporcionado es nulo.");
        }
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
    IEnumerator EnviarRegistro(string nombre, string apellido, string usuario, string correo, string contrasena)
    {
        WWWForm form = new WWWForm();
        form.AddField("nombre", nombre);
        form.AddField("apellido", apellido);
        form.AddField("usuario", usuario);
        form.AddField("correo", correo);
        form.AddField("contrasena", contrasena);
        form.AddField("token", tokenSecreto);

        UnityWebRequest www = UnityWebRequest.Post(urlRegistro, form);
        www.certificateHandler = new NoSSLValidation();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            logTexto.text = "Error de conexión: " + www.error;
            logTexto.color = Color.red;
            logTexto.gameObject.SetActive(true);
        }
        else
        {
            GestionarRespuesta(www);
        }
    }

    private void GestionarRespuesta(UnityWebRequest www)
    {
        string respuesta = www.downloadHandler.text;
        Debug.Log("Respuesta del servidor: " + respuesta);

        RegistroResponse res = JsonUtility.FromJson<RegistroResponse>(respuesta);

        logTexto.text = res.message;
        logTexto.color = (res.status == "success") ? Color.green : Color.red;
        logTexto.gameObject.SetActive(true);

        if (res.status == "success")
        {
            gestionadorPaneles.MostrarPanel(panelAMostrar);
            VaciarCampos();
        }
    }

    private void VaciarCampos()
    {
        nombreInput.text = "";
        apellidoInput.text = "";
        usuarioInput.text = "";
        correoInput.text = "";
        contrasenhaInput.text = "";
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

    [System.Serializable]
    public class RegistroResponse
    {
        public string status;
        public string message;
    }
}
