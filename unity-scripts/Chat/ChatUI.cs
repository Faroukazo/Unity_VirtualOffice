using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public static ChatUI instancia;

    [Header("Para escribir")]
    public TMP_InputField inputChat;
    [Header("Para mostrar")]
    public TextMeshProUGUI chatTextArea;
    [Header("Para enviar")]
    public Button enviarBtn;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }

        enviarBtn.onClick.AddListener(EnviarMensaje);
    }

    private void Update()
    {
        if (inputChat.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            EnviarMensaje();
        }
    }

    public void EnviarMensaje()
    {
        string mensaje = inputChat.text;

        if (!string.IsNullOrWhiteSpace(mensaje))
        {
            string remitente = PlayerPrefs.GetString("username"); 
            ChatManager.instancia.CmdEnviarMensaje(mensaje, remitente);
            inputChat.text = "";
            inputChat.ActivateInputField();
        }
    }

    public void MostrarMensaje(string mensaje)
    {
        chatTextArea.text += mensaje + "\n";
    }

    public bool ChatActivo()
    {
        return inputChat.isFocused;
    }
}
