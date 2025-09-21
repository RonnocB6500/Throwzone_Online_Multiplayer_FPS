using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform cameraHolder;

    private Vector3 cameraNormal;

    public float sensX;
    public float sensY;

    float xRotation;
    float yRotation;

    private Vector3 deathCameraStartPosition;
    private float camTimePos = 0f;
    private float camTimeRot = 0f;

    private void Start()
    {
        ThrowGameManager.Instance.OnLocalGamePaused += KitchenGameManager_OnLocalGamePaused;
        ThrowGameManager.Instance.OnLocalGameUnpaused += KitchenGameManager_OnLocalGameUnpaused;

        cameraNormal = new Vector3();

        deathCameraStartPosition = new Vector3(0f, 1.9f, -0.7f);

        LockMouse();
    }

    private void KitchenGameManager_OnLocalGameUnpaused(object sender, System.EventArgs e)
    {
        LockMouse();
    }

    private void KitchenGameManager_OnLocalGamePaused(object sender, System.EventArgs e)
    {
        UnlockMouse();
    }

    private void Update()
    {
        cameraNormal = transform.forward;
    }

    private void FixedUpdate()
    {
        if (player.gameObject.GetComponent<PlayerHealth>().dead == false)
        {
            camTimePos = 0f;
            camTimeRot = 0f;
            transform.localPosition = new Vector3(0,1.4f,0.45f);
            MoveCamera();
        }
        else if (player.gameObject.GetComponent<PlayerHealth>().dead == true)
        {
            //if (time < 1)
            //{
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(30, 0, 0), camTimeRot);
            transform.localPosition = Vector3.MoveTowards(deathCameraStartPosition, new Vector3(0, 4, -4), camTimePos);
            camTimePos += Time.deltaTime * 0.5f;
            camTimeRot += Time.deltaTime * 0.01f;
            //Debug.Log(transform.localPosition);
            //}
        }
    }

    private void MoveCamera()
    {
        // get mouse input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate cam and orientation
        player.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        Vector3 offset = new Vector3(0, 1.4f, 0.45f);            

        // Get the forward vector of the camera's transform
        cameraNormal = transform.forward;

        // You can use this vector for various purposes, e.g., raycasting, movement
        Debug.DrawRay(transform.position, cameraNormal * 10f, Color.blue);

        //transform.position = player.transform.position + offset;
    }

    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public Vector3 GetCameraNormalVector()
    {
        return cameraNormal;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
