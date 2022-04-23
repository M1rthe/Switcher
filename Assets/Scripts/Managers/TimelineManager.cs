using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class TimelineManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static TimelineManager Instance;

    //Current timeline
    public static int CurrentTimeline = -1;
    public static int CurrentTimelineOtherPlayer = -1;

    //Timelines
    [SerializeField] Timeline[] timelines = new Timeline[3];
    public static Timeline[] Timelines = new Timeline[3];

    //PhotonView
    public PhotonView photonView { get; private set; }

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Instance = this;

        CurrentTimeline = -1;
        CurrentTimelineOtherPlayer = -1;
    }

    [PunRPC]
    public void SendOtherPlayerYourCurrentTimeline(int timeline)
    {
        bool firstTime = CurrentTimelineOtherPlayer == -1;

        CurrentTimelineOtherPlayer = timeline;

        var onSwitchedTimelines = FindObjectsOfType<MonoBehaviour>().OfType<IOnSwitchedTimeline>();
        foreach (IOnSwitchedTimeline onSwitchedTimeline in onSwitchedTimelines)
        {
            onSwitchedTimeline.OnOtherPlayerSwitched(firstTime);
        }
    }

    public void SetTimeline(int timeline)
    {
        //Enable current
        CurrentTimeline = timeline;
        var onSwitchedTimelines = FindObjectsOfType<MonoBehaviour>().OfType<IOnSwitchedTimeline>();
        foreach (IOnSwitchedTimeline onSwitchedTimeline in onSwitchedTimelines)
        {
            onSwitchedTimeline.OnPlayerSwitched();
        }

        photonView.RPC("SendOtherPlayerYourCurrentTimeline", RpcTarget.Others, timeline);
        Instance.StartCoroutine(WaitForOtherPlayersCurrentTimeline(delegate{
            for (int i = 0; i < Timelines.Length; i++)
            {
                Timelines[i].Enable(i == timeline);
            }
        }));
    }

    public static IEnumerator WaitForOtherPlayersCurrentTimeline(System.Action action)
    {
        if (TimelineManager.CurrentTimelineOtherPlayer == -1) yield return null;

        action.Invoke();
    }

    public static void PreviewTimeline(int timeline, bool enable)
    {
        Timelines[timeline].TransparentMode(enable);
    }

    //Convert timeline to int
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

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        Timelines = timelines;
    }

    public static int GetTimelineOf(GameObject go)
    {
        Timeline timeline = go.GetComponent<Timeline>();
        if (timeline != null)
        {
            return go.transform.GetSiblingIndex();
        }

        Transform parentTransform = go.transform.parent;
        if (parentTransform == null) return -1;
        return GetTimelineOf(parentTransform.gameObject);
    }
}
