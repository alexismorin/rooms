using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayTrigger : MonoBehaviour {

    [SerializeField]
    bool doOnce = true;
    [SerializeField]
    InventoryItem conditionalItem;
    [SerializeField]
    float delay = 0f;

    [Header("Object States")]
    [SerializeField]
    List<GameObject> toEnable = new List<GameObject>();
    [SerializeField]
    List<GameObject> toDisable = new List<GameObject>();
    [SerializeField]
    GameObject triggerMethod = null;

    [Header("Notifications")]

    [SerializeField]
    string pushObjective = "";
    [TextArea]
    [SerializeField]
    string pushSubtitle = "";
    [SerializeField]
    string pushMessage = "";
    [SerializeField]
    string pushPrompt = "";
    [Space(10)]
    [SerializeField]
    AudioClip sound;
    [SerializeField]
    float soundVolume = 1f;

    // Internals

    bool done;
    PlayerSelection playerSelection;


    private void OnTriggerEnter(Collider other) {
        if (!done) {
            if (other.gameObject.tag == "Player") {

                playerSelection = other.gameObject.GetComponent<PlayerSelection>();

                if (conditionalItem) {
                    if (playerSelection.TryItem(conditionalItem)) {
                        Invoke("Interaction", delay);
                    }
                } else {
                    Invoke("Interaction", delay);
                }


            }
        }
    }

    void Interaction() {

        // Object states
        foreach (var item in toEnable) {
            item.SetActive(true);
        }
        foreach (var item in toDisable) {
            item.SetActive(false);
        }

        if (triggerMethod) {
            triggerMethod.SendMessage("TriggerMethod", SendMessageOptions.DontRequireReceiver);
        }


        // UI Pushes
        if (pushObjective != "") {
            playerSelection.PushObjective(pushObjective);
        }
        if (pushSubtitle != "") {
            playerSelection.PushSubtitle(pushSubtitle);
        }
        if (pushMessage != "") {
            playerSelection.PushMessage(pushMessage);
        }
        if (pushPrompt != "") {
            playerSelection.PushPrompt(pushPrompt);
        }

        // Audio
        if (sound) {
            playerSelection.PushAudio(sound, soundVolume);
        }

        // Ending
        if (doOnce) {
            done = true;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(0f, 0f, 1f, 0.25f);
        if (triggerMethod) {
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        }
        if (pushMessage != "") {
            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(new Vector3(0f, transform.localScale.y * 0.23f, 0f), Vector3.one);
    }

}