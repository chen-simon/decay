using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour {
    public EnemyController controller;

    new Collider2D collider;

    private void Start() {
        collider = GetComponent<Collider2D>();
    }

    public void Flip(bool isRight) {
        int sign = 1;
        if (!isRight) { sign = -1; }
        collider.offset = new Vector2(sign * Mathf.Abs(collider.offset.x), collider.offset.y);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Player" && !controller.isChasing && !controller.isShrieking) {
            controller.Shriek();
        }
    }
}
