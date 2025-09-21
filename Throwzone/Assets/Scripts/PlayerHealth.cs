using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public static PlayerHealth Instance { get; private set; }
    public event EventHandler playerDeath;

    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private PlayerCam playerCam;
    [SerializeField] private Player player;
    private Rigidbody rb;

    private float playerBaseHealth = 100f;
    private float playerCurrentHealth;
    private float playerHealthRegen = 1f;
    private float playerHealthRegenTimerMax = 1f;
    private float playerHealthRegenTimer = 0f;
    public bool dead = false;
    private float respawnTimerMax = 7f;
    private float respawnTimerCurrent = 0f;

    private ThrowObject throwObject;

    private MeshRenderer meshBody;

    private int lastHitBy_playerId;

    private int clientId;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        throwObject = player.GetComponent<ThrowObject>();

        clientId = player.clientId;

        playerCurrentHealth = playerBaseHealth;
    }

    private void Update()
    {
        //Debug.Log("Player " + clientId + " is dead: " + dead);
        //Debug.Log("Player " + clientId + " health: " + playerCurrentHealth);

        HealthRegenServerRpc();

        if (playerCurrentHealth <= 0)
        {
            DeathServerRpc();
            
        }

        if (dead)
        {
            respawnTimerCurrent += Time.deltaTime;
            if (respawnTimerCurrent > respawnTimerMax)
            {
                RespawnServerRpc();
                respawnTimerCurrent = 0f;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Player " + clientId + " hit.");
        if (dead || playerCurrentHealth <= 0)
        {
            return;
        }
        GameObject otherObject = other.gameObject;
        ThrowableObject throwableObject = otherObject.GetComponent<ThrowableObject>();
        int damage = throwableObject.damage;
        //if (throwableObject.clientThrownId != player.clientId)
        //{
        if (otherObject.CompareTag("Throwable"))
        {
            TakeDamageServerRpc(damage);
            lastHitBy_playerId = throwableObject.clientThrownId;
        }
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    private void HealthRegenServerRpc()
    {
        HealthRegenClientRpc();
    }

    [ClientRpc]
    private void HealthRegenClientRpc()
    {
        if (playerCurrentHealth < playerBaseHealth && playerCurrentHealth > 0)
        {
            playerHealthRegenTimer += Time.deltaTime;
            if (playerHealthRegenTimer > playerHealthRegenTimerMax)
            {
                playerHealthRegenTimer = 0f;
                playerCurrentHealth += playerHealthRegen;
            }
        }
        else
        {
            playerHealthRegenTimer = 0f;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int damage)
    {
        TakeDamageClientRpc(damage);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(int damage)
    {
        playerCurrentHealth -= damage;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeathServerRpc()
    {
        DeathClientRpc();
        
        ThrowGameMultiplayer.Instance.AddKillForClientId(lastHitBy_playerId);
    }

    [ClientRpc]
    private void DeathClientRpc()
    {
        if (IsLocalPlayer)
        {
            GameInput.Instance.playerInputActions.Player.Disable();
            playerDeath?.Invoke(this, EventArgs.Empty);
        }
        //player.transform.position = new Vector3(0, 10, 0);
        rb.constraints = RigidbodyConstraints.FreezePosition;
        playerCurrentHealth = playerBaseHealth;
        //playerVisual.Hide();
        dead = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnServerRpc()
    {
        RespawnClientRpc();
    }
    
    [ClientRpc]
    private void RespawnClientRpc()
    {
        if (IsLocalPlayer)
        {
            GameInput.Instance.playerInputActions.Player.Enable();
        }
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerCurrentHealth = playerBaseHealth;
        //playerVisual.Show();
        dead = false;
        playerCam.transform.localPosition = new Vector3(0,1.4f,0.45f);
    }
}
