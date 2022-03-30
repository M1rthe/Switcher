using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimelineManager : MonoBehaviour, ISerializationCallbackReceiver
{
    //Current timeline
    public static int CurrentTimeline = -1;
    public static int CurrentTimelineOtherPlayer = -1;

    //Timelines
    [SerializeField] Timeline[] timelines = new Timeline[3];
    public static Timeline[] Timelines = new Timeline[3];

    //PhotonView
    public static PhotonView PhotonView;

    public static MonoBehaviour Instance;

    void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        Instance = this;
    }

    [PunRPC]
    public void SendOtherPlayerYourCurrentTimeline(int timeline)
    {
        CurrentTimelineOtherPlayer = timeline;
    }

    public static void SetTimeline(int timeline)
    {
        //Enable current
        CurrentTimeline = timeline;
        PhotonView.RPC("SendOtherPlayerYourCurrentTimeline", RpcTarget.Others, timeline);
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
}
