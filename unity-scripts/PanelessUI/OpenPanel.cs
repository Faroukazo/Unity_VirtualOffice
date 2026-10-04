using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] private UIConfirmacionTransicion uiConfirmacion;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            if (collision.TryGetComponent<Movimiento>(out Movimiento playerMoveScript))
            {
                playerMoveScript.enabled = false;
                uiConfirmacion.Show(
                    confirmar: () =>
                    {
                        UIInputLock.instancia.BloquearInput();
                        playerMoveScript.enabled = true;
                    },
                    cancelar: () =>
                    {
                        panel.SetActive(false);
                        UIInputLock.instancia.DesbloquearInput();
                        playerMoveScript.enabled = true;
                    });
            }
            else
            {
                UIInputLock.instancia.BloquearInput();
            }
        }
    }
}
