using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenManager : MonoBehaviour
{
    protected void ManageKeycodes()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
                if (Cursor.lockState == CursorLockMode.Confined) Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Screen.fullScreen = true;
                if (Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }
}
