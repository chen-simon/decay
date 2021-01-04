using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    static float walkSpeed = 1.25f;
    static float runSpeed = 6.5f;
    static float strikingRange = 2.5f;
    static float hearingRange = 10f;

    public bool isChasing;
    public bool isShrieking;
    public bool walkingRight = true;

    bool killed;

    Animator animator;
    SpriteRenderer spriteRenderer;
    AudioController audioController;

    public LayerMask colliderMask;
    public EnemyDetection detector;

    void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioController = GetComponent<AudioController>();

        StartCoroutine(Step(1.35f, 1 / walkSpeed, walkingRight));
    }

    void Update() {
        if (isChasing) {
            if (Vector2.Distance(transform.position, PlayerController.controller.transform.position) < strikingRange && !killed) {
                killed = true;
                GameManager.manager.GameOver();
                audioController.PlaySound("Slash");
                StopAllCoroutines();
            }
        } else if (!isShrieking && PlayerController.controller.makingNoise) {
            if (Vector2.Distance(transform.position, PlayerController.controller.transform.position) < hearingRange) {
                if (PlayerController.controller.transform.position.x < transform.position.x && walkingRight) {
                    walkingRight = false;
                    spriteRenderer.flipX = true;
                    detector.Flip(walkingRight);
                } else if (PlayerController.controller.transform.position.x > transform.position.x && !walkingRight) {
                    walkingRight = true;
                    spriteRenderer.flipX = false;
                    detector.Flip(walkingRight);
                }
            }
        }

        if (!isShrieking) {
            if (walkingRight) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position - Vector3.up, Vector2.right, 1.4f, colliderMask);
                if (hit.collider != null) {
                    walkingRight = false;
                    spriteRenderer.flipX = true;
                    detector.Flip(walkingRight);
                    if (isChasing) {
                        isChasing = false;
                    }
                }
            } else {
                RaycastHit2D hit = Physics2D.Raycast(transform.position - Vector3.up, Vector2.left, 1.4f, colliderMask);
                if (hit.collider != null) {
                    walkingRight = true;
                    spriteRenderer.flipX = false;
                    detector.Flip(walkingRight);
                    if (isChasing) {
                        isChasing = false;
                    }
                }
            }
        }
    }

    public void Shriek() {
        isShrieking = true;
        StopAllCoroutines();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().Shake();
        audioController.PlaySound("Shriek");
        StartCoroutine(Shriek(2.4f));
    }

    IEnumerator Step(float distance, float time, bool isRight) {
        animator.SetTrigger("Step");
        float travelledDistance = 0;
        while (travelledDistance < distance) {
            travelledDistance += (distance/time) * Time.deltaTime;
            if (isRight) {
                transform.position += Vector3.right * (distance/time) * Time.deltaTime;
            } else {
                transform.position += Vector3.left * (distance/time) * Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        if (isChasing) {
            StartCoroutine(Slide(0.4f, 1 / runSpeed, walkingRight));
        } else {
            StartCoroutine(Slide(0.4f, 1 / walkSpeed, walkingRight));
        }
        audioController.PlaySound("Slosh 2");
    }

    IEnumerator Slide(float distance, float time, bool isRight) {
        float travelledDistance = 0;
        while (travelledDistance < distance) {
            travelledDistance += (distance / time) * Time.deltaTime;
            if (isRight) {
                transform.position += Vector3.right * (distance / time) * Time.deltaTime;
            } else {
                transform.position += Vector3.left * (distance / time) * Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        if (isChasing) {
            StartCoroutine(Step(1.35f, 1 / runSpeed, walkingRight));
        } else {
            StartCoroutine(Step(1.35f, 1 / walkSpeed, walkingRight));
        }
    }

    IEnumerator Shriek(float seconds) {
        animator.SetTrigger("Shriek");
        if (PlayerController.controller.transform.position.x < transform.position.x) {
            spriteRenderer.flipX = true;
        } else {
            spriteRenderer.flipX = false;
        }
        yield return new WaitForSeconds(seconds);
        isShrieking = false;
        isChasing = true;
        if (PlayerController.controller.transform.position.x < transform.position.x) {
            walkingRight = false;
            spriteRenderer.flipX = true;
            detector.Flip(walkingRight);
        } else {
            walkingRight = true;
            spriteRenderer.flipX = false;
            detector.Flip(walkingRight);
        }
        StartCoroutine(Step(1.2f, 1 / runSpeed, walkingRight));
    }
}
