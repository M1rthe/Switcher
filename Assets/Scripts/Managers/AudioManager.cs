using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    Transform[] transforms;

    public void PlayAudio(string audioSourceName)
    {
        if (transforms == null) transforms = GetComponentsInChildren<Transform>();

        foreach (Transform transform in transforms)
        {
            if (transform.gameObject.name == audioSourceName)
            {
                AudioSource[] audioSources = transform.GetComponents<AudioSource>();
                foreach (AudioSource audioSource in audioSources) audioSource.Play();
            }
        }
    }
}
