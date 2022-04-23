using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SpriteSheetAnimation : MonoBehaviour
{
    public bool loop = true;
    [SerializeField] float fps = .25f;
    [SerializeField] Sprite[] frames;

    [HideInInspector] public Image image;

    int currentFrame;
    float timer;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (loop)
        {
            timer += Time.deltaTime;

            if (timer >= fps)
            {
                timer = 0;

                currentFrame = (currentFrame + 1) % frames.Length;
                image.sprite = frames[currentFrame];
            }
        }
    }
}
