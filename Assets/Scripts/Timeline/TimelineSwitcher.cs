using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Photon.Pun;

public class TimelineSwitcher : MonoBehaviour
{
    enum PreviewType { Hold, Toggle, CertainTime }
    [SerializeField] PreviewType previewType = PreviewType.Hold;

    [SerializeField] Volume vignette;
    [SerializeField] Text currentTimeline;
    [SerializeField] GameObject topLeftPanel;

    [Space]

    PhotonView missionCheckView;

    bool displayingPreview = false;
    float previewDuration = 3.5f;

    void Start()
    {
        vignette.weight = 0f;
        TimelineManager.SetTimeline(TimelineManager.CurrentTimeline);
        currentTimeline.text = TimelineManager.Timelines[TimelineManager.CurrentTimeline].name;

        missionCheckView = FindObjectOfType<MissionCheck>().transform.GetComponent<PhotonView>();
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
        missionCheckView.RPC("Switched", RpcTarget.All);

        if (Client.PlayerType.PastPresent == Client.playerType)
        {
            if (TimelineManager.CurrentTimeline == 0)
            {
                //SWITCH TO PRESENT                
                TimelineManager.SetTimeline(1);
            }
            else
            {
                //SWITCH TO PAST
                TimelineManager.SetTimeline(0);
            }
        }

        if (Client.PlayerType.FuturePresent == Client.playerType)
        {
            if (TimelineManager.CurrentTimeline == 2)
            {
                //SWITCH TO PRESENT
                TimelineManager.SetTimeline(1);
            }
            else
            {
                //SWITCH TO FUTURE
                TimelineManager.SetTimeline(2);
            }
        }

        if (TimelineManager.CurrentTimeline == 1) vignette.weight = 0f;
        else vignette.weight = 1f;

        currentTimeline.text = TimelineManager.Timelines[TimelineManager.CurrentTimeline].name;
    }

    void DisplayPreview(bool display)
    {
        displayingPreview = display;

        if (display)
        {
            vignette.weight = 0.7f;

            if (Client.PlayerType.PastPresent == Client.playerType)
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

            if (Client.PlayerType.FuturePresent == Client.playerType)
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
            vignette.weight = 0f;

            if (Client.PlayerType.PastPresent == Client.playerType)
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

            if (Client.PlayerType.FuturePresent == Client.playerType)
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
