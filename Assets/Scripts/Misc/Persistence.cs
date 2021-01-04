using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistence : MonoBehaviour {
    public static Persistence persistence;
    public AudioClip reverseDecay;

    void Awake() {
        if (!persistence) {
            persistence = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Start() {
        if (PlayerPrefs.GetInt("pieces") == 0b11111) {
            Reverse();
        }
    }

    public void Reverse() {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == reverseDecay) { return; }

        audioSource.Stop();
        audioSource.clip = reverseDecay;
        audioSource.Play();
    }
}
