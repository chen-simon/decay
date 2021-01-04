using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedPentagram : Pentagram {
    public GameObject slosher;
    public Transform sloshers;

    public override void LightPentagram() {
        base.LightPentagram();
        GameManager.manager.FlashScreen();
        SpawnSlosher();
    }

    public override void PerformRitual() {
        player.PerformFailedRitual();
    }

    void SpawnSlosher() {
        Instantiate(slosher, transform.position, Quaternion.identity, sloshers);
    }
}
