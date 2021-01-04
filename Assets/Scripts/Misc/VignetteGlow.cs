using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteGlow : MonoBehaviour {

    PostProcessVolume volume;

    public float minWeight = 0.7f;
    public float maxWeight = 0.9f;
    public float speed = 1;

    float shift;
    float amplitude;

    void Start() {
        volume = GetComponent<PostProcessVolume>();
        amplitude = (maxWeight - minWeight) / 2;
        shift = (maxWeight + minWeight) / 2;
    }

    void Update() {
        volume.weight = amplitude * Mathf.Sin(Time.time * speed) + shift;
    }
}
