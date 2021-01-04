using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : Interactable {

    public GameObject[] accessPoints;

    [HideInInspector]
    public float minHeight;
    [HideInInspector]
    public float maxHeight;

    new void Start() {
        base.Start();
        float lowestSoFar = Mathf.Infinity;
        float highestSoFar = Mathf.NegativeInfinity;

        foreach (GameObject o in accessPoints) {
            if (o.transform.position.y < lowestSoFar) { lowestSoFar = o.transform.position.y; }
            if (o.transform.position.y > highestSoFar) { highestSoFar = o.transform.position.y; }
        }

        minHeight = lowestSoFar;
        maxHeight = highestSoFar;
    }

    public override bool CanInteract() {
        if (!player.freeFall) { return true; }
        return false;
    }

    public override void Interact() {
        player.ClimbLadder();
    }

    public override Vector2 indicatorPosition() {
        return (Vector2)ClosestAccessPointByHeight(accessPoints).transform.position + indicatorOffset;
    }

    GameObject ClosestAccessPointByHeight(GameObject[] points) {
        float shortestDistance = Mathf.Infinity;
        GameObject closestSoFar = null;
        foreach (GameObject p in points) {
            float distance = Mathf.Abs(player.transform.position.y - p.transform.position.y);
            if (distance < shortestDistance) {
                closestSoFar = p;
                shortestDistance = distance;
            }
        }
        return closestSoFar;
    }

}
