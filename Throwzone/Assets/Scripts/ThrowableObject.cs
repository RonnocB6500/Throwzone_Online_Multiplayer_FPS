using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Animations;

public class ThrowableObject : NetworkBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] private GameObject throwableVisual;

    public Transform throwableObjectHoldPoint;
    public bool thrown;
    private float throwableObjectLifetime = 5f;
    private float throwableObjectLifetimer = 0f;

    private float destroyTimerMax = 0.1f;
    private float destroyTimerCurrent = 0f;
    private bool hidden = false;

    public int clientThrownId;

    private Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        GravityOff();

        thrown = false;

    }

    private void Update()
    {
        if (!thrown && throwableObjectHoldPoint != null)
        {
            transform.position = throwableObjectHoldPoint.position;
        }
        else if (thrown)
        {
            throwableObjectLifetimer += Time.deltaTime;
            if (throwableObjectLifetimer > throwableObjectLifetime)
            {
                throwableObjectLifetimer = 0f;
                DestroyServerRpc();
            }
        }

        if (hidden)
        {
            destroyTimerCurrent += Time.deltaTime;
            if (destroyTimerCurrent > destroyTimerMax)
            {
                DestroyServerRpc();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (clientThrownId == other.gameObject.GetComponent<Player>().clientId)
            {
                return;
            }
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        HideServerRpc();
    }

    public void ThrowWithForce(Vector3 direction, float force)
    {
        rb.AddForce(direction * force);
    }

    public void GravityOn()
    {
        rb.useGravity = true;
    }

    public void GravityOff()
    {
        rb.useGravity = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HideServerRpc()
    {
        throwableVisual.SetActive(false);
        hidden = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        Destroy(gameObject);
    }


}
