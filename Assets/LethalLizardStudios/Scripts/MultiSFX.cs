using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSFX : MonoBehaviour
{
    public AudioSource[] sources;

    [HideInInspector] public AudioClip clip;

    private int count = 0;

    private void Start()
    {
        foreach (AudioSource s in sources)
            s.volume = sources[0].volume;

        foreach (AudioSource s in sources)
            s.pitch = sources[0].pitch;

        clip = sources[0].clip;
    }

    public void Play()
    {
        sources[count].clip = clip;
        sources[count].Play();

        if (count >= sources.Length - 1)
            count = 0;
        else
            count++;
    }

    public void Volume(float volume)
    {
        foreach (AudioSource s in sources)
            s.volume = volume;
    }

    public void Pitch(float pitch)
    {
        foreach (AudioSource s in sources)
            s.pitch = pitch;
    }
}

