using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using Unity.Services.Core.Environments;
using System;
using System.Threading.Tasks;

public class VivoxInitializer : MonoBehaviour
{
    async void Start()
    {
        try
        {
            Debug.Log("Preparing Unity Services...");
            var options = new InitializationOptions();
            options.SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options);
            Debug.Log("Unity Services initialized.");
            if (AuthenticationService.Instance != null)
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            else
            {
                Debug.LogError("AuthenticationService.Instance is still null after initialization!");
            }

            Debug.Log("Initializing Vivox...");
            await VivoxService.Instance.InitializeAsync();
            Debug.Log("Vivox initialized successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[VivoxInitializer] Initialization failed: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
