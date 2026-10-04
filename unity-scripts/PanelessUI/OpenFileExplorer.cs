using UnityEngine;
using SimpleFileBrowser;
using System;
using System.Collections;

public class OpenFileExplorer : MonoBehaviour
{
    public GameObject fileBrowserCanvas;
    private Action<string> onFileSelected;

    public void StartFileBrowser(Action<string> callback)
    {
        onFileSelected = callback;
        StartCoroutine(ShowFileDialog());
    }

    IEnumerator ShowFileDialog()
    {
        if (fileBrowserCanvas != null && !fileBrowserCanvas.activeSelf)
            fileBrowserCanvas.SetActive(true);

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Selecciona un archivo", "Seleccionar");

        if (FileBrowser.Success)
        {
            string selectedFile = FileBrowser.Result[0];
            Debug.Log("Archivo seleccionado: " + selectedFile);

            onFileSelected?.Invoke(selectedFile);
        }
        else
        {
            Debug.Log("No se seleccionó ningún archivo.");
        }

        if (fileBrowserCanvas != null)
        {
            fileBrowserCanvas.SetActive(false);
        }
    }
}
