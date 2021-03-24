using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {


    bool interacting;
    float doorState = 0f;

    float smoothTime = 0.3f;
    float yVelocity = 0.0f;
    float input = 0;
    PlayerSelection playerSelection;

    [SerializeField]
    bool canRamInto = true;

    [Space(10)]

    [SerializeField]
    float inputSensitivity = 2f;
    [SerializeField]
    bool blockMouselook = true;

    [Space(10)]

    [SerializeField]
    bool locked;
    [SerializeField]
    InventoryItem keyItem;

    [Space(10)]

    [SerializeField]
    bool onlyOpensOnOneSide = true;

    void Bash(Player player) {

        if (canRamInto) {
            if (player.movementSpeed == player.movementSpeeds.y) {
                StartCoroutine(SlamOpen());
            }
        }



    }

    IEnumerator SlamOpen() {

        if (playerSelection) {

            playerSelection.ClearPrompt();
            playerSelection.gameObject.GetComponent<Player>().rig.SetTrigger("bash");


            float elapsedTime = 0;
            float waitTime = 0.1f;

            while (elapsedTime < waitTime) {
                float state = Mathf.Lerp(0f, 90f, Mathf.SmoothStep(0f, 1f, elapsedTime / waitTime));
                transform.localEulerAngles = new Vector3(0f, state, 0f);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
            playerSelection.transform.GetComponent<Player>().OnEndInteract();
            doorState = 1f;

            playerSelection.gameObject.GetComponent<Player>().rig.ResetTrigger("bash");

            Destroy(this);
        }

    }

    public void OnStartHover(Transform player) {

        playerSelection = player.GetComponent<PlayerSelection>();

        if (Vector3.Dot(playerSelection.cameraTransform.forward, transform.right) < 0f) {
            playerSelection.PushPrompt("Close Door");
        } else {
            playerSelection.PushPrompt("Open Door");
        }



    }

    public void OnEndHover(Transform player) {
        playerSelection.ClearPrompt();

    }

    public void OnStartInteract(Transform player) {

        playerSelection.ClearPrompt();

        // If door has never been opened, it can only be opened from one side
        if (onlyOpensOnOneSide && doorState == 0) {
            if (Vector3.Dot(playerSelection.cameraTransform.forward, transform.right) < 0f) {
                playerSelection.PushMessage("Does not open from this side.");
                return;
            }
        }

        if (locked) {
            if (playerSelection.TryRemoveItem(keyItem)) {
                playerSelection.PushMessage("Unlocked.");
                locked = false;
            }
        }

        if (!locked) {
            interacting = true;
            if (blockMouselook) {
                player.GetComponent<Player>().OnStartInteract();
            }
        } else {
            if (keyItem) {
                playerSelection.PushMessage("Locked, " + keyItem.name + " Required.");
            } else {
                playerSelection.PushMessage("Locked.");
            }

        }


    }

    public void OnEndInteract(Transform player) {
        if (!locked) {
            interacting = false;
            if (blockMouselook) {
                player.GetComponent<Player>().OnEndInteract();
            }
        }

    }

    void Update() {
        if (interacting) {

            // Account for closing doors on the "wrong" side.
            float inputFlip = 1;
            if (Vector3.Dot(playerSelection.cameraTransform.forward, transform.right) < 0f) {
                inputFlip = -1f;
            }



            float smoothedInput = Mathf.SmoothDamp(input, Input.GetAxis("Mouse Y") * inputFlip, ref yVelocity, smoothTime);
            input = smoothedInput;

            doorState += input * inputSensitivity;
            doorState = Mathf.Clamp(doorState, 0f, 1f);
        } else {

            float smoothedInput = Mathf.SmoothDamp(input, 0f, ref yVelocity, smoothTime);
            input = smoothedInput;
            doorState += input * inputSensitivity;
            doorState = Mathf.Clamp(doorState, 0f, 1f);
        }

        // Rotate the door accordingly
        transform.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0f, 90f, 0f), doorState);
    }
}