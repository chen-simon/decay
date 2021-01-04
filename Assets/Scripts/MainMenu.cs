using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private void Start() {
        Persistence.persistence.Start();
    }

    public void BeginGame() {
        SceneManager.LoadScene("World");
    }

    private void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)) {
            PlayerPrefs.SetInt("pieces", 0b00000);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
