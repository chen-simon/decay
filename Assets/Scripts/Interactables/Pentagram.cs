using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pentagram : Interactable {
    public GameObject[] flames;

    protected int sacrifices = 0;

    new void Start() {
        base.Start();
    }

    void Update() {
        
    }

    public override bool CanInteract() {
        if (!player.freeFall && player.corpse) { return true; } 
        // Perform Ritual when 5 sacrifices 
        else if (!player.freeFall && !player.corpse && sacrifices >= 5) { return true; }
        return false;
    }

    public override void Interact() {
        if (player.corpse) {
            player.Place();
        } 
        // Perform Ritual
        else { PerformRitual(); }
    }

    public virtual void LightPentagram() {
        flames[sacrifices].SetActive(true);
        sacrifices += 1;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().Shake();
        GameManager.manager.burnSound.Play();
    }

    public virtual void PerformRitual() {
        if (sacrifices < 6) { player.PerformFailedRitual(); }
        else { player.PerformSuccessfulRitual(); }
    }
}
