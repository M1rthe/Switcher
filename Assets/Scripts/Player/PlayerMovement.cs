using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    Camera playerCamera;

    [Header("Walk")]
    [SerializeField] float walkSpeed = 15f;
    Vector3 move;
    Vector3 lastMove;

    [Header("Sprint")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] float sprintMultiplier = 2.1f;

    [Header("Jump")]
    [SerializeField] float jumpforce = 1.2f;
    [SerializeField] float jumpHeighForce = 1.5f;
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
        // X & Z
        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.up) + (transform.forward * Input.GetAxis("Vertical"));

        //Y
        if (characterController.isGrounded)
        {
            //Jumping
            verticalSpeed = -1f;
            if (Input.GetKeyDown(KeyCode.Space)) verticalSpeed = jumpforce;
        }
        else
        {
            //Falling
            verticalSpeed += gravity * Time.deltaTime;
            move = lastMove;
        }

        float sprint = Input.GetKey(sprintKey) ? sprintMultiplier : 1; // Sprint

        move.y = verticalSpeed * jumpHeighForce;
        characterController.Move(move * walkSpeed * sprint * Time.deltaTime);
        lastMove = move;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (characterController.isGrounded || hit.normal.y > 0.1f) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        if (hit.transform.CompareTag("Slippery")) return;

        move = hit.normal * 0.6f;
        verticalSpeed = jumpforce * 1.5f;
    }
}
