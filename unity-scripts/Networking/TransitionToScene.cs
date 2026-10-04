using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class TransitionToScene : NetworkBehaviour
{
    [Header("Referencias necesarias")]
    [SerializeField] private MyNetworkManager myNetworkManagerScript;
    [SerializeField] private FadeInOutScreen fadeInOutScreenScript;

    [Header("Datos de transición")]
    [Scene]
    public string transitionToSceneName;
    public string scenePosToSpawnOn;

    [Header("UI de confirmación de transición")]
    [SerializeField] private UIConfirmacionTransicion uiConfirmacion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isClient || collision.gameObject != NetworkClient.localPlayer?.gameObject) return;

        if (collision.TryGetComponent<Movimiento>(out Movimiento playerMoveScript))
        {
            playerMoveScript.enabled = false;

            uiConfirmacion.Show(
                confirmar: () =>
                {
                    playerMoveScript.CmdSolicitarTransicion(transitionToSceneName, scenePosToSpawnOn);
                },
                cancelar: () =>
                {
                    playerMoveScript.enabled = true;
                });
        }
    }
}
