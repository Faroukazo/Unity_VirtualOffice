using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGestionador : MonoBehaviour
{
    public GameObject panelLogin;
    public GameObject panelRegistrarse;
    public GameObject panelRecuperarCuenta;
    public GameObject panelCambiarContraseña;

    void Start()
    {
        MostrarPanel(panelLogin);
    }

    public void MostrarPanel(GameObject panel)
    {
        panelLogin.SetActive(false);
        panelRegistrarse.SetActive(false);
        panelRecuperarCuenta.SetActive(false);
        panelCambiarContraseña.SetActive(false);

        panel.SetActive(true);
    }
}
