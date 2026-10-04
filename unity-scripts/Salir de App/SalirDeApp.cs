using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalirDeApp : MonoBehaviour
{
    public void Salir()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
