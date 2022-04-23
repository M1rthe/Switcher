using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineAudioSource : MonoBehaviour
{
    public GameObject[] origins;
    public string tagName = "null";
    public AudioSource audioSource;

    private void Awake()
    {
        if (audioSource.loop)
        {
            audioSource.Play();
            audioSource.mute = true;
        }

        if (tagName != "null")
        {
            origins = GameObject.FindGameObjectsWithTag(tagName);
        }
    }
}
