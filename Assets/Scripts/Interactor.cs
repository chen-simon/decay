using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour {
    ArrayList interactables;

    public GameObject indicator;
    public GameObject currentInteractable;

    public bool withLadder;

    PlayerController player;

    private void Start() {
        player = PlayerController.controller;
    }
    void FixedUpdate() {
        withLadder = false;
        interactables = new ArrayList(0);
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.GetComponent<Interactable>() != null) {
            interactables.Add(collision.gameObject);
        }
        if (collision.tag == "Ladder") {
            withLadder = true;
        }
    }

    void Update() {
        indicator.SetActive(false);
        if (!player.isHiding && !player.onLadder) {
            if (interactables.Count > 0 && player.canMove) {
                currentInteractable = ClostestInteractableObject((GameObject[])interactables.ToArray(typeof(GameObject)));
                if (currentInteractable) {
                    indicator.SetActive(true);
                    indicator.transform.position = currentInteractable.GetComponent<Interactable>().indicatorPosition();
                }
            } else {
                currentInteractable = null;
            }
        }
    }

    public void RemoveInteractable() {
        interactables.Remove(currentInteractable);
    }

    GameObject ClostestInteractableObject(GameObject[] objects) {
        float shortestDistance = Mathf.Infinity;
        GameObject closestSoFar = null;
        foreach (GameObject o in objects) {
            if (o && o.GetComponent<Interactable>().CanInteract()) {
                float distance = Vector2.Distance(o.transform.position, player.transform.position);
                if (distance < shortestDistance) {
                    closestSoFar = o;
                    shortestDistance = distance;
                }
            }
        }
        return closestSoFar;
    }
}
