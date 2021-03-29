using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class TalkableNPC : MonoBehaviour
{

    [SerializeField]
    Dialogue.CharacterInfo character;
    [SerializeField]
    DialogueGraph dialogue;

    bool inDialogue = false;
    PlayerSelection playerSelection;

    public void OnStartHover(Transform player)
    {
        if (!inDialogue)
        {
            playerSelection = player.GetComponent<PlayerSelection>();
            playerSelection.PushPrompt("Talk to " + character.name);
        }
    }

    public void OnEndHover(Transform player)
    {
        playerSelection.ClearPrompt();
    }

    public void OnStartInteract(Transform player)
    {
        if (!inDialogue)
        {
            playerSelection.ClearPrompt();
            playerSelection.dialogueSystemReference.ProcessDialogue(dialogue, this.gameObject);
            inDialogue = true;
        }
    }

    public void OnEndInteract(Transform player)
    {
    }


    public void EndDialogue()
    {
        Invoke("ResetDialogue", 0.5f);
    }

    void ResetDialogue()
    {
        inDialogue = false;
    }

}
