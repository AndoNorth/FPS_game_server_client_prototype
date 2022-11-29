using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    private void Awake()
    {
        // singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        // initialise sounds
        foreach(Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.loop = sound.loop;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }
    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        sound.source.Play();
    }
}
