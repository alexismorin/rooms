using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    PlayerSelection playerSelection;

    [SerializeField]
    InventoryItem item;

    public void OnStartHover(Transform player) {
        playerSelection = player.GetComponent<PlayerSelection>();


        playerSelection.PushPrompt("Pick up " + item.name);

    }

    public void OnEndHover(Transform player) {
        playerSelection.ClearPrompt();
    }

    public void OnStartInteract(Transform player) {

        playerSelection.ClearPrompt();

        if (playerSelection.TryAddItem(item)) {
            playerSelection.PushMessage("Picked up " + item.name);
            Destroy(gameObject);
        }

    }

    public void OnEndInteract(Transform player) {

    }
}