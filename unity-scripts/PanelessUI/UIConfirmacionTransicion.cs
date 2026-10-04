using UnityEngine;
using UnityEngine.UI;
using System;

public class UIConfirmacionTransicion : MonoBehaviour
{
    [SerializeField] private GameObject panelConfirmacion; 
    [SerializeField] private Button botonSi;
    [SerializeField] private Button botonNo;

    private Action onConfirm;
    private Action onCancel;

    private void Awake()
    {
        panelConfirmacion.SetActive(false);

        botonSi.onClick.AddListener(OnBotonSiClicked);
        botonNo.onClick.AddListener(OnBotonNoClicked);
    }
    private void OnBotonSiClicked()
    {
        panelConfirmacion.SetActive(false);
        if (onConfirm != null)
        {
            onConfirm.Invoke();
        }
    }
    private void OnBotonNoClicked()
    {
        panelConfirmacion.SetActive(false);
        if (onCancel != null)
        {
            onCancel.Invoke();
        }
    }
    public void Show(Action confirmar, Action cancelar)
    {
        Debug.Log("Mostrando UI de confirmación");
        if (panelConfirmacion == null)
        {
            Debug.LogError("panelConfirmacion es NULL");
            return;
        }

        panelConfirmacion.SetActive(true);
        onConfirm = confirmar;
        onCancel = cancelar;
    }
}
