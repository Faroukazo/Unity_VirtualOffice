using UnityEngine;
using Unity.Services.Vivox;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine.Networking;
using System.Collections;

public class VivoxUserActions : MonoBehaviour
{
    public static VivoxUserActions Instance;
    private string token = Environment.GetEnvironmentVariable("API_SECRET_TOKEN") /* TODO: do not hardcode shared secrets in client code */;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
    }
    public async void ConnectAndJoinAsync(string channelToJoin)
    {
        string playerName = PlayerPrefs.GetString("username");
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                LoginOptions options = new LoginOptions();
                options.DisplayName = playerName;
                options.EnableTTS = true;

                Debug.Log("Attempting to login...");
                await VivoxService.Instance.LoginAsync(options);
                Debug.Log("Successfully logged in to Vivox.");
            }

            await ClienteEnCanal(channelToJoin, playerName);
            Debug.Log($"Attempting to join channel {channelToJoin}...");
            await VivoxService.Instance.JoinGroupChannelAsync(channelToJoin, ChatCapability.AudioOnly);
            await SendDataToPHPAsync(playerName, channelToJoin, token, "entrar");
            Debug.Log($"Successfully joined channel {channelToJoin}.");
            DiccionarioManager.Instance.AddPlayerToChannel(playerName, channelToJoin);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during Vivox connection: {ex.Message}\n{ex.StackTrace}");
        }
    }
    public async Task LeaveChannelAsync()
    {
        string playerName = PlayerPrefs.GetString("username");
        string currentChannel = DiccionarioManager.Instance.GetPlayerChannel(playerName);
        try
        {
            Debug.Log($"Attempting to leave channel {currentChannel}...");
            await SendDataToPHPAsync(playerName, currentChannel, token, "salir");
            await VivoxService.Instance.LeaveChannelAsync(currentChannel);
            Debug.Log($"Successfully left channel {currentChannel}.");
            DiccionarioManager.Instance.RemovePlayerFromChannel(playerName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during leaving channel: {ex.Message}\n{ex.StackTrace}");
        }
    }


    // Método que envía la solicitud HTTP POST al PHP
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


    private async Task ClienteEnCanal(string channelToJoin, string playerName)
    {
        if (DiccionarioManager.Instance.IsPlayerInChannel(playerName))
        {
            string currentChannel = DiccionarioManager.Instance.GetPlayerChannel(playerName);
            if (currentChannel == channelToJoin)
            {
                Debug.Log($"{playerName} is already in channel {channelToJoin}.");
                return;
            }
            else
            {
                Debug.Log($"{playerName} is in a different channel ({currentChannel}). Leaving current channel first...");
                await LeaveChannelAsync();
            }
        }
    }
    public async void SalirDeLosCanales()
    {
        string playerName = PlayerPrefs.GetString("username");
        string currentChannel = DiccionarioManager.Instance.GetPlayerChannel(PlayerPrefs.GetString("username"));
        await VivoxService.Instance.LeaveAllChannelsAsync();
        await SendDataToPHPAsync(playerName, currentChannel, token, "salir");
    }

    public async void LogoutAsync()
    {
        string playerName = PlayerPrefs.GetString("username");
        string currentChannel = DiccionarioManager.Instance.GetPlayerChannel(PlayerPrefs.GetString("username"));
        await SendDataToPHPAsync(playerName, currentChannel, token, "salir");
        await VivoxService.Instance.LogoutAsync();
    }

    private bool isMuted = false;

    public void ToggleMute()
    {
        if (isMuted)
        {
            VivoxService.Instance.UnmuteInputDevice();
            Debug.Log("User unmuted themselves.");
        }
        else
        {
            VivoxService.Instance.MuteInputDevice();
            Debug.Log("User muted themselves.");
        }

        isMuted = !isMuted;
    }

    private bool isDeafened = false;

    public void ToggleDeafen()
    {
        if (isDeafened)
        {
            VivoxService.Instance.UnmuteOutputDevice();
            VivoxService.Instance.UnmuteInputDevice();
            Debug.Log("User undeafened themselves.");
            Debug.Log("User unmuted themselves.");
        }
        else
        {
            VivoxService.Instance.MuteInputDevice();
            VivoxService.Instance.MuteOutputDevice();
            Debug.Log("User deafened themselves.");
            Debug.Log("User muted themselves.");
        }
        isDeafened = !isDeafened;
    }
}
