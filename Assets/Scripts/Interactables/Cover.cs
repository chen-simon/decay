using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Interactable {

    public override bool CanInteract() {
        if (!player.freeFall && !player.isHiding) { return true; }
        return false;
    }

    public override void Interact() {
        player.Hide();
    }
}
