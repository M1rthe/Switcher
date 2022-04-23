using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Photon.Pun;
using UnityEngine.UIElements;

[System.Serializable]
public class Timeline : MonoBehaviour, IOnPlayersSpawned
{
    [SerializeField] Transform[] transforms;
    [SerializeField] Item[] items;

    [SerializeField] ManagePhotonPlayer[] managePhotonPlayer;
    [SerializeField] TimelineAudioSource[] timelineAudioSources;
    [SerializeField] Volume[] volumes;

    LayerMask ignorePlayerMask;
    LayerMask defaultMask;

    public void OnPlayersSpawned()
    {
        managePhotonPlayer = FindObjectsOfType<ManagePhotonPlayer>();
        timelineAudioSources = FindObjectsOfType<TimelineAudioSource>();
        volumes = FindObjectsOfType<Volume>();
    }

    void Awake()
    {
        defaultMask = gameObject.layer;
        ignorePlayerMask = LayerMask.NameToLayer("IgnorePlayer");

        transforms = GetComponentsInChildren<Transform>(true);
        items = GetComponentsInChildren<Item>();
    }

    public void Enable(bool enable)
    {
        foreach (Transform transform in transforms)
        {
            if (enable) transform.gameObject.layer = defaultMask;
            else transform.gameObject.layer = ignorePlayerMask;
        }

        foreach (Item item in items)
        {
            if (item.inHand)
            {
                if (item.type == Item.Type.CurrentTL) item.StartTimeline = TimelineManager.CurrentTimeline;
                if (item.type == Item.Type.OnlyCurrentTL) item.GetComponentInParent<Pickup>().Drop(GameManager.photonPlayer, item.transform.position, item.transform.eulerAngles);
            }

            item.PhotonView.RPC("UpdateLayer", RpcTarget.All);
        }

        if (enable)
        {
            foreach (ManagePhotonPlayer managePhotonPlayer in managePhotonPlayer)
            {
                managePhotonPlayer.photonView.RPC("SetColorTransparent", RpcTarget.All, TimelineManager.CurrentTimeline != TimelineManager.CurrentTimelineOtherPlayer);
            }

            foreach (TimelineAudioSource timelineAudioSource in timelineAudioSources)
            {
                foreach (GameObject origin in timelineAudioSource.origins)
                {
                    bool inSameTimelineAsOrigin = TimelineManager.GetTimelineOf(origin) == TimelineManager.CurrentTimeline;

                    if (timelineAudioSource.audioSource.loop == true)
                    {
                        timelineAudioSource.audioSource.mute = !inSameTimelineAsOrigin;
                    }

                    if (inSameTimelineAsOrigin) break;
                }
            }
        }
    }

    public void TransparentMode(bool enable)
    {
        //No implementation yet
    }
}