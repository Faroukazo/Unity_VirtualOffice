using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DiccionarioManager : MonoBehaviour
{
    private Dictionary<string, string> playerChannelDict = new Dictionary<string, string>();
    public static DiccionarioManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        if (SceneManager.GetActiveScene().name == "GestionarLoggin")
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AddPlayerToChannel(string playerName, string channelName)
    {
        if (playerChannelDict.ContainsKey(playerName))
        {
            playerChannelDict[playerName] = channelName;
        }
        else
        {
            playerChannelDict.Add(playerName, channelName);
        }
        Debug.Log($"{playerName} added to channel {channelName}");
    }
    public void RemovePlayerFromChannel(string playerName)
    {
        if (playerChannelDict.ContainsKey(playerName))
        {
            playerChannelDict.Remove(playerName);
            Debug.Log($"{playerName} removed from channel.");
        }
        else
        {
            Debug.LogWarning($"Player {playerName} not found in the dictionary.");
        }
    }
    public string GetPlayerChannel(string playerName)
    {
        if (playerChannelDict.ContainsKey(playerName))
        {
            return playerChannelDict[playerName];
        }
        return null;
    }
    public bool IsPlayerInChannel(string playerName)
    {
        return playerChannelDict.ContainsKey(playerName);
    }
}
