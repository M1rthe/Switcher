using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyPaste : MonoBehaviour
{
    public void Copy(InputField inputField)
    {
        GUIUtility.systemCopyBuffer = inputField.text;
    }

    public void Paste(InputField inputField)
    {
        inputField.text = GUIUtility.systemCopyBuffer;
    }

    public void Copy(Text inputField)
    {
        GUIUtility.systemCopyBuffer = inputField.text;
    }

    public void Paste(Text inputField)
    {
        inputField.text = GUIUtility.systemCopyBuffer;
    }
}
