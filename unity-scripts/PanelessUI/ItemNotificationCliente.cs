using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemNotificationCliente : MonoBehaviour
{
    [Header("Componentes de UI Notificaciones")]
    public TMP_Text usuarioText;
    public TMP_Text nombreText;

    public void Inicializar(string usuario, string nombre)
    {
        usuarioText.text = "Usuario: "+usuario;
        nombreText.text = "Nombre: "+nombre;
    }
}
