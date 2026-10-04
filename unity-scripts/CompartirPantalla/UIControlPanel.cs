using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIControlPanel : MonoBehaviour
{
    public Button botonCompartir;
    public Button botonCancelarCompartir;
    public Button botonVer;

    public RawImage outputImage; 

    private DesktopCaptureManager captureManager;

    void Start()
    {
        // Verifica que el jugador local ya existe
        if (NetworkClient.localPlayer != null)
        {
            captureManager = NetworkClient.localPlayer.GetComponent<DesktopCaptureManager>();

            if (captureManager != null)
            {
                // Asigna la RawImage a DesktopCaptureManager
                captureManager.outputImage = outputImage;

                // Conecta los botones a los métodos del jugador
                botonCompartir.onClick.AddListener(captureManager.StartScreenSharing);
                botonCancelarCompartir.onClick.AddListener(captureManager.StopScreenSharing);
                botonVer.onClick.AddListener(captureManager.ViewSharedScreen);
            }
            else
            {
                Debug.LogWarning("DesktopCaptureManager no encontrado en el jugador local.");
            }
        }
        else
        {
            Debug.LogWarning("Jugador local no disponible. El panel no podrá funcionar correctamente.");
        }
    }
}
