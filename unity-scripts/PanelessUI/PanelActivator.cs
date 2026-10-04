using TMPro;
using UnityEngine;

public class PanelActivator : MonoBehaviour
{
    [Header("Panel SubirArchivos")]
    public GameObject panel;
    [Header("Panel DescargarAechivos")]
    public GameObject panelDescargarArchivos;
    [Header("Campos de entrada")]
    public TMP_InputField nombreInput;
    public TMP_InputField asuntoInput;
    public TMP_InputField urlInput;

    [Header("Mensaje de error")]
    public TextMeshProUGUI mensajeError;

    public void ActivatePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            UIInputLock.instancia.BloquearInput(); 
        }
    }

    public void DeactivatePanel()
    {
        if (panel != null)
        {
            BorrarCampos();
            panel.SetActive(false);
            UIInputLock.instancia.DesbloquearInput();  
        }
    }

    private void BorrarCampos()
    { 
        nombreInput.text = "";
        asuntoInput.text = "";
        urlInput.text = "";
        mensajeError.text = "";
        mensajeError.gameObject.SetActive(false);
    }    
}
