using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceReflectionProbe : MonoBehaviour {

    public bool refresh = false;
    // Start is called before the first frame update
    void OnValidate() {
        transform.position = transform.parent.GetComponent<MeshRenderer>().bounds.center;
        ReflectionProbe reflectionProbe = GetComponent<ReflectionProbe>();
        reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
        reflectionProbe.boxProjection = true;
        reflectionProbe.resolution = 32;
        reflectionProbe.blendDistance = 0.1f;

        reflectionProbe.size = transform.parent.GetComponent<MeshRenderer>().bounds.size * 1.1f;
        refresh = false;
    }

    void Start() {
        Invoke("Bake", 1f);
    }

    void Bake() {
        GetComponent<ReflectionProbe>().size = transform.parent.GetComponent<MeshRenderer>().bounds.size * 1.1f;
        GetComponent<ReflectionProbe>().RenderProbe();
    }

}