using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class HUD : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] Volume vignette;
    public static Volume Vignette;

    [Header("Text")]
    [SerializeField] Text currentTimeline;
    public static Text CurrentTimeline;

    [Header("Panels")]
    [SerializeField] GameObject topLeftPanel;
    public static GameObject TopLeftPanel;

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        Vignette = vignette;

        //Text
        CurrentTimeline = currentTimeline;

        //Panels
        TopLeftPanel = topLeftPanel;
    }
}
