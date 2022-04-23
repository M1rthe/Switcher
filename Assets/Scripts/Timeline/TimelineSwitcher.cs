using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Photon.Pun;

public class TimelineSwitcher : MonoBehaviour, IOnPlayersSpawned, IOnSwitchedTimeline
{
    enum PreviewType { Hold, Toggle, CertainTime }
    [SerializeField] PreviewType previewType = PreviewType.Hold;

    [SerializeField] Volume vignette;
    [SerializeField] Text currentTimeline;
    [SerializeField] GameObject topLeftPanel;

    AudioManager audioManager;

    [Space]

    PhotonView missionCheckView;

    bool displayingPreview = false;
    float previewDuration = 3.5f;

    Exposure flare;

    void Start()
    {
        vignette.weight = 0f;

        audioManager = GetComponentInChildren<AudioManager>();

        missionCheckView = FindObjectOfType<MissionCheck>().transform.GetComponent<PhotonView>();

        foreach (Volume volume in TimelineManager.Instance.GetComponentsInChildren<Volume>())
        {
            if (volume.profile.TryGet<Exposure>(out var f))
            {
                flare = f;
                break;
            }
        }

        //Start timeline
        List<StartTimeline> startTimelines = LevelData.ReadStartTimeline();
        StartTimeline startTimeline = startTimelines[GameManager.GetCurrentScene() - 1];
        if (GameManager.hostJoin == GameManager.HostJoin.Host)
        {
            if (startTimeline.p0 == 0) { startTimeline.p0 = 1; Debug.LogError("Json tried to start the host (present|future) on past >:|"); }
            TimelineManager.Instance.SetTimeline(startTimeline.p0);
        }
        else
        {
            if (startTimeline.p1 == 2) { startTimeline.p1 = 1; Debug.LogError("Json tried to start the joiner (past|present) on future >:|"); }
            TimelineManager.Instance.SetTimeline(startTimeline.p1);
        }
    }

    public void OnPlayersSpawned()
    {
        UpdateCurrentTimelineHUD(TimelineManager.CurrentTimeline, GameManager.hostJoin, true);
        currentTimeline.text = TimelineManager.Timelines[TimelineManager.CurrentTimeline].name;
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
        audioManager.PlayAudio("SwitchTimeline");
        StartCoroutine(LightFlare(flare));

        if (GameManager.PlayerType.PastPresent == GameManager.playerType)
        {
            if (TimelineManager.CurrentTimeline == 0)
            {
                //SWITCH TO PRESENT                
                TimelineManager.Instance.SetTimeline(1);
            }
            else
            {
                //SWITCH TO PAST
                TimelineManager.Instance.SetTimeline(0);
            }
        }

        if (GameManager.PlayerType.FuturePresent == GameManager.playerType)
        {
            if (TimelineManager.CurrentTimeline == 2)
            {
                //SWITCH TO PRESENT
                TimelineManager.Instance.SetTimeline(1);
            }
            else
            {
                //SWITCH TO FUTURE
                TimelineManager.Instance.SetTimeline(2);
            }
        }

        if (TimelineManager.CurrentTimeline == 1) vignette.weight = 0f;
        else vignette.weight = 1f;

        UpdateCurrentTimelineHUD(TimelineManager.CurrentTimeline, GameManager.hostJoin);

        currentTimeline.text = TimelineManager.Timelines[TimelineManager.CurrentTimeline].name;
    }

    void UpdateCurrentTimelineHUD(int currentTimeline, GameManager.HostJoin hostJoin, bool first = false)
    {
        Slider slider;
        if (hostJoin == GameManager.HostJoin.Host) slider = GameManager.hud.hostPlayerSlider; //Error
        else slider = GameManager.hud.playerSlider; //error

        if (first) slider.value = currentTimeline;
        else GameManager.hud.StartCoroutine(GameManager.hud.MoveSliderTowards(slider, currentTimeline)); //NULL REFERENCE on JOINER
    }

    public void OnOtherPlayerSwitched(bool firstTime)
    {
        GameManager.HostJoin hj = GameManager.HostJoin.Undefined;
        if (GameManager.hostJoin == GameManager.HostJoin.Host) hj = GameManager.HostJoin.Join;
        if (GameManager.hostJoin == GameManager.HostJoin.Join) hj = GameManager.HostJoin.Host;

        if (hj != GameManager.HostJoin.Undefined)
        {
            UpdateCurrentTimelineHUD(TimelineManager.CurrentTimelineOtherPlayer, hj, firstTime);
        }
    }
    public void OnPlayerSwitched() { }

    IEnumerator LightFlare(Exposure exposure)
    {
        float t = 0;
        int lastKey = GameManager.Instance.lightFlareCurve.length - 1;
        float end = GameManager.Instance.lightFlareCurve.keys[lastKey].time;

        while (t < end)
        {
            t += Time.deltaTime;
            exposure.fixedExposure.value = GameManager.Instance.lightFlareCurve.Evaluate(t);

            yield return null;
        }

        yield return null;
    }

    void DisplayPreview(bool display)
    {
        //displayingPreview = display;

        //if (display)
        //{
        //    vignette.weight = 0.7f;

        //    if (GameManager.PlayerType.PastPresent == GameManager.playerType)
        //    {
        //        if (TimelineManager.CurrentTimeline == 0)
        //        {
        //            //In past, previewing present
        //            TimelineManager.PreviewTimeline(1, true);
        //        }
        //        else
        //        {
        //            //In present, previewing past
        //            TimelineManager.PreviewTimeline(0, true);
        //        }
        //    }

        //    if (GameManager.PlayerType.FuturePresent == GameManager.playerType)
        //    {
        //        if (TimelineManager.CurrentTimeline == 2)
        //        {
        //            //"In future, previewing present
        //            TimelineManager.PreviewTimeline(1, true);
        //        }
        //        else
        //        {
        //            //"In present, previewing future
        //            TimelineManager.PreviewTimeline(2, true);
        //        }
        //    }
        //}
        //else
        //{
        //    vignette.weight = 0f;

        //    if (GameManager.PlayerType.PastPresent == GameManager.playerType)
        //    {
        //        if (TimelineManager.CurrentTimeline == 0)
        //        {
        //            TimelineManager.PreviewTimeline(1, false);
        //        }
        //        else
        //        {
        //            TimelineManager.PreviewTimeline(0, false);
        //        }
        //    }

        //    if (GameManager.PlayerType.FuturePresent == GameManager.playerType)
        //    {
        //        if (TimelineManager.CurrentTimeline == 2)
        //        {
        //            TimelineManager.PreviewTimeline(1, false);
        //        }
        //        else
        //        {
        //            TimelineManager.PreviewTimeline(2, false);
        //        }
        //    }
        //}
    }

    IEnumerator DisplayPreview()
    {
        DisplayPreview(true);
        yield return new WaitForSeconds(previewDuration);
        DisplayPreview(false);
    }
}
