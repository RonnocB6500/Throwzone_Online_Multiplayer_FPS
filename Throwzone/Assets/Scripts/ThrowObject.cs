using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowObject : NetworkBehaviour
{
    public static ThrowObject Instance { get; private set; }

    public event EventHandler throwObject;

    [SerializeField] private ThrowableObject throwableObjectPrefab;
    [SerializeField] private Transform throwableObjectHoldPoint;
    [SerializeField] private PlayerCam playerCam;
    [SerializeField] private Player player;



    //public ThrowableObject throwableObject;
    private float waitToSpawnTimerMax = 1f;
    private float waitToSpawnTimer = 0f;
    private float throwForce = 1500f;
    private bool isHoldingThrowable = false;
    public bool throwing = false;

    private Rigidbody rb;

    public ThrowableObject obj;

    public Collider objectCollider;


    private void Awake()
    {
        Instance = this;

        

        if (throwableObjectPrefab != null)
            throwableObjectPrefab.gameObject.SetActive(true);
    }

    void Start()
    {
        playerCam = gameObject.GetComponent<Player>().playerCam;
        GameInput.Instance.OnThrowAction += GameInput_OnThrowAction;
        
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!isHoldingThrowable)
        {
            waitToSpawnTimer += Time.deltaTime;
            if (waitToSpawnTimer > waitToSpawnTimerMax)
            {
                //throwableObject = Instantiate(throwableObjectPrefab);
                //throwableObject.throwableObjectHoldPoint = throwableObjectHoldPoint;
                //throwableObject.Show();
                //rb = throwableObject.GetComponent<Rigidbody>();
                isHoldingThrowable = true;
                RequestSpawnServerRpc(throwableObjectHoldPoint.position);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnServerRpc(Vector3 position)
    {
        SpawnObject(position);
    }

    public void SpawnObject(Vector3 position)
    {
        //if (!IsServer) return; // Only the server can spawn
        
        obj = Instantiate(throwableObjectPrefab, position, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn(true); // Syncs across all clients
        obj.throwableObjectHoldPoint = throwableObjectHoldPoint;
        //obj.SetFollow(throwableObjectHoldPoint);
        obj.transform.SetParent(player.transform);
        obj.clientThrownId = (int) OwnerClientId;
        rb = obj.GetComponent<Rigidbody>();
        objectCollider = obj.GetComponent<SphereCollider>();
        objectCollider.enabled = false;
        //rb.isKinematic = true;
        obj.Show();
    }

    private void GameInput_OnThrowAction(object sender, System.EventArgs e)
    {
        if (isHoldingThrowable && IsLocalPlayer && playerCam != null)
        {
            //Debug.Log("Throw Action");

            throwObject?.Invoke(this, EventArgs.Empty);
            Vector3 throwDirection = playerCam.GetCameraNormalVector();
            //Debug.Log(throwDirection);
            //RequestSpawnServerRpc(throwableObjectHoldPoint.position);
            ThrowServerRpc(throwDirection);

            waitToSpawnTimer = 0f;
            isHoldingThrowable = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowServerRpc(Vector3 throwDirection)
    {
        //ThrowClientRpc(throwDirection);
        Throw(throwDirection);
    }

    [ClientRpc]
    private void ThrowClientRpc(Vector3 throwDirection)
    {
        Throw(throwDirection);
    }

    private void Throw(Vector3 throwDirection)
    {
        if (obj == null) return;

        //Vector3 throwDirection = playerCam.GetCameraNormalVector();

        //if (throwableObject != null)
        //{
        obj.transform.position = throwableObjectHoldPoint.transform.position;

        obj.thrown = true;

        //obj.SetFollow(null);

        rb = obj.GetComponent<Rigidbody>();

        //rb.isKinematic = false;

        rb.freezeRotation = true;

        rb.useGravity = true;

        //Debug.Log("Add Force");

        rb.AddForce(throwDirection * throwForce);

        //obj.ThrowWithForce(throwDirection, throwForce);

        objectCollider.enabled = true;
        obj.transform.SetParent(null);
        //}
    }

    

    private bool IsHoldingThrowable()
    {
        return isHoldingThrowable;
    }
}
