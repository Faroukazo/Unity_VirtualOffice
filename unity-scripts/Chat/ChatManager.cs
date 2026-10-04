using Mirror;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager instancia;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }       
    }

    [Command(requiresAuthority = false)]
    public void CmdEnviarMensaje(string mensaje, string remitente)
    {
        RpcMostrarMensaje(mensaje, remitente);
    }

    [ClientRpc]
    void RpcMostrarMensaje(string mensaje, string remitente)
    {
        ChatUI.instancia.MostrarMensaje($"{remitente}: {mensaje}");
    }
}
