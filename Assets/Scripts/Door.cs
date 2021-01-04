using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public int id;

    new Collider2D collider;
    SpriteRenderer spriteRenderer;

    private void Start() {
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Open() {
        spriteRenderer.enabled = false;
        collider.enabled = false;
    }

    public void Close() {

    }
}
