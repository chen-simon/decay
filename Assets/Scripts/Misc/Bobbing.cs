using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    public float minHeight = -0.1f;
    public float maxHeight = 0.1f;
    public float speed = 1;

    float shift;
    float amplitude;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        amplitude = (maxHeight - minHeight) / 2;
        shift = (maxHeight + minHeight) / 2;
    }

    void LateUpdate() {
        Vector2 pos = transform.localPosition;
        pos.y = amplitude * Mathf.Sin(Time.time * speed) + shift;
        transform.localPosition = pos;
    }
}