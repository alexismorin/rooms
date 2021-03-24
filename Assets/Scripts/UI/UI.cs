using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    [SerializeField]
    Text subtitle;
    [SerializeField]
    Color subtitleColor = Color.white;

    [SerializeField]
    public Text prompt;

    [SerializeField]
    Text objective;
    [SerializeField]
    AnimationCurve animationCurve;


    [SerializeField]
    Text message;

    void Start() {
        message.color = Color.clear;
        subtitle.color = Color.clear;
        objective.color = Color.clear;
        prompt.text = "";
    }


    public IEnumerator PushObjective(string newObjective) {
        objective.text = newObjective;

        float elapsedTime = 0;
        float waitTime = Mathf.Clamp(newObjective.Length / 4.7f, 5f, 15f);

        while (elapsedTime < waitTime) {
            float opacity = animationCurve.Evaluate((elapsedTime / waitTime));
            objective.color = new Color(1f, 1f, 1f, opacity);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objective.color = Color.clear;
        yield return null;
    }

    public IEnumerator PushMessage(string newMessage) {
        message.text = newMessage;

        float elapsedTime = 0;
        float waitTime = Mathf.Clamp(newMessage.Length / 4.7f, 5f, 15f);

        while (elapsedTime < waitTime) {
            float opacity = animationCurve.Evaluate((elapsedTime / waitTime));
            message.color = new Color(1f, 1f, 1f, opacity);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        message.color = Color.clear;
        yield return null;
    }

    public IEnumerator PushSubtitle(string newSubtitle) {

        subtitle.text = newSubtitle;
        subtitle.color = subtitleColor;

        float waitTime = Mathf.Clamp(newSubtitle.Length / 4.7f, 5f, 15f);
        yield return new WaitForSeconds(waitTime);

        subtitle.color = Color.clear;
        yield return null;
    }
}