﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ManagePhotonPlayer : MonoBehaviour
{
    public PhotonView PhotonView { get; set; }
    Camera cam;

    [SerializeField] List<Behaviour> components = new List<Behaviour>();
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();

    LayerMask playerLayer;
    LayerMask otherPlayerLayer;
    LayerMask ghostPlayerLayer;
    LayerMask otherGhostPlayerLayer;

    MeshRenderer body;

    void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        cam = GetComponentInChildren<Camera>();
        body = transform.Find("PlayerModel/Body").GetComponent<MeshRenderer>();

        playerLayer = LayerMask.NameToLayer("Player");
        otherPlayerLayer = LayerMask.NameToLayer("OtherPlayer");
        ghostPlayerLayer = LayerMask.NameToLayer("GhostPlayer");
        otherGhostPlayerLayer = LayerMask.NameToLayer("OtherGhostPlayer");

        List<StartTimeline> startTimelines = LevelData.ReadStartTimeline();
        StartTimeline startTimeline = startTimelines[Client.GetCurrentScene() - 1];
        if (Client.hostJoin == Client.HostJoin.Host) TimelineManager.SetTimeline(startTimeline.p0);
        else TimelineManager.SetTimeline(startTimeline.p1);

        ConvertToOtherPlayer(!PhotonView.IsMine);
    }

    void ConvertToOtherPlayer(bool isOtherPlayer)
    {
        if (isOtherPlayer)
        {
            //Other player
            SetLayerRecursively(gameObject, otherPlayerLayer); //Player Layer
            cam.cullingMask |= (1 << LayerMask.NameToLayer("ItemInvisibleToPlayer")); //Culling Mask
        }
        else
        {
            //Current player
            Client.photonPlayer = PhotonView.Owner;

            SetLayerRecursively(gameObject, playerLayer); //Player Layer
            cam.cullingMask |= (1 << LayerMask.NameToLayer("ItemInvisibleToOther")); //Culling Mask
        }

        //Turn off the extra components
        foreach (Behaviour component in components) component.enabled = !isOtherPlayer;
        //Turn off the extra gameobjects
        foreach (GameObject gameObject in gameObjects) gameObject.SetActive(!isOtherPlayer);
    }

    [PunRPC]
    public void ChangeColor()
    {
        body.material.SetColor("_BaseColor", Color.red);
    }

    [PunRPC]
    public void SetColorTransparent(bool enable)
    {
        Color c = body.material.GetColor("_BaseColor");
        if (enable)
        {
            c.a = 0.5f;
            if (gameObject.layer == playerLayer) SetLayerRecursively(gameObject, ghostPlayerLayer);
            if (gameObject.layer == otherPlayerLayer) SetLayerRecursively(gameObject, otherGhostPlayerLayer);
        }
        else
        {
            c.a = 1f;
            if (gameObject.layer == ghostPlayerLayer) SetLayerRecursively(gameObject, playerLayer);
            if (gameObject.layer == otherGhostPlayerLayer) SetLayerRecursively(gameObject, otherPlayerLayer);
        }
        body.material.SetColor("_BaseColor", c);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}