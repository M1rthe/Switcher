using System.Collections;
using UnityEngine;

public class TimelineSwitcher : MonoBehaviour
{
    enum PlayerType { PastPresent, FuturePresent }
    [SerializeField] PlayerType playerType;

    enum PreviewType { Hold, Toggle, CertainTime }
    [SerializeField] PreviewType previewType = PreviewType.Hold;
    bool displayingPreview = false;
    float previewDuration = 3.5f;

    void Start()
    {
        TimelineManager.SetTimeline(TimelineManager.TimelineAsInt("Past"), false);
        TimelineManager.SetTimeline(TimelineManager.TimelineAsInt("Present"), true);
        TimelineManager.SetTimeline(TimelineManager.TimelineAsInt("Future"), false);

        HUD.CurrentTimeline.text = TimelineManager.Timelines[TimelineManager.CurrentTimeline].name;
    }

    void Update()
    {
        //Preview
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (previewType == PreviewType.Toggle) DisplayPreview(!displayingPreview);
            else if (previewType == PreviewType.Hold) DisplayPreview(true);
            else if (previewType == PreviewType.CertainTime && !displayingPreview) StartCoroutine(DisplayPreview());
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (previewType == PreviewType.Hold) DisplayPreview(false);
        }

        //Toggle Timelines
        if (Input.GetMouseButtonDown(3))
        {
            bool switchPreview = displayingPreview;

            if (switchPreview) DisplayPreview(false);
            ToggleTimelines();
            if (switchPreview) DisplayPreview(true);
        }
    }

    void ToggleTimelines()
    {
        if (PlayerType.PastPresent == playerType)
        {
            if (TimelineManager.CurrentTimeline == 0)
            {
                //SWITCH TO PRESENT
                TimelineManager.SetTimeline(0, false);
                TimelineManager.SetTimeline(1, true);
            }
            else
            {
                //SWITCH TO PAST
                TimelineManager.SetTimeline(0, true);
                TimelineManager.SetTimeline(1, false);
            }
        }

        if (PlayerType.FuturePresent == playerType)
        {
            if (TimelineManager.CurrentTimeline == 2)
            {
                //SWITCH TO PRESENT
                TimelineManager.SetTimeline(1, true);
                TimelineManager.SetTimeline(2, false);
            }
            else
            {
                //SWITCH TO FUTURE
                TimelineManager.SetTimeline(1, false);
                TimelineManager.SetTimeline(2, true);
            }
        }

        Debug.LogError("Set timeline: "+ TimelineManager.Timelines[TimelineManager.CurrentTimeline].name);
        HUD.CurrentTimeline.text = TimelineManager.Timelines[TimelineManager.CurrentTimeline].name;
    }

    void DisplayPreview(bool display)
    {
        displayingPreview = display;

        if (display)
        {
            HUD.Vignette.weight = 0.7f;

            if (PlayerType.PastPresent == playerType)
            {
                if (TimelineManager.CurrentTimeline == 0)
                {
                    //In past, previewing present
                    TimelineManager.PreviewTimeline(1, true);
                }
                else
                {
                    //In present, previewing past
                    TimelineManager.PreviewTimeline(0, true);
                }
            }

            if (PlayerType.FuturePresent == playerType)
            {
                if (TimelineManager.CurrentTimeline == 2)
                {
                    //"In future, previewing present
                    TimelineManager.PreviewTimeline(1, true);
                }
                else
                {
                    //"In present, previewing future
                    TimelineManager.PreviewTimeline(2, true);
                }
            }
        }
        else
        {
            HUD.Vignette.weight = 0f;

            if (PlayerType.PastPresent == playerType)
            {
                if (TimelineManager.CurrentTimeline == 0)
                {
                    TimelineManager.PreviewTimeline(1, false);
                }
                else
                {
                    TimelineManager.PreviewTimeline(0, false);
                }
            }

            if (PlayerType.FuturePresent == playerType)
            {
                if (TimelineManager.CurrentTimeline == 2)
                {
                    TimelineManager.PreviewTimeline(1, false);
                }
                else
                {
                    TimelineManager.PreviewTimeline(2, false);
                }
            }
        }
    }

    IEnumerator DisplayPreview()
    {
        DisplayPreview(true);
        yield return new WaitForSeconds(previewDuration);
        DisplayPreview(false);
    }
}
