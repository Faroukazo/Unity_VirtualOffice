using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerrarPanel : MonoBehaviour
{
    public GameObject panel;
    public void DeactivatePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
            UIInputLock.instancia.DesbloquearInput();
        }        
    }
}
