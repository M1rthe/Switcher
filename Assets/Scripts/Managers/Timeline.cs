using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Timeline : MonoBehaviour
{
    MeshRenderer[] meshRenderers;
    ParticleSystem[] particleSystems;
    Canvas[] canvasses;
    LayerMask ignorePlayerMask;
    LayerMask defaultMask;

    void Start()
    {
        defaultMask = gameObject.layer;
        ignorePlayerMask = LayerMask.NameToLayer("IgnorePlayer");
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        canvasses = GetComponentsInChildren<Canvas>();
    }

    public void Enable(bool enable)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer.CompareTag("Item"))
            {
                Item item = meshRenderer.GetComponent<Item>();

                if (item.timelineExistance == Item.TimelineExistance.EveryTL)
                {
                    meshRenderer.enabled = true;
                    //if (enable) meshRenderer.gameObject.layer = defaultMask;
                    //else meshRenderer.gameObject.layer = item.itemTypeLayer;
                }
                if (item.timelineExistance == Item.TimelineExistance.CurrentTL)
                {
                    if (item.inHand) item.startTimeline = TimelineManager.CurrentTimeline;
                    meshRenderer.enabled = item.startTimeline == TimelineManager.CurrentTimeline;

                    //if (takeItemWithPlayer) meshRenderer.gameObject.layer = defaultMask;
                    //else meshRenderer.gameObject.layer = item.itemHeldTypeLayer;
                }
                if (item.timelineExistance == Item.TimelineExistance.OnlyCurrentTL)
                {
                    //Visual
                    meshRenderer.enabled = enable;

                    //Physics
                    //if (enable) meshRenderer.gameObject.layer = defaultMask;
                    //else meshRenderer.gameObject.layer = item.itemHeldTypeLayer;

                    if (item.inHand) item.GetComponentInParent<Pickup>().Drop(Client.photonPlayer);
                }
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

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (enable) particleSystem.Play();
            else particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }


        foreach (Canvas canvas in canvasses)
        {
            canvas.gameObject.SetActive(enable);
        }
    }

    public void TransparentMode(bool enable)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer.CompareTag("Item"))
            {
                Item item = meshRenderer.GetComponent<Item>();

                if (item.timelineExistance == Item.TimelineExistance.EveryTL)
                {
                    meshRenderer.enabled = true;
                    print("Keep : " + item.name);
                }
                if (item.timelineExistance == Item.TimelineExistance.CurrentTL && item.inHand)
                {
                    print("Keep with player : " + item.name);
                }
                if (item.timelineExistance == Item.TimelineExistance.OnlyCurrentTL && enable)
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
