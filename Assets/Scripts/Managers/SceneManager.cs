using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SceneManager : FullScreenManager
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Client.location = Client.Location.Menu;

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        ManageKeycodes();
    }
}
