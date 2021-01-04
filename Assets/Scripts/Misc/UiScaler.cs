using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiScaler : MonoBehaviour {
    RectTransform rectTransform;

    float scale = 1;

    Vector2 positionAtFHD;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        positionAtFHD = rectTransform.anchoredPosition;
    }

    private void Start() {
        UpdateScale();
    }

    public void UpdateScale() {
        scale = Screen.width / 1920f;

        rectTransform.anchoredPosition = positionAtFHD * scale;

        rectTransform.localScale = new Vector2(1, 1) * scale;
    }
}
