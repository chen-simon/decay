using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager manager;

    public GameObject[] flames;
    public Transform doors;
    public Transform corpses;

    public Image controls;
    public Image screenTint;
    public Image quitMeterImage;
    public GameObject inspectableImage;

    public Sprite[] pagePieceImages;
    public Sprite[] posterPieceImages;

    public Inspectable poster;
    public Axe axe;
    public Sprite bloodyAxe;
    public GameObject schoolGirl;
    public AudioSource axeSlashes;
    public GameObject corpseObject;

    public AudioSource burnSound;

    PlayerController player;
    public Inspectable currentInspectable;

    public bool[] pagePieces = new bool[5];

    public static float quitTime = 1.5f;
    float quitMeter;
    bool quit;

    public bool isInspecting;
    
    void Awake() {
        Application.targetFrameRate = 60;

        // Singleton
        if (manager) {
            Destroy(gameObject);
        } else {
            manager = this;
        }
    }

    private void Start() {
        LoadPagePieces();
        poster.PosterCheck();

        player = PlayerController.controller;
        FadeControls();
        FadeIn(3f, 1f);
    }

    private void Update() {
        if (!quit) {
            if (Input.GetKey(KeyCode.Escape)) {
                if (!isInspecting) {
                    quitMeter += Time.deltaTime;
                    if (quitMeter > quitTime) {
                        // Return to main menu
                        FadeOut(0.5f, 1f, 1);
                        quit = true;
                        quitMeter = 0;
                    }
                }
            } else {
                quitMeter = 0;
            }
            quitMeterImage.fillAmount = quitMeter / quitTime;
        }
    }

    public void OpenDoors(int[] doors) {
        foreach (int i in doors) {
            foreach (Transform door in this.doors) {
                if (i == door.GetComponent<Door>().id) {
                    door.GetComponent<Door>().Open();
                }
            }
        }
    }

    public void DecayCorpses(int[] corpses) {
        foreach (int i in corpses) {
            foreach (Transform corpse in this.corpses) {
                if (i == corpse.GetComponent<Corpse>().id) {
                    corpse.GetComponent<Corpse>().Decay();
                }
            }
        }
    }

    public void GameOver() {
        player.canMove = false;
        player.DisableColliders();
        screenTint.color = Color.black;
        quit = true;
        StartCoroutine(Death(3f));
    }

    public void Inspect(Sprite sprite, Inspectable inspectable) {
        isInspecting = true;
        inspectableImage.GetComponent<UiScaler>().UpdateScale();
        inspectableImage.GetComponent<Image>().color = Color.white;
        inspectableImage.GetComponent<Image>().sprite = inspectable.sprite;

        for (int i = 0; i < 5; i++) {
            if (inspectable.pieces[i]) {
                poster.pieces[i] = inspectable.pieces[i];
            }
        }
        if (!inspectable.isPoster) {
            poster.UpdatePoster();
        }

        if (inspectable.id != -1) {
            pagePieces[inspectable.id] = true;
        }

        currentInspectable = inspectable;
        player.canMove = false;
        FadeOut(0.5f, 0.5f, 0);
    }

    public void Uninspect() {
        isInspecting = false;
        inspectableImage.GetComponent<Image>().color = Color.clear;
        player.canMove = true;
        FadeIn(0.5f, 0.5f);
        if (currentInspectable.destroyOnInspect) {
            Destroy(currentInspectable.gameObject);
        }
    }

    public void FadeControls() {
        controls.GetComponent<UiScaler>().UpdateScale();
        StartCoroutine(FadeControls(5f, 1f, 4.5f));
    }

    public void FadeIn(float fadeTime, float opacity) {
        screenTint.GetComponent<UiScaler>().UpdateScale();
        StartCoroutine(FadeInCoroutine(fadeTime, opacity));
    }

    public void FadeOut(float fadeTime, float opacity, int type) {
        screenTint.GetComponent<UiScaler>().UpdateScale();
        StartCoroutine(FadeOutCoroutine(fadeTime, opacity, type));
    }

    public void FlashScreen() {
        screenTint.GetComponent<UiScaler>().UpdateScale();
        StartCoroutine(FlashScreen(0.5f, 0));
    }

    public void AxeSection() {
        player.canMove = false;
        screenTint.GetComponent<UiScaler>().UpdateScale();
        StartCoroutine(AxeSection(0.5f, 6));
    }

    void SavePagePieces() {
        int piecesBitwise = 0b00000;
        for (int i = 4; i >= 0; i--) {
            if (pagePieces[i]) {
                piecesBitwise += (int)Mathf.Pow(2, i);
            }
        }
        // No pieces 0b00000
        // All pieces 0b11111
        PlayerPrefs.SetInt("pieces", piecesBitwise);
    }

    void LoadPagePieces() {
        int piecesBitwise = PlayerPrefs.GetInt("pieces", 0b00000);

        // Axe Section Unlock
        if (piecesBitwise == 0b11111) {
            axe.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        for (int i = 4; i >= 0; i--) {
            int n = piecesBitwise >> i;
            if (n == 1) {
                pagePieces[i] = true;
                piecesBitwise -= n * (int)Mathf.Pow(2, i);
            }
        }
        poster.UpdatePoster();
    }

    IEnumerator FadeControls(float delay, float fadeTime, float stayTime) {
        // Delay
        yield return new WaitForSeconds(delay);

        // Fade In
        float elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = controls.color;
            color.a = -(Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) - 1)/2;
            controls.color = color;
            yield return new WaitForEndOfFrame();
        }

        // Stay
        yield return new WaitForSeconds(stayTime);

        // Fade Out
        elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = controls.color;
            color.a = (Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) + 1)/2;
            controls.color = color;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeInCoroutine(float fadeTime, float opacity) {
        float elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = screenTint.color;
            color.a = opacity * (Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) + 1) / 2;
            screenTint.color = color;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeOutCoroutine(float fadeTime, float opacity, int type) {
        if (type == 3) { screenTint.color = new Color(1, 1, 1, 0); }

        float elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = screenTint.color;
            color.a = opacity * -(Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) - 1) / 2;
            screenTint.color = color;
            yield return new WaitForEndOfFrame();
        }


        // Types: 0 --> Nothing
        // 1 --> Return to main menu
        // 2 --> Roll Credits
        // 3 --> Roll Credits AND Fade to white
        if (type == 1) {
            // Application go to main menu
            SceneManager.LoadScene("Menu");
        } else if (type == 2) {
            // Application go to credits
            SavePagePieces();
            SceneManager.LoadScene("Credits");
        } else if (type == 3) {
            SavePagePieces();
            SceneManager.LoadScene("True Credits");
        }
    }

    IEnumerator Death(float seconds) {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator FlashScreen(float fadeTime, float stayTime) {
        screenTint.color = new Color(1, 1, 1, 0);

        // Fade In
        float elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = screenTint.color;
            color.a = -(Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) - 1) / 2;
            screenTint.color = color;
            yield return new WaitForEndOfFrame();
        }

        // Stay
        yield return new WaitForSeconds(stayTime);

        // Fade Out
        elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = screenTint.color;
            color.a = (Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) + 1) / 2;
            screenTint.color = color;
            yield return new WaitForEndOfFrame();
        }
        screenTint.color = new Color(0, 0, 0, 0);
    }

    IEnumerator AxeSection(float fadeTime, float stayTime) {
        // Fade In
        float elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = screenTint.color;
            color.a = -(Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) - 1) / 2;
            screenTint.color = color;
            yield return new WaitForEndOfFrame();
        }
        screenTint.color = Color.black;
        axeSlashes.Play();

        // Stay
        yield return new WaitForSeconds(stayTime);

        axe.GetComponent<SpriteRenderer>().sprite = bloodyAxe;
        axe.gameObject.layer = LayerMask.NameToLayer("Uninteractable");
        Instantiate(corpseObject, schoolGirl.transform.position - new Vector3(0, 0.25f, 0), Quaternion.identity, corpses);
        Destroy(schoolGirl);

        // Fade Out
        elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            Color color = screenTint.color;
            color.a = (Mathf.Cos(Mathf.PI * elapsedTime / fadeTime) + 1) / 2;
            screenTint.color = color;
            yield return new WaitForEndOfFrame();
        }
        player.canMove = true;
        screenTint.color = new Color(0, 0, 0, 0);
    }
}
