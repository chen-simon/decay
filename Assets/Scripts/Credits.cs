using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour {
    public Image scrollingCredits;

    static float scrollingSpeed = 130;

    void Start() {
        StartCoroutine(BeginCreditSequence(3, 18, 0));
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Menu");
        }
    }

    IEnumerator BeginCreditSequence(float waitTimeStart, float waitTimeMiddle, float waitTimeEnd) {
        yield return new WaitForSeconds(waitTimeStart);
        float elapsedTime = 0;
        while (elapsedTime < waitTimeMiddle) {
            scrollingCredits.rectTransform.anchoredPosition -= Vector2.down * Time.deltaTime * scrollingSpeed;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(waitTimeEnd);
        SceneManager.LoadScene("Menu");
    }
}
