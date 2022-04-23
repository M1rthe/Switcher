using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour, IOnPlayersSpawned
{
    public GameObject deathScreen;
    public WinScreen winScreen;

    [Header("TimelineSlider")]
    public Slider playerSlider;
    public Slider hostPlayerSlider;
    float sliderSpeed = 1.8f;


    public void OnPlayersSpawned()
    {
        deathScreen.SetActive(false);
    }

    void Start()
    {
        winScreen.gameObject.SetActive(false);
    }

    public IEnumerator MoveSliderTowards(Slider slider, float value)
    {
        while (Mathf.Abs(slider.value - value) > 0.03)
        {
            slider.value = Mathf.MoveTowards(slider.value, value, sliderSpeed * Time.deltaTime);

            yield return null;
        }

        yield return null;
    }
}
