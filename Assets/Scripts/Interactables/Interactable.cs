using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    protected PlayerController player;
    public Vector2 indicatorOffset;

    public void Start() {
        player = PlayerController.controller;
    }

    public virtual void Interact() {
        throw new UnityException();
    }

    public virtual bool CanInteract() {
        throw new UnityException();
    }

    public virtual Vector2 indicatorPosition() {
        return (Vector2)transform.position + indicatorOffset;
    }
}
