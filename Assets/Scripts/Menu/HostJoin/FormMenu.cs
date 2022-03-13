using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FormMenu : MonoBehaviour
{
    Button[] buttons;
    InputField[] inputFields;
    int currentInputfield = -1;

    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        inputFields = GetComponentsInChildren<InputField>();

        for (int i = 0; i < inputFields.Length; i++)
        {
            EventTrigger eventTrigger = inputFields[i].gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            int temp = i;
            entry.callback.AddListener((eventData) => { currentInputfield = temp; });
            eventTrigger.triggers.Add(entry);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentInputfield < inputFields.Length - 1)
            {
                currentInputfield++;
                inputFields[currentInputfield].Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach (Button btn in buttons)
            {
                if (btn.interactable)
                {
                    btn.onClick.Invoke();
                    break;
                }
            }
        }
    }
}
