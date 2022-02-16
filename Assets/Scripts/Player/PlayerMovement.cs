using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    Camera playerCamera;

    [Header("Walk")]
    [SerializeField] float walkSpeed = 15f;

    [Header("Sprint")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] float sprintMultiplier = 2.1f;

    [Header("Jump")]
    [SerializeField] float jumpforce = 2f;
    [SerializeField] float gravity = -5f;
    float verticalSpeed;

    [Header("Look")]
    [SerializeField] float mouseSensitivityX = 100f;
    [SerializeField] float mouseSensitivityY = 100f;
    float xRotation = 0f;

    void Start()
    {
        //Get Stuff
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked; // Cursor
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        // Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90); // Clamp

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Rotate Camera
        transform.Rotate(Vector3.up * mouseX); // Rotate Player
    }

    void Move()
    {
        // Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Y
        if (characterController.isGrounded)
        {
            verticalSpeed = -1;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalSpeed = jumpforce;
            }
        }
        verticalSpeed += gravity * Time.deltaTime;

        float sprint = Input.GetKey(sprintKey) ? sprintMultiplier : 1; // Sprint

        // X & Y & Z
        Vector3 move = (transform.right * x) + (transform.up * verticalSpeed) + (transform.forward * z);
        characterController.Move(move * walkSpeed * sprint * Time.deltaTime);
    }
}
