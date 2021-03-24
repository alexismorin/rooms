using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressableButton : MonoBehaviour {
    PlayerSelection playerSelection;

    [SerializeField]
    GameObject receiver;

    [SerializeField]
    string prompt = "Push Button";

    public void OnStartHover(Transform player) {
        playerSelection = player.GetComponent<PlayerSelection>();
        playerSelection.PushPrompt(prompt);
    }

    public void OnEndHover(Transform player) {
        playerSelection.ClearPrompt();
    }

    public void OnStartInteract(Transform player) {

        playerSelection.ClearPrompt();

        if (receiver) {
            receiver.SendMessage("RemoteInteract", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnEndInteract(Transform player) {

    }
}