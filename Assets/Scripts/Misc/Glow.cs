using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    public float minAlpha = 0.7f;
    public float maxAlpha = 0.9f;
    public float speed = 1;

    float shift;
    float amplitude;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        amplitude = (maxAlpha - minAlpha) / 2;
        shift = (maxAlpha + minAlpha) / 2;
    }

    void Update() {
        Color color = spriteRenderer.color;
        color.a = amplitude * Mathf.Sin(Time.time * speed) + shift;
        spriteRenderer.color = color;
    }
}
