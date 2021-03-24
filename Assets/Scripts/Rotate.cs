using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    [SerializeField]
    float maxSpeed = 3f;

    void Start() {
        maxSpeed = Random.Range(0.1f, maxSpeed);
    }

    void Update() {
        transform.Rotate((Vector3.up * maxSpeed) * Time.deltaTime, Space.Self);
    }
}