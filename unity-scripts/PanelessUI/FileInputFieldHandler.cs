using TMPro;
using UnityEngine;

public class FileInputFieldHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public OpenFileExplorer fileUploader;

    void Start()
    {
        inputField.onSelect.AddListener(OnInputFieldClicked);
    }

    void OnInputFieldClicked(string value)
    {
        if (fileUploader != null)
        {
            fileUploader.StartFileBrowser(SetFilePath);
        }
    }

    void SetFilePath(string path)
    {
        inputField.text = path;
    }
}
