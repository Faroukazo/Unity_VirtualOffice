using Mirror;
using TMPro;
using UnityEngine;

public class NombreJugador : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNombreCambiado))]
    public string nombreUsuario;
    public TextMeshPro textoNombre;

    public override void OnStartLocalPlayer()
    {
        string miNombre = PlayerPrefs.GetString("username");
        CmdEstablecerNombre(miNombre);
    }
    [Command]
    void CmdEstablecerNombre(string nombre)
    {
        nombreUsuario = nombre;
    }
    void OnNombreCambiado(string anterior, string nuevo)
    {
        textoNombre.text = nuevo;
    }
}
