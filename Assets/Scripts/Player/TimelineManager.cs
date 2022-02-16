using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static int CurrentTimeline = -1;
    [SerializeField] Timeline[] timelines = new Timeline[3];
    public static Timeline[] Timelines = new Timeline[3];

    public static int TimelineAsInt(string timeline)
    {
        for (int i = 0; i < Timelines.Length; i++)
        {
            if (Timelines[i].name == timeline)
            {
                return Timelines[i].transform.GetSiblingIndex();
            }
        }

        return -1;
    }

    public static void SetTimeline(int timeline, bool enable)
    {
        //Enable current
        Timelines[timeline].Enable(enable);

        if (enable) 
        {
            CurrentTimeline = timeline;
        }
    }

    public static void PreviewTimeline(int timeline, bool enable)
    {
        Timelines[timeline].TransparentMode(enable);
    }

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        Timelines = timelines;
    }
}
