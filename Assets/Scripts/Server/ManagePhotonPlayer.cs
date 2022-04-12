using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ManagePhotonPlayer : MonoBehaviour
{
    public PhotonView photonView { get; private set; }
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
        photonView = GetComponent<PhotonView>();
        cam = GetComponentInChildren<Camera>();
        body = transform.Find("PlayerModel/Body").GetComponent<MeshRenderer>();

        playerLayer = LayerMask.NameToLayer("Player");
        otherPlayerLayer = LayerMask.NameToLayer("OtherPlayer");
        ghostPlayerLayer = LayerMask.NameToLayer("GhostPlayer");
        otherGhostPlayerLayer = LayerMask.NameToLayer("OtherGhostPlayer");

        List<StartTimeline> startTimelines = LevelData.ReadStartTimeline();
        StartTimeline startTimeline = startTimelines[GameManager.GetCurrentScene() - 1];
        if (GameManager.hostJoin == GameManager.HostJoin.Host) TimelineManager.Instance.SetTimeline(startTimeline.p0);
        else TimelineManager.Instance.SetTimeline(startTimeline.p1);

        ConvertToOtherPlayer(!photonView.IsMine);
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
            GameManager.photonPlayer = photonView.Owner;

            SetLayerRecursively(gameObject, playerLayer); //Player Layer
            cam.cullingMask |= (1 << LayerMask.NameToLayer("ItemInvisibleToOther")); //Culling Mask
            GameManager.hud = transform.GetComponentInChildren<HUD>();
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
        if ((~LayerMask.GetMask("Item", "ItemInvisibleToPlayer", "ItemInvisibleToOther", "ItemInvisibleToBothPlayers") & (1 << obj.layer)) != 0) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}
