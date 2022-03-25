using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FormControls : MonoBehaviour
{
    Button[] buttons;
    InputField[] inputFields;
    int currentInputfield = -1;

    void Start()
    {
        //Get things
        buttons = GetComponentsInChildren<Button>();
        inputFields = GetComponentsInChildren<InputField>();

        for (int i = 0; i < inputFields.Length; i++)
        {
            //Add event trigger to the inputfields
            EventTrigger eventTrigger = inputFields[i].gameObject.AddComponent<EventTrigger>();

            //Add on Inputfield selected event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;

            //Add event where 'currentInputfield' gets updated on selected
            int temp = i;
            entry.callback.AddListener((eventData) => { currentInputfield = temp; });
            eventTrigger.triggers.Add(entry);
        }
    }

    void Update()
    {
        //TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentInputfield < inputFields.Length - 1)
            {
                //Select next inputfield
                currentInputfield++;
                inputFields[currentInputfield].Select();
            }
        }

        //ENTER
        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach (Button btn in buttons)
            {
                if (btn.interactable)
                {
                    //Invoke button
                    btn.onClick.Invoke();
                    break;
                }
            }
        }
    }
}
