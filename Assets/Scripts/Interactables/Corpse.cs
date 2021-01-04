using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Interactable {
    public int id;

    // The doors that will open, and the other corpses that will burn upon this corpse's sacrifice.
    public int[] doors;
    public int[] corpses;

    public bool destroyed;

    public bool freeFall;

    Animator animator;
    Rigidbody2D rb;

    new void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Pentagram pentagram = collision.GetComponent<Pentagram>();
        if (collision.GetComponent<Pentagram>() && !destroyed) {
            destroyed = true;
            Burn(collision.GetComponent<Pentagram>());
        } else if (collision.tag == "Level" && freeFall) {
            animator.SetTrigger("Land");
            freeFall = false;
        }
    }

    public override bool CanInteract() {
        if (!player.freeFall && !player.corpse && !destroyed) { return true; }
        return false;
    }

    public override void Interact() {
        player.Pickup();
    }

    public void EnterFreeFall() {
        if (!animator) {
            Start();
        }
        animator.SetTrigger("Free Fall");
        freeFall = true;
        rb.gravityScale = 2;
    }

    void Burn(Pentagram pentagram) {
        StartCoroutine(Burn(0.8f, pentagram));
    }

    public void Decay() {
        gameObject.layer = LayerMask.NameToLayer("Uninteractable");
        animator.SetTrigger("Decay");
    }

    IEnumerator Burn(float seconds, Pentagram pentagram) {
        animator.SetTrigger("Burn");
        pentagram.LightPentagram();

        yield return new WaitForSeconds(seconds);

        GameManager.manager.OpenDoors(doors);
        GameManager.manager.DecayCorpses(corpses);
        Destroy(gameObject);
    }
}
