using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialogue;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class DialogueSystem : MonoBehaviour
{

    [SerializeField]
    DialogueGraph testdialogue;

    [Header("Rendering")]

    [SerializeField]
    GameObject templateButton = null;

    [SerializeField]
    GameObject textBox;
    Text text;
    Text nextText;
    Text nameText;

    // Internals

    DialogueGraph graph;
    bool inDialogue;
    List<GameObject> currentButtons = new List<GameObject>();

    void Start()
    {
        nameText = textBox.transform.GetChild(0).GetComponent<Text>();
        text = textBox.transform.GetChild(1).GetComponent<Text>();
        nextText = textBox.transform.GetChild(2).GetComponent<Text>();

        textBox.SetActive(false);
        templateButton.SetActive(false);

        ProcessDialogue(testdialogue);
    }

    void ProcessDialogue(DialogueGraph conversation)
    {
        graph = conversation;
        graph.Restart();
        inDialogue = true;

        UpdateDialogue();
    }

    void UpdateDialogue()
    {
        textBox.SetActive(true);
        text.text = graph.current.text;

        nameText.text = graph.current.character.name;
        nameText.color = graph.current.character.color;

        if (graph.current.answers.Count == 0)
        {
            nextText.enabled = true;
        }
        else
        {
            nextText.enabled = false;
        }

        // Remove old buttons
        for (int i = 0; i < currentButtons.Count; i++)
        {
            Destroy(currentButtons[i]);
        }

        // Add new buttons
        for (int i = 0; i < graph.current.answers.Count; i++)
        {
            GameObject buttonInstance = Instantiate(templateButton, templateButton.transform.position, Quaternion.identity, templateButton.transform.parent);
            currentButtons.Add(buttonInstance);

            buttonInstance.SetActive(true);
            buttonInstance.transform.GetChild(0).GetComponent<Text>().text = graph.current.answers[i].text;
            int currentAnswer = i;
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => UpdateDialogueState(currentAnswer));
        }

    }

    public void UpdateDialogueState(int reply)
    {
        graph.AnswerQuestion(reply);
        UpdateDialogue();
    }

    void EndDialogue()
    {
        inDialogue = false;
        textBox.SetActive(false);
        templateButton.SetActive(false);
    }

    public void Update()
    {
        if (inDialogue)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // If this is a node with no answer, we're either a no-reply node, or the end
                if (graph.current.answers.Count == 0)
                {
                    foreach (NodePort port in graph.current.Ports)
                    {
                        if (port.fieldName == "output")
                        {
                            if (port.GetConnections().Count == 0)
                            {
                                // End the dialogue
                                EndDialogue();
                            }
                            else
                            {
                                // This is a text-only graph
                                UpdateDialogueState(0);
                            }
                        }
                    }
                }
            }
        }
    }




}