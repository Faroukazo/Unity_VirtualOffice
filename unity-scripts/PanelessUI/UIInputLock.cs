using UnityEngine;

public class UIInputLock : MonoBehaviour
{
    public static UIInputLock instancia;
    public bool estaUsandoUI = false;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BloquearInput()
    {
        estaUsandoUI = true;
    }

    public void DesbloquearInput()
    {
        estaUsandoUI = false;
    }
}
