using UnityEngine;
using Mirror;
using Unity.Services.Vivox;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class ClientDisconnectionHandler : MonoBehaviour
{
    private string token = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;
    public async void DisconnectClient()
    {
        // Verificar si el cliente está conectado antes de intentar desconectarlo
        if (NetworkClient.isConnected)
        {
            Debug.Log("Desconectando al cliente...");            
            NetworkManager.singleton.StopClient();
            VivoxUserActions.Instance.LogoutAsync();
            await SalirChatDeVozUI();

        }
        else
        {
            Debug.LogWarning("El cliente ya está desconectado.");
        }
    }    
    private async Task SalirChatDeVozUI()
    {
        string playerName = PlayerPrefs.GetString("username");
        string currentChannel = DiccionarioManager.Instance.GetPlayerChannel(playerName);
        await SendDataToPHPAsync(playerName, currentChannel, token, "salir");
    }
    private async Task SendDataToPHPAsync(string usuario, string nombreSala, string token, string action)
    {
        string url = "https://www.oficinavirtuales.com/entradaSalidaChatDeVoz.php";
        WWWForm form = new WWWForm();
        form.AddField("usuario", usuario);
        form.AddField("nombreSala", nombreSala);
        form.AddField("token", token);
        form.AddField("action", action);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var asyncOp = www.SendWebRequest();

            while (!asyncOp.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending data to PHP: " + www.error);
            }
            else
            {
                Debug.Log("Data sent successfully to PHP: " + www.downloadHandler.text);
            }
        }
    }
}
