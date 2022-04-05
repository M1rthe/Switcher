using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;
using UnityEngine.UIElements;

[System.Serializable]
public class Timeline : MonoBehaviour
{
    MeshRenderer[] meshRenderers;
    ParticleSystem[] particleSystems;
    Canvas[] canvasses;
    Volume[] volumes;
    Door[] doors;

    LayerMask ignorePlayerMask;
    LayerMask defaultMask;

    void Start()
    {
        defaultMask = gameObject.layer;
        ignorePlayerMask = LayerMask.NameToLayer("IgnorePlayer");
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        canvasses = GetComponentsInChildren<Canvas>();
        volumes = GetComponentsInChildren<Volume>();
        doors = GetComponentsInChildren<Door>();
    }

    public void Enable(bool enable)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer.CompareTag("Item"))
            {
                Item item = meshRenderer.GetComponent<Item>();

                if (item.inHand)
                {
                    if (item.type == Item.Type.CurrentTL) item.StartTimeline = TimelineManager.CurrentTimeline; 
                    if (item.type == Item.Type.OnlyCurrentTL) item.GetComponentInParent<Pickup>().Drop(GameManager.photonPlayer);
                }

                item.PhotonView.RPC("UpdateLayer", RpcTarget.All);
            }
            else
            {
                //Visual
                meshRenderer.enabled = enable;

                //Physics
                if (enable) meshRenderer.gameObject.layer = defaultMask;
                else meshRenderer.gameObject.layer = ignorePlayerMask;
            }
        }

        if (enable)
        {
            foreach (ManagePhotonPlayer managePhotonPlayer in GameObject.FindObjectsOfType<ManagePhotonPlayer>())
            {
                //Debug.LogError("SetColorTransparent() : CurrentTimeline = "+TimelineManager.CurrentTimeline + ", CurrentTimelineOtherPlayer = "+TimelineManager.CurrentTimelineOtherPlayer);
                managePhotonPlayer.PhotonView.RPC("SetColorTransparent", RpcTarget.All, TimelineManager.CurrentTimeline != TimelineManager.CurrentTimelineOtherPlayer);
            }
        }

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (enable) particleSystem.Play();
            else particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        foreach (Canvas canvas in canvasses)
        {
            canvas.gameObject.SetActive(enable);
        }

        foreach (Volume volume in volumes)
        {
            volume.enabled = enable;
        }

        foreach (Door door in doors)
        {
            if (!enable) door.checkInput = false;
        }
    }

    public void TransparentMode(bool enable)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer.CompareTag("Item"))
            {
                Item item = meshRenderer.GetComponent<Item>();

                if (item.type == Item.Type.EveryTL)
                {
                    print("Keep : " + item.name);
                }
                if (item.type == Item.Type.CurrentTL && item.inHand)
                {
                    print("Keep with player : " + item.name);
                }
                if (item.type == Item.Type.OnlyCurrentTL && enable)
                {
                    print("Stay behind in timeline : " + item.name);
                }
            }
            else
            {
                //Visual
                if (meshRenderer.CompareTag("Addition"))
                {
                    Color c = meshRenderer.material.color;
                    c.a = 1f;

                    if (enable) c.a = 0.5f;

                    meshRenderer.material.color = c;

                    meshRenderer.enabled = enable;
                }

                //Physics
                if (enable) meshRenderer.gameObject.layer = ignorePlayerMask;
                else meshRenderer.gameObject.layer = defaultMask;
            }
        }
    }
}
