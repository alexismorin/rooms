using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialogue;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class DialogueSystem : MonoBehaviour {

    [SerializeField]
    DialogueGraph testdialogue;

    [Header("Rendering")]

    [SerializeField]
    GameObject templateButton = null;

    [SerializeField]
    GameObject textBox;
    Text text;
    Text nextText;

    // Internals

    DialogueGraph graph;
    bool inDialogue;
    List<GameObject> currentButtons = new List<GameObject>();

    void Start() {
        text = textBox.transform.GetChild(0).GetComponent<Text>();
        nextText = textBox.transform.GetChild(1).GetComponent<Text>();

        textBox.SetActive(false);
        templateButton.SetActive(false);

        ProcessDialogue(testdialogue);
    }

    void ProcessDialogue(DialogueGraph conversation) {
        graph = conversation;
        graph.Restart();
        inDialogue = true;

        UpdateDialogue();
    }

    void UpdateDialogue() {
        textBox.SetActive(true);
        text.text = graph.current.text;

        if (graph.current.answers.Count == 0) {
            nextText.enabled = true;
        } else {
            nextText.enabled = false;
        }



        // Remove old buttons
        for (int i = 0; i < currentButtons.Count; i++) {
            Destroy(currentButtons[i]);
        }

        // Add new buttons
        for (int i = 0; i < graph.current.answers.Count; i++) {
            GameObject buttonInstance = Instantiate(templateButton, templateButton.transform.position, Quaternion.identity, templateButton.transform.parent);
            currentButtons.Add(buttonInstance);

            buttonInstance.SetActive(true);
            buttonInstance.transform.GetChild(0).GetComponent<Text>().text = graph.current.answers[i].text;
            int currentAnswer = i;
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => UpdateDialogueState(currentAnswer));
        }
    }

    public void UpdateDialogueState(int reply) {
        graph.AnswerQuestion(reply);
        UpdateDialogue();
    }

    public void Update() {
        if (inDialogue) {
            if (Input.GetButtonDown("Fire1")) {

                if (graph.current.endConversation) {
                    inDialogue = false;
                } else {

                    if (graph.current.answers.Count == 0) {
                        UpdateDialogueState(0);
                    }
                }

            }
        }
    }


}