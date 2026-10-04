using UnityEngine;
using System;

[Serializable]
public class Wrapper<T>
{
    public T[] notificaciones;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"notificaciones\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.notificaciones;
    }
}
