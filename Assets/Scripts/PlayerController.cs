using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class PlayerController : MonoBehaviour {

    public static PlayerController controller;

    // These are the base movement values for the player.
    static float walkSpeed = 2.05f;
    static float airSpeed = 2.8f;
    static float runSpeed = 3.5f;
    static float haulSpeed = 1.6f;
    static float ladderSpeed = 3.25f;
    static float ladderHaulSpeed = 2f;

    float speed = walkSpeed;

    public bool canMove;

    public bool isHiding;
    public bool isGrounded;
    public bool onLadder;

    bool isStepping;

    public bool makingNoise;

    public Corpse corpse;

    public Transform corpses;
    public GameObject corpseObject;

    new Collider2D collider;
    Collider2D groundCheckCollider;

    public LayerMask colliderMask;

    Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Interactor interactor;
    Animator animator;

    public bool inWater;

    public AudioClip stepLand;
    public AudioClip stepWater;

    public AudioSource stepSource;

    float maxHeight;
    float minHeight;

    public bool freeFall;

    void Awake() {
        // Singleton
        if (controller) {
            Destroy(gameObject);
        } else {
            controller = this;
        }
    }

    void Start() {
        collider = GetComponent<CapsuleCollider2D>();
        groundCheckCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactor = GetComponentInChildren<Interactor>();

        WakeUp();
    }

    void FixedUpdate() {
        makingNoise = false;
        isGrounded = false;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isClimbing", false);
        if (canMove) {
            if (!onLadder) {
                float horizontal = 0;
                if (Input.GetKey(KeyCode.A)) { horizontal -= 1; }
                if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.D)) { horizontal += 1; }
                if (horizontal != 0) {
                    animator.SetBool("isWalking", true);
                    if (horizontal > 0) {
                        spriteRenderer.flipX = false;
                    } else {
                        spriteRenderer.flipX = true;
                    }
                }

                if (corpse) {
                    speed = haulSpeed;
                } else if (Input.GetKey(KeyCode.LeftShift) && horizontal != 0 && !freeFall) {
                    speed = runSpeed;
                    animator.SetBool("isRunning", true);
                    if (!isStepping) { StartCoroutine(Step()); }
                    makingNoise = true;
                } else if (freeFall) {
                    speed = airSpeed;
                } else {
                    speed = walkSpeed;
                }

                transform.position += Vector3.right * horizontal * speed * Time.deltaTime;

            } else {
                float vertical = 0;
                if (Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.W)) { vertical += 1; }
                if (Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.S)) { vertical -= 1; }
                if (vertical != 0) { animator.SetBool("isClimbing", true); }

                float climbSpeed = ladderSpeed;
                if (corpse) { climbSpeed = ladderHaulSpeed; }

                Vector3 pos = transform.position + Vector3.up * vertical * climbSpeed * Time.deltaTime;
                pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
                if (pos.y == transform.position.y) { animator.SetBool("isClimbing", false); }
                transform.position = pos;
            }
        }
        inWater = false;
    }

    void Update() {
        if (canMove) {
            if (!onLadder) {
                rb.gravityScale = 2;
                if (isGrounded) {
                    rb.gravityScale = 0; rb.velocity = Vector2.zero;
                }
            }
        }
    }

    void LateUpdate() {
        if (canMove) {
            if (!onLadder) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    if (interactor.currentInteractable) {
                        Interactable interactable = interactor.currentInteractable.GetComponent<Interactable>();
                        interactable.Interact();
                    } else if (corpse && isGrounded) {
                        Place();
                    }
                }
            } else {
                if (Input.GetKeyDown(KeyCode.Space) && interactor.withLadder) {
                    onLadder = false;
                    animator.SetBool("onLadder", false);
                    FallFromHeight();
                    collider.enabled = true;
                    groundCheckCollider.enabled = true;
                }
            }
        } else if (isHiding) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Unhide();
            }
        } else if (GameManager.manager.isInspecting && (Input.GetKeyDown(KeyCode.Space) || (Input.GetKeyDown(KeyCode.Escape)))) {
            GameManager.manager.Uninspect();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Level") {
            isGrounded = true;
            if (freeFall) {
                freeFall = false;
                Land();
            }
        } else if (collision.tag == "Water") {
            inWater = true;
            makingNoise = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Level") {
            FallFromHeight();
        }
    }

    public void ClimbLadder() {
        onLadder = true;
        freeFall = false;
        collider.enabled = false;
        interactor.indicator.SetActive(false);
        animator.SetBool("onLadder", true);

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        Vector2 pos = transform.position;
        pos.x = interactor.currentInteractable.transform.position.x;
        transform.position = pos;

        minHeight = interactor.currentInteractable.GetComponent<Ladder>().minHeight;
        maxHeight = interactor.currentInteractable.GetComponent<Ladder>().maxHeight;
    }

    public void Hide() {
        interactor.indicator.SetActive(false);
        isHiding = true;
        spriteRenderer.enabled = false;
        canMove = false;
        DisableColliders();

        Vector2 pos = transform.position;
        pos.x = interactor.currentInteractable.transform.position.x;
        transform.position = pos;
    }

    void Unhide() {
        isHiding = false;
        canMove = true;
        spriteRenderer.enabled = true;
        EnableColliders();
    }

    public void Place() {
        StartCoroutine(Place(1.1f));
    }

    public void Pickup() {
        corpse = interactor.currentInteractable.GetComponent<Corpse>();
        interactor.RemoveInteractable();
        interactor.currentInteractable.SetActive(false);
        StartCoroutine(Pickup(1));
    }

    void Land() {
        StartCoroutine(Land(1));
        makingNoise = true;
    }

    void WakeUp() {
        canMove = false;
        StartCoroutine(WakeUp(5));
    }

    public void PerformSuccessfulRitual() {
        canMove = false;
        animator.SetTrigger("Perform Successful Ritual");
        StartCoroutine(PerformSuccessfulRitual(6));
    }

    public void PerformFailedRitual() {
        canMove = false;
        animator.SetTrigger("Perform Failed Ritual");
        StartCoroutine(PerformFailedRitual(7));
    }

    public void DisableColliders() {
        collider.enabled = false;
        groundCheckCollider.enabled = false;
    }

    public void EnableColliders() {
        collider.enabled = true;
        groundCheckCollider.enabled = true;
    }

    void FallFromHeight() {
        // Drop Height
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2.5f, colliderMask);
        if (hit.collider == null) {
            freeFall = true;
            animator.SetTrigger("freeFall");

            if (corpse) {
                GameObject newCorpse = SpawnCorpse();
                float horizontal = 0;
                if (Input.GetKey(KeyCode.A)) { horizontal -= 1; }
                if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.D)) { horizontal += 1; }

                newCorpse.GetComponent<Rigidbody2D>().velocity = Vector2.right * horizontal * 2.8f;
                newCorpse.GetComponent<Corpse>().EnterFreeFall();
            }
        }
    }

    GameObject SpawnCorpse() {
        GameObject newCorpse = Instantiate(corpseObject, transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity, corpses);
        newCorpse.GetComponent<Corpse>().id = corpse.id;
        newCorpse.GetComponent<Corpse>().doors = corpse.doors;
        newCorpse.GetComponent<Corpse>().corpses = corpse.corpses;
        newCorpse.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
        Destroy(corpse.gameObject);
        corpse = null;
        return newCorpse;
    }

    IEnumerator Place(float seconds) {
        canMove = false;
        animator.SetTrigger("Place");
        yield return new WaitForSeconds(seconds);

        SpawnCorpse();
        canMove = true;
    }

    IEnumerator Pickup(float seconds) {
        canMove = false;
        animator.SetTrigger("Pickup");
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    IEnumerator Land(float seconds) {
        canMove = false;
        animator.ResetTrigger("freeFall");
        animator.SetTrigger("Land");
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    IEnumerator WakeUp(float seconds) {
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    IEnumerator PerformFailedRitual(float seconds) {
        yield return new WaitForSeconds(seconds);
        GameManager.manager.FadeOut(5f, 1f, 2);
    }

    IEnumerator PerformSuccessfulRitual(float seconds) {
        yield return new WaitForSeconds(seconds);
        GameManager.manager.FadeOut(5f, 1f, 3);
    }

    IEnumerator Step() {
        isStepping = true;
        if (inWater) { stepSource.clip = stepWater; }
        else { stepSource.clip = stepLand; }
        stepSource.Play();
        yield return new WaitForSeconds(0.25f);
        isStepping = false;
    }
}
