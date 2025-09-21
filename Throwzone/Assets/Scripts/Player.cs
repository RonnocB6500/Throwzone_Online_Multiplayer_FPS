using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {


    public static event EventHandler OnAnyPlayerSpawned;


    public static void ResetStaticData() {
        OnAnyPlayerSpawned = null;
    }


    public static Player LocalInstance { get; private set; }



    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] public List<Vector3> spawnPositionListTeamA;
    [SerializeField] private List<Vector3> spawnPositionListTeamB;
    [SerializeField] private PlayerVisual playerVisual;



    // Booleans
    private bool isWalkingFWD;
    private bool isWalkingBWD;
    private bool isWalkingLFT;
    private bool isWalkingRGT;
    private bool isRunning;
    private bool isJumping;
    private bool isShiftDown;


    // Move Speed
    private float walkSpeed = 5f;
    private float sprintSpeed = 10f;
    private float currentSpeed = 5f;

    // Grounded
    [SerializeField] public LayerMask whatIsGround;
    private bool grounded;
    private float jumpForce = 3000f;
    private float jumpCooldown = 0.25f;
    private float airMultiplier = 0.6f;
    private bool readyToJump = true;

    // Player
    public int clientId;
    private Rigidbody rb;
    [SerializeField] public PlayerCam playerCam;

    // Camera
    [SerializeField] public PlayerCamHolder playerCamHolder;
    private Vector3 cameraNormal;
    private Vector3 cameraOffset;

    public float sensX;
    public float sensY;

    float xRotation;
    float yRotation;


    private void Awake()
    {
        //transform.position = spawnPositionListTeamA[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        //transform.position = spawnPositionListTeamA[(int)OwnerClientId];

        if (IsLocalPlayer)
        {
            playerCam.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        //transform.position = spawnPositionListTeamA[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        //transform.position = spawnPositionListTeamA[(int)OwnerClientId];
        SetPlayerSpawnPositionServerRpc();

        //playerCam = playerCamHolder.playerCam;

        cameraOffset = new Vector3(0, 1.4f, 0.45f);
        playerCamHolder.transform.position = transform.position;

        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        //GameInput.Instance.OnSprintAction += GameInput_OnSprintAction;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        PlayerData playerData = ThrowGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        clientId = (int)playerData.clientId;

        playerVisual.ActiveCloakOption(playerData.cloakOptionId);
        playerVisual.ActiveBodyOption(playerData.bodyOptionId);
        playerVisual.ActiveHairOption(playerData.hairOptionId);
        playerVisual.ActiveHeadOption(playerData.headOptionId);
        playerVisual.ActiveEyesOption(playerData.eyesOptionId);
        playerVisual.ActiveMouthOption(playerData.mouthOptionId);
        playerVisual.SetPlayerCloakColor(ThrowGameMultiplayer.Instance.GetPlayerCloakColor(playerData.cloakColorId));
        playerVisual.SetPlayerBodyColor(ThrowGameMultiplayer.Instance.GetPlayerBodyColor(playerData.bodyColorId));
        playerVisual.SetPlayerHairColor(ThrowGameMultiplayer.Instance.GetPlayerHairColor(playerData.hairColorId));

        //playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    private void Update()
    {
        //grounded = !Physics.Raycast(transform.position, Vector3.down, 10f, whatIsGround);
        //Debug.DrawRay(transform.position, Vector3.down * 10f, Color.blue);

        if (!IsOwner)
        {
            return;
        }

        //Ground Check

        playerCamHolder.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        MoveCamera();
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject otherObject = collision.gameObject;
        if (otherObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject otherObject = collision.gameObject;
        if (otherObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3();
        moveDir = transform.forward * inputVector.y + transform.right * inputVector.x;

        Vector3 direction = GetVectorLargestDirection(inputVector);

        //Debug.Log(direction);

        HandleMovementBooleans(direction);

        if (grounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }
        }


        //Debug.Log("Walk: " + isWalking);
        //Debug.Log("Run: " + isRunning);
        //Debug.Log("Jump: " + isJumping);

        if (grounded)
        {
            rb.linearVelocity = new Vector3(moveDir.normalized.x * currentSpeed, rb.linearVelocity.y, moveDir.normalized.z * currentSpeed);
        }
        else if (!grounded)
        {
            rb.linearVelocity = new Vector3(moveDir.normalized.x * currentSpeed * airMultiplier, rb.linearVelocity.y, moveDir.normalized.z * currentSpeed * airMultiplier);
        }
    }

    private void MoveCamera()
    {
        // get mouse input
        /*float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);*/

        //transform.rotation = Quaternion.Euler(0, yRotation, 0);

        //playerCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        // Rotate cam and orientation
        //transform.rotation = Quaternion.Euler(0, yRotation, 0);

        //transform.Rotate(new Vector3(xRotation, 0, 0), Space.Self);

        //playerCamHolder.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        //playerCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        //cameraHolder.transform.rotation.x = xRotation;

        //Vector3 offset = new Vector3(0, 1.4f, 0.45f);            

        // Get the forward vector of the camera's transform
        //cameraNormal = playerCam.GetCameraNormalVector();

        //Debug.Log("Player: " + cameraNormal);

        // You can use this vector for various purposes, e.g., raycasting, movement
        //Debug.DrawRay(transform.position, cameraNormal * 10f, Color.blue);

        //transform.position = player.transform.position + offset;
    }

    private Vector3 GetVectorLargestDirection(Vector3 vector)
    {
        float x = Math.Abs(vector.x);
        float y = Math.Abs(vector.y);

        if (x == 0 && y == 0)
        {
            //Do nothing
        }
        else if (x < y)
        {
            return new Vector3(0, vector.y, 0);
        }
        else if (x >= y)
        {
            return new Vector3(vector.x, 0, 0);
        }

        return Vector3.zero;
    }

    private void HandleMovementBooleans(Vector3 direction)
    {
        if (grounded && !gameObject.GetComponent<PlayerHealth>().dead)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isWalkingFWD = false;
                isWalkingBWD = false;
                isWalkingLFT = false;
                isWalkingRGT = false;
                isRunning = true;
                isJumping = false;
            }
            else
            {
                if (direction.x == 0 && direction.y != 0)
                {
                    if (direction.y > 0)
                    {
                        isWalkingFWD = true;
                        isWalkingBWD = false;
                        isWalkingLFT = false;
                        isWalkingRGT = false;
                        isRunning = false;
                        isJumping = false;
                    }
                    else if (direction.y < 0)
                    {
                        isWalkingFWD = false;
                        isWalkingBWD = true;
                        isWalkingLFT = false;
                        isWalkingRGT = false;
                        isRunning = false;
                        isJumping = false;
                    }
                }
                else if (direction.y == 0 && direction.x != 0)
                {
                    if (direction.x > 0)
                    {
                        isWalkingFWD = false;
                        isWalkingBWD = false;
                        isWalkingLFT = false;
                        isWalkingRGT = true;
                        isRunning = false;
                        isJumping = false;
                    }
                    else if (direction.x < 0)
                    {
                        isWalkingFWD = false;
                        isWalkingBWD = false;
                        isWalkingLFT = true;
                        isWalkingRGT = false;
                        isRunning = false;
                        isJumping = false;
                    }
                }
                else
                {
                    isWalkingFWD = false;
                    isWalkingBWD = false;
                    isWalkingLFT = false;
                    isWalkingRGT = false;
                    isRunning = false;
                    isJumping = false;
                }
            }
        }
        else if (gameObject.GetComponent<PlayerHealth>().dead)
        {
            isWalkingFWD = false;
            isWalkingBWD = false;
            isWalkingLFT = false;
            isWalkingRGT = false;
            isRunning = false;
            isJumping = false;
        }
        else if (!grounded)
        {
            isWalkingFWD = false;
            isWalkingBWD = false;
            isWalkingLFT = false;
            isWalkingRGT = false;
            isRunning = false;
            isJumping = true;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            playerCam.Show();
            transform.position = spawnPositionListTeamA[(int)OwnerClientId];
        }

        //Debug.Log("NetworkSpawn");

        //transform.position = spawnPositionListTeamA[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        //transform.position = spawnPositionListTeamA[(int)OwnerClientId];
        SetPlayerSpawnPositionServerRpc();

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerSpawnPositionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerSpawnPositionClientRpc();
    }

    [ClientRpc]
    private void SetPlayerSpawnPositionClientRpc()
    {
        transform.position = spawnPositionListTeamA[(int)OwnerClientId];
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //if (clientId == OwnerClientId && HasKitchenObject())
    }

    private void GameInput_OnJumpAction(object sender, EventArgs e)
    {
        //Debug.Log("Player " + clientId + " attempted Jump");
        Jump();
        readyToJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            //Debug.Log("Player " + clientId + " Jumped");
            
            //rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!ThrowGameManager.Instance.IsGamePlaying()) return;

    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (!ThrowGameManager.Instance.IsGamePlaying()) return;

    }


    public bool IsWalkingFWD() {
        return isWalkingFWD;
    }
    public bool IsWalkingBWD() {
        return isWalkingBWD;
    }
    public bool IsWalkingLFT() {
        return isWalkingLFT;
    }
    public bool IsWalkingRGT() {
        return isWalkingRGT;
    }
    
    public bool IsRunning()
    {
        return isRunning;
    }
    
    public bool IsJumping()
    {
        return isJumping;
    }


    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }


    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }

}