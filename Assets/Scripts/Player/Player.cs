using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    public Animator rig;

    [Header("Camera")]

    [SerializeField]
    Transform cameraTransform = null;

    [Header("Motion")]

    [SerializeField]
    public Vector2 movementSpeeds = new Vector2(1f, 2f);

    [SerializeField]
    float stamina = 1f;

    [SerializeField]
    float staminaDepletionRate = 0.25f;

    [SerializeField]
    Vector2 movementSensitivity = Vector2.one;

    [Header("Looking")]

    [SerializeField]
    Vector2 lookSensitivity = Vector2.one;

    [SerializeField]
    Vector2 verticalLookLimits = new Vector2(-45f, 45f);

    [Header("Physics")]

    // Internals
    Camera playerCamera;
    CharacterController characterController;
    Vector3 movementVector;

    bool interacting = false;
    float horizontalLook = 0f;
    float verticalLook = 0f;
    [HideInInspector]
    public float movementSpeed = 0f;
    //  IKDelegate iKDelegate;

    void Start() {
        //    iKDelegate = GetComponentInChildren<IKDelegate>();
        characterController = GetComponent<CharacterController>();
        playerCamera = cameraTransform.GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        movementSpeed = movementSpeeds.x;
    }

    void Update() {

        // User Input

        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        //    iKDelegate.velocity = transform.InverseTransformDirection(characterController.velocity);

        if (horizontalMovement + verticalMovement > 0f) {
            //     rig.SetBool("move", true);
        } else {
            //     rig.SetBool("move", false);
        }


        if (!interacting) {
            horizontalLook = Input.GetAxis("Mouse X");
            verticalLook = -Input.GetAxis("Mouse Y");
        } else {
            horizontalLook = 0f;
            verticalLook = 0f;
        }

        // Running

        if (Input.GetButtonDown("Run")) {
            //     rig.SetBool("running", true);
            movementSpeed = movementSpeeds.y;
        }
        if (Input.GetButtonUp("Run")) {
            //      rig.SetBool("running", false);
            movementSpeed = movementSpeeds.x;
        }

        // If we're running, deplete stamina
        if (movementSpeed == movementSpeeds.y) {
            stamina -= staminaDepletionRate * Time.deltaTime;
        }

        if (stamina <= 0) {
            rig.SetBool("running", false);
            movementSpeed = movementSpeeds.x;
            stamina = 0f;
        }

        if (movementSpeed == movementSpeeds.x) {
            stamina += (staminaDepletionRate * 0.5f) * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, 1f);
        }

        // Rotation

        transform.Rotate(new Vector3(0f, horizontalLook * lookSensitivity.x, 0f), Space.Self);
        cameraTransform.Rotate(new Vector3(verticalLook * lookSensitivity.y, 0f, 0f), Space.Self);
        cameraTransform.localEulerAngles = new Vector3(Mathf.Clamp(cameraTransform.localEulerAngles.x, verticalLookLimits.x, verticalLookLimits.y), 0f, 0f);

        // Movement Math

        movementVector = movementSpeed * ((transform.right * movementSensitivity.x * horizontalMovement) + (transform.forward * movementSensitivity.y * verticalMovement));

        // CHEATS

        if (Input.GetKeyDown(KeyCode.L)) {
            staminaDepletionRate = 0f;
        }


    }

    void FixedUpdate() {
        characterController.SimpleMove(movementVector);
    }

    public void OnStartInteract() {
        interacting = true;
    }

    public void OnEndInteract() {
        interacting = false;
    }
}