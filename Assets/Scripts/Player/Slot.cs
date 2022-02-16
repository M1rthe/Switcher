using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item item = null;
    [SerializeField] Image background;
    [SerializeField] Image icon;

    public void SetSlotActive(bool active)
    {
        if (active)
        {
            background.color = Color.grey;
        }
        else
        {
            background.color = Color.white;
        }
    }
}
