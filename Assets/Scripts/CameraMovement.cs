using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement: MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float smoothness = 0.05f;
    public Vector3 offset = new Vector3(0, 0.5f, 0);
    public Vector2 noise;

    public static float shakeAmount = 0.1f;

    float targetZoom;

    new Camera camera;

    private void Start() {
        transform.position = PlayerController.controller.transform.position + offset;
        camera = GetComponent<Camera>();
    }

    void FixedUpdate() {
        float x = Mathf.Lerp(transform.position.x, PlayerController.controller.transform.position.x + offset.x, smoothness);
        float y = Mathf.Lerp(transform.position.y, PlayerController.controller.transform.position.y + offset.y, smoothness);
        transform.position = new Vector3(x + noise.x, y + noise.y, -10);
    }

    private void Update() {
        offset.y = 0.5f;
        if (!PlayerController.controller.onLadder) {
            if (Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.W)) { offset.y += 2f; }
            if (Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.S)) { offset.y -= 2f; }
        }

        if (PlayerController.controller.isHiding) { targetZoom = 4; } else { targetZoom = 5; }

        if (camera.orthographicSize != targetZoom) {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetZoom, 0.05f);
        }
    }

    public void Shake() {
        StartCoroutine(Shake(1.8f));
    }

    public IEnumerator Shake(float shakeTime) {
        float elapsedTime = 0;
        while (elapsedTime < shakeTime) {
            elapsedTime += Time.deltaTime;
            noise = new Vector2(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount)) * (shakeTime - elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        noise = Vector2.zero;
    }
}
