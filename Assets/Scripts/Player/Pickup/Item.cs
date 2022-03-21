using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Item : MonoBehaviour
{
    public string itemName = "";

    public Vector3 positionOffset = new Vector3(0, 0, 0);
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    private PhotonView photonView;
    public PhotonView PhotonView
    {
        get { return photonView; }
        private set { photonView = value; }
    }


    public enum TimelineExistance
    {
        CurrentTL,
        OnlyCurrentTL,
        EveryTL
    }
    public TimelineExistance timelineExistance;
    [HideInInspector] public int startTimeline;
    [HideInInspector] public LayerMask itemTypeLayer;
    [HideInInspector] public LayerMask itemHeldTypeLayer;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform startParent;

    [HideInInspector] public bool inHand = false;

    void Start()
    {
        PhotonView = GetComponent<PhotonView>();

        //Set layer
        if (timelineExistance == TimelineExistance.CurrentTL) itemTypeLayer = LayerMask.NameToLayer("Item_CurrentTL"); 
        if (timelineExistance == TimelineExistance.EveryTL) itemTypeLayer = LayerMask.NameToLayer("Item_EveryTL"); 
        if (timelineExistance == TimelineExistance.OnlyCurrentTL) itemTypeLayer = LayerMask.NameToLayer("Item_OnlyCurrentTL");
        startTimeline = GetComponentInParent<Timeline>().transform.GetSiblingIndex();
        itemHeldTypeLayer = LayerMask.NameToLayer("Held_" + LayerMask.LayerToName(itemTypeLayer));
        gameObject.layer = itemTypeLayer;

        //Parent
        startParent = transform.parent; //Get Parent
        GetComponent<MeshRenderer>().shadowCastingMode = 0; //Cast Shadows OFF
                
        //Rigidbody
        rb = GetComponent<Rigidbody>(); //Get rigidbody
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>(); //Add rigidbody
    }

    public void RequestOwnership(Photon.Realtime.Player photonPlayer)
    {
        photonView.TransferOwnership(photonPlayer);

        if (photonPlayer != Client.photonPlayer)
        { 
            gameObject.layer = itemTypeLayer;
        }
    }

    public virtual void OnPickup()
    {
        inHand = true;
        gameObject.layer = itemHeldTypeLayer;
    }

    public virtual void OnDrop()
    {
        inHand = false; 
        gameObject.layer = itemTypeLayer;
    }
}
