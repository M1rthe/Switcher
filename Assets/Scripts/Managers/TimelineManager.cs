using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimelineManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static int CurrentTimeline = -1;
    public static int CurrentTimelineOtherPlayer = -1;
    [SerializeField] Timeline[] timelines = new Timeline[3];
    public static Timeline[] Timelines = new Timeline[3];

    public static PhotonView PhotonView;

    void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

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

    [PunRPC]
    public void SendOtherPlayerYourCurrentTimeline(int timeline)
    {
        Debug.LogError("Other player timeline: "+timeline);
        CurrentTimelineOtherPlayer = timeline;
    }

    public static void SetTimeline(int timeline)
    {
        //Enable current
        CurrentTimeline = timeline;
        PhotonView.RPC("SendOtherPlayerYourCurrentTimeline", RpcTarget.Others, timeline);
        for (int i = 0; i < Timelines.Length; i++)
        {
            Timelines[i].Enable(i == timeline);
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
