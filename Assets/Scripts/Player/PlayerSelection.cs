using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour {

    Transform currentTarget;
    bool interacting;
    string currentObjective;
    AudioSource larynx;

    Coroutine objectiveCoroutine;
    Coroutine messageCoroutine;
    Coroutine subtitleCoroutine;

    [SerializeField]
    public Transform cameraTransform = null;

    [SerializeField]
    float interactDistance = 2f;

    [SerializeField]
    public List<InventoryItem> inventory = new List<InventoryItem>();

    [SerializeField]
    public List<InventoryItem> gameplayTags = new List<InventoryItem>();

    [SerializeField]
    public UI uiReference;

    [SerializeField]
    LayerMask layerMask;

    void Start() {
        larynx = GetComponent<AudioSource>();
    }

    void Update() {

        // cast rays to look for stuff
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactDistance, layerMask)) {

            if (!currentTarget) {
                hit.transform.SendMessage("OnStartHover", this.transform, SendMessageOptions.DontRequireReceiver);
                currentTarget = hit.transform;
            } else {

                if (hit.transform != currentTarget) {
                    // Stop what we were doing
                    if (interacting) {
                        currentTarget.SendMessage("OnEndInteract", this.transform, SendMessageOptions.DontRequireReceiver);
                        interacting = false;
                    }
                    currentTarget.SendMessage("OnEndHover", this.transform, SendMessageOptions.DontRequireReceiver);
                    currentTarget = null;

                    // Hover over new
                    hit.transform.SendMessage("OnStartHover", this.transform, SendMessageOptions.DontRequireReceiver);
                    currentTarget = hit.transform;
                }

            }
        } else {
            if (currentTarget) {
                if (interacting) {
                    currentTarget.SendMessage("OnEndInteract", this.transform, SendMessageOptions.DontRequireReceiver);
                    interacting = false;
                }
                currentTarget.SendMessage("OnEndHover", this.transform, SendMessageOptions.DontRequireReceiver);
                currentTarget = null;
            }
        }

        // if a target is in out midst, we can interact with it
        if (currentTarget) {

            if (!interacting) {
                if (Input.GetButtonDown("Fire1")) {
                    currentTarget.SendMessage("OnStartInteract", this.transform, SendMessageOptions.DontRequireReceiver);
                    interacting = true;
                }
            }
            if (interacting) {
                if (Input.GetButtonUp("Fire1")) {
                    currentTarget.SendMessage("OnEndInteract", this.transform, SendMessageOptions.DontRequireReceiver);
                    interacting = false;
                }
            }

        }
    }

    public void PushAudio(AudioClip audioClip, float volume = 1f) {
        larynx.PlayOneShot(audioClip, volume);
    }

    public bool TryItem(InventoryItem item) {
        if (inventory.Contains(item)) {
            return true;
        } else {
            return false;
        }
    }

    public bool TryAddItem(InventoryItem item) {
        if (!inventory.Contains(item)) {
            inventory.Add(item);
            uiReference.PushMessage("Picked up " + item.name);
            return true;
        } else {
            return false;
        }
    }

    public bool TryRemoveItem(InventoryItem item) {
        if (inventory.Contains(item)) {
            inventory.Remove(item);
            return true;
        } else {
            return false;
        }
    }

    public void PushObjective(string newObjective) {
        currentObjective = newObjective;
        if (objectiveCoroutine != null) {
            StopCoroutine(objectiveCoroutine);
        }

        objectiveCoroutine = StartCoroutine(uiReference.PushObjective(newObjective));
    }

    public void PushSubtitle(string newSubtitle) {
        if (subtitleCoroutine != null) {
            StopCoroutine(subtitleCoroutine);
        }

        subtitleCoroutine = StartCoroutine(uiReference.PushSubtitle(newSubtitle));
    }

    public void PushMessage(string newMessage) {
        if (messageCoroutine != null) {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(uiReference.PushMessage(newMessage));
    }

    // Force-Clear a prompt
    public void ClearPrompt() {
        uiReference.prompt.text = "";
    }

    public void PushPrompt(string prompt) {
        uiReference.prompt.text = prompt;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        hit.gameObject.SendMessage("Bash", GetComponent<Player>(), SendMessageOptions.DontRequireReceiver);
    }

}