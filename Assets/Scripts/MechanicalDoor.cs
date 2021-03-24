using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalDoor : MonoBehaviour {

    [SerializeField]
    Transform[] transforms;

    [SerializeField]
    Transform doorTransform;

    [SerializeField]
    AnimationCurve animationCurve;

    [SerializeField]
    float doorOpenTime = 4f;

    [SerializeField]
    bool opened;
    bool moving;

    public void RemoteInteract() {
        if (!moving) {
            if (opened) {
                StartCoroutine(Gate(false));
            } else {
                StartCoroutine(Gate(true));
            }
        }
    }

    public IEnumerator Gate(bool open) {

        moving = true;

        float lerpProgress = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < doorOpenTime) {
            lerpProgress = animationCurve.Evaluate((elapsedTime / doorOpenTime));

            if (open == true) {
                doorTransform.position = Vector3.Lerp(transforms[0].position, transforms[1].position, lerpProgress);
                doorTransform.rotation = Quaternion.Lerp(transforms[0].rotation, transforms[1].rotation, lerpProgress);
            }
            if (open == false) {
                doorTransform.position = Vector3.Lerp(transforms[0].position, transforms[1].position, 1f - lerpProgress);
                doorTransform.rotation = Quaternion.Lerp(transforms[0].rotation, transforms[1].rotation, 1f - lerpProgress);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (open) {
            opened = true;
        } else {
            opened = false;
        }

        moving = false;
        yield return null;
    }
}