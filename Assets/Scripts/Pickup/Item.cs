using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviour
{
    [Space(25)]
    [Header("Requires: Collider, Rigidbody & PhotonView")]
    public string itemName = "";

    public Vector3 positionOffset = new Vector3(0, 0, 0);
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform startParent;
    [HideInInspector] public bool inHand = false;

    MeshRenderer[] meshRenderers;

    //PhotonView
    private PhotonView photonView;
    public PhotonView PhotonView
    {
        get { return photonView; }
        private set { photonView = value; }
    }

    //Type
    public enum Type
    {
        CurrentTL,
        OnlyCurrentTL,
        EveryTL
    }
    public Type type;

    //Start timeline
    int startTimeline;
    public int StartTimeline
    {
        get { return startTimeline; }
        set { photonView.RPC("SetStartTimeline", RpcTarget.All, value); }
    }

    //Layers
    LayerMask itemLayer;
    LayerMask itemInvisibleToPlayerLayer;
    LayerMask itemInvisibleToOtherLayer;
    LayerMask itemInvisibleToBothPlayersLayer;

    void Start()
    {
        PhotonView = GetComponent<PhotonView>();

        //Start timeline
        startTimeline = GetComponentInParent<Timeline>().transform.GetSiblingIndex();

        //Get layers
        itemLayer = LayerMask.NameToLayer("Item");
        itemInvisibleToPlayerLayer = LayerMask.NameToLayer("ItemInvisibleToPlayer");
        itemInvisibleToOtherLayer = LayerMask.NameToLayer("ItemInvisibleToOther");
        itemInvisibleToBothPlayersLayer = LayerMask.NameToLayer("ItemInvisibleToBothPlayers");
        SetLayerRecursively(gameObject, itemLayer); //Set layer

        //Parent
        startParent = transform.parent; //Get Parent
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers) meshRenderer.shadowCastingMode = 0; //Cast Shadows OFF

        //Rigidbody
        rb = GetComponent<Rigidbody>(); //Get rigidbody
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>(); //Add rigidbody

        //Tag
        gameObject.tag = "Item";
    }

    public void RequestOwnership(Photon.Realtime.Player photonPlayer)
    {
        photonView.TransferOwnership(photonPlayer);
    }

    [PunRPC]
    public void SetStartTimeline(int timeline)
    {
        startTimeline = timeline;
        startParent = TimelineManager.Timelines[timeline].transform;
    }

    [PunRPC]
    public void UpdateLayer()
    {
        bool invisibleToPlayer = StartTimeline != TimelineManager.CurrentTimeline;
        bool invisibleToOtherPlayer = StartTimeline != TimelineManager.CurrentTimelineOtherPlayer;

        SetColorTransparent(false);

        if (invisibleToPlayer) //Invisble to player
        {
            if (type == Type.CurrentTL)
            {
                SetLayerRecursively(gameObject, itemInvisibleToOtherLayer);
                SetColorTransparent(true);
            }
            if (type == Type.OnlyCurrentTL)
            {
                SetLayerRecursively(gameObject, itemInvisibleToPlayerLayer);
            }
        }

        if (invisibleToOtherPlayer) //Invisible to other player
        {
            SetLayerRecursively(gameObject, itemInvisibleToOtherLayer);
        }

        if (invisibleToPlayer && invisibleToOtherPlayer) //Invisble to both
        {
            if (type == Type.CurrentTL)
            {
                SetLayerRecursively(gameObject, itemLayer);
                SetColorTransparent(true);
            }
            if (type == Type.OnlyCurrentTL)
            {
                SetLayerRecursively(gameObject, itemInvisibleToBothPlayersLayer);
            }
        }

        if (!invisibleToPlayer && !invisibleToOtherPlayer) //Visible to both
        {
            SetLayerRecursively(gameObject, itemLayer);
        }
    }

    public virtual void OnPickup()
    {
        inHand = true;
    }

    public virtual void OnDrop()
    {
        inHand = false;
    }

    public void SetColorTransparent(bool enable)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            Color c = meshRenderer.material.GetColor("_BaseColor");
            if (enable) c.a = 0.25f;
            else c.a = 1f;
            meshRenderer.material.SetColor("_BaseColor", c);
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}