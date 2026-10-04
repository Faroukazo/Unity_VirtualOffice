using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Movimiento : NetworkBehaviour
{
    public static Movimiento instancia;

    public float velocidad = 5f;
    private Rigidbody2D rb2;
    private Vector2 move;
    private Animator animator;

    private float dirX;
    private float dirY;
    private bool isMoving;

    [SyncVar(hook = nameof(OnDirXChanged))] private float syncDirX;
    [SyncVar(hook = nameof(OnDirYChanged))] private float syncDirY;
    [SyncVar(hook = nameof(OnIsMovingChanged))] private bool syncIsMoving;

    [Header("Input del chat (asignar en el inspector)")]
    public TMP_InputField chatInputField;

    private void Awake()
    {
        if (isLocalPlayer)
        {
            if (instancia == null)
            {
                instancia = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instancia != this)
            {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator no encontrado en: " + gameObject.name);

        MoverA_SpawnPoint();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if ((UIInputLock.instancia != null && UIInputLock.instancia.estaUsandoUI) || (ChatUI.instancia != null && ChatUI.instancia.ChatActivo()))
            {
                move = Vector2.zero;
                isMoving = false;
                return;
            }

            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            move = new Vector2(moveX, moveY).normalized;

            dirX = moveX != 0 ? Mathf.Sign(moveX) : 0;
            dirY = moveY != 0 ? Mathf.Sign(moveY) : 0;
            isMoving = move.magnitude > 0;

            if (animator != null)
            {
                animator.SetFloat("DirX", dirX);
                animator.SetFloat("DirY", dirY);
                animator.SetBool("isMoving", isMoving);
            }

            CmdUpdateAnimation(dirX, dirY, isMoving);
        }
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            rb2.velocity = move * velocidad;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MoverA_SpawnPoint();
    }

    private void MoverA_SpawnPoint()
    {
        GameObject spawnPoint = GameObject.Find("SpawnPointLobby");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [Command]
    void CmdUpdateAnimation(float newDirX, float newDirY, bool newIsMoving)
    {
        syncDirX = newDirX;
        syncDirY = newDirY;
        syncIsMoving = newIsMoving;
    }

    private void OnDirXChanged(float oldValue, float newValue)
    {
        if (animator != null)
            animator.SetFloat("DirX", newValue);
    }

    private void OnDirYChanged(float oldValue, float newValue)
    {
        if (animator != null)
            animator.SetFloat("DirY", newValue);
    }

    private void OnIsMovingChanged(bool oldValue, bool newValue)
    {
        if (animator != null)
            animator.SetBool("isMoving", newValue);
    }
    [Command]
    public void CmdSolicitarTransicion(string transitionToSceneName, string spawnPointName)
    {
        StartCoroutine(SendPlayerToNewScene(transitionToSceneName, spawnPointName));
    }

    [Server]
    private IEnumerator SendPlayerToNewScene(string sceneName, string spawnPoint)
    {
        MyNetworkManager myNetworkManager = FindObjectOfType<MyNetworkManager>();
        FadeInOutScreen fadeInOutScreen = FindObjectOfType<FadeInOutScreen>();

        if (myNetworkManager == null || fadeInOutScreen == null)
        {
            yield break;
        }

        NetworkConnectionToClient conn = connectionToClient;
        conn.Send(new SceneMessage { sceneName = gameObject.scene.path, sceneOperation = SceneOperation.UnloadAdditive, customHandling = true });
        yield return new WaitForSeconds((fadeInOutScreen.speed * 0.1f));

#pragma warning disable CS0618
        NetworkServer.RemovePlayerForConnection(conn, false);
#pragma warning restore CS0618

        var puntoInicio = StartPositionRegistry.instancia.Obtener(spawnPoint);
        Transform start = myNetworkManager.GetStartPosition();
        if (puntoInicio != null)
        {
            start = puntoInicio.transform;
        }

        transform.position = start.position;
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByPath(sceneName));
        conn.Send(new SceneMessage { sceneName = sceneName, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });
        NetworkServer.AddPlayerForConnection(conn, gameObject);
        if (TryGetComponent<Movimiento>(out Movimiento moveScript))
        {
            moveScript.enabled = true;
        }
    }

}
