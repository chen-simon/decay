using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Interactable {
    public override bool CanInteract() {
        return true;
    }

    public override void Interact() {
        GameManager.manager.AxeSection();
    }
}
