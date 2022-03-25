using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviour
{
    public string itemName = "";

    public Vector3 positionOffset = new Vector3(0, 0, 0);
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform startParent;
    [HideInInspector] public bool inHand = false;

    MeshRenderer meshRenderer;

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
        gameObject.layer = itemLayer; //Set layer

        //Parent
        startParent = transform.parent; //Get Parent
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = 0; //Cast Shadows OFF
                
        //Rigidbody
        rb = GetComponent<Rigidbody>(); //Get rigidbody
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>(); //Add rigidbody
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

        if (invisibleToPlayer) gameObject.layer = itemInvisibleToPlayerLayer;
        if (invisibleToOtherPlayer) gameObject.layer = itemInvisibleToOtherLayer;
        if (invisibleToPlayer && invisibleToOtherPlayer) gameObject.layer = itemInvisibleToBothPlayersLayer;
        if (!invisibleToPlayer && !invisibleToOtherPlayer) gameObject.layer = itemLayer;

        //Debug.LogError("UpdateLayer() layer : " + LayerMask.LayerToName(gameObject.layer));
    }

    public virtual void OnPickup()
    {
        inHand = true;
    }

    public virtual void OnDrop()
    {
        inHand = false; 
    }
}
