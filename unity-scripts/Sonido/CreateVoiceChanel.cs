using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateVoiceChanel : MonoBehaviour
{
    [Header("NombreSala InputTextField")]
    public TMP_InputField inputChat;

    [Header("Texto para mostrar error")]
    public TMP_Text errorText;

    private char[] invalidChars = new char[] { '@', '-', '.', '!', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=', '/', '\\', ',', ' ' };

    public void CreateChatVoice()
    {
        string nombreSala = inputChat.text;
        errorText.text = "";
        if (string.IsNullOrWhiteSpace(nombreSala))
        {
            errorText.text = "El nombre de la sala no puede estar vacío.";
            return;
        }
        foreach (char c in invalidChars)
        {
            if (nombreSala.Contains(c.ToString()))
            {
                errorText.text = $"El nombre no puede contener caracteres: {string.Join(" ", invalidChars)}";
                return;
            }
        }
        if(nombreSala == "Lobby" || nombreSala == "Oficina")
        {
            errorText.text = "El nombre de la sala no puede ser 'Lobby' o 'Oficina'";
            return;
        }
        CrearSala(nombreSala);
    }

    private void CrearSala(string nombreSala)
    {
        string remitente = PlayerPrefs.GetString("username");
        if (VivoxUserActions.Instance != null)
        {
            VivoxUserActions.Instance.ConnectAndJoinAsync(nombreSala);
        }
        else
        {
            Debug.LogError("VivoxUserActions no está disponible.");
        }
        inputChat.text = "";
        inputChat.ActivateInputField();
    }
}
