using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public Sound[] sounds;

    public float radius = 1;
    public float maxVolume = 1;

    public void Awake() {
        foreach (Sound sound in sounds) {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;

            sound.audioSource.pitch = sound.pitch;

            sound.audioSource.loop = sound.looping;
        }
    }

    public void PlaySound(string name) {
        foreach (Sound sound in sounds) {
            if (sound.name == name) {
                sound.audioSource.Play();
            }
        }
    }

    void Update () {
        float x = Mathf.Abs(transform.position.x - PlayerController.controller.transform.position.x);
        float y = Mathf.Abs(transform.position.y - PlayerController.controller.transform.position.y);

        float xSquared = Mathf.Pow(x, 2);
        float ySquared = Mathf.Pow(y, 2);

        float newVolume = Mathf.Clamp(radius / xSquared, 0, 1) * Mathf.Clamp(0.1f * radius / ySquared, 0, maxVolume);
        float newPan = Mathf.Clamp(Mathf.Pow((transform.position.x - PlayerController.controller.transform.position.x) / radius, 3), -1, 1);
        foreach (Sound sound in sounds) {
            sound.audioSource.volume = newVolume * sound.volumeMultiplier;
            sound.audioSource.panStereo = newPan;
        }
    }
}
