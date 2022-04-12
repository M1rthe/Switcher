using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public GameObject deathScreen;

    void Start()
    {
        deathScreen.SetActive(false);
    }
}
