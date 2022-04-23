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
    LayerMask itemPlayerLayer;
    LayerMask itemOtherPlayerLayer;
    LayerMask itemBothPlayersLayer;
    LayerMask itemNeitherPlayersLayer;

    protected Camera cam;

    public virtual void Start()
    {
        PhotonView = GetComponent<PhotonView>();

        //Start timeline
        startTimeline = GetComponentInParent<Timeline>().transform.GetSiblingIndex();

        //Get layers
        itemPlayerLayer = LayerMask.NameToLayer("ItemPlayer");
        itemOtherPlayerLayer = LayerMask.NameToLayer("ItemOtherPlayer");
        itemBothPlayersLayer = LayerMask.NameToLayer("ItemBothPlayers");
        itemNeitherPlayersLayer = LayerMask.NameToLayer("ItemNeitherPlayers");

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

    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (inHand)
            {
                //Raycast check
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 2f, ~LayerMask.GetMask("IgnorePlayer")))
                {
                    Use(hit.transform.gameObject);
                }
            }
        }
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

        StartTimelineSwitched();
    }

    [PunRPC]
    public void UpdateLayer()
    {
        bool onPlayerTimeline = StartTimeline == TimelineManager.CurrentTimeline;
        bool onOtherPlayerTimeline = StartTimeline == TimelineManager.CurrentTimelineOtherPlayer;

        if (onPlayerTimeline && onOtherPlayerTimeline)
        {
            if (type == Type.CurrentTL)
            {
                SetLayerRecursively(gameObject, itemBothPlayersLayer);
            }
            if (type == Type.OnlyCurrentTL)
            {
                SetLayerRecursively(gameObject, itemBothPlayersLayer);
            }

            return;
        }

        if (!onPlayerTimeline && !onOtherPlayerTimeline) //Visible to both
        {
            SetLayerRecursively(gameObject, itemNeitherPlayersLayer);
            return;
        }

        if (onPlayerTimeline) //Invisble to player
        {
            if (type == Type.CurrentTL)
            {
                SetLayerRecursively(gameObject, itemPlayerLayer);
            }
            if (type == Type.OnlyCurrentTL)
            {
                SetLayerRecursively(gameObject, itemPlayerLayer);
            }

            return;
        }

        if (onOtherPlayerTimeline) //Invisible to other player
        {
            SetLayerRecursively(gameObject, itemOtherPlayerLayer);

            return;
        }
    }

    public virtual void Use(GameObject go)
    {

    }

    public virtual void StartTimelineSwitched()
    {

    }

    public virtual void OnPickup()
    {
        inHand = true;
        cam = GameManager.GetComponentInParentRecursively(gameObject, typeof(Camera)) as Camera;
    }

    public virtual void OnDrop()
    {
        inHand = false;
    }

    //public void SetColorTransparent(bool enable)
    //{
    //    foreach (MeshRenderer meshRenderer in meshRenderers)
    //    {
    //        foreach (Material material in meshRenderer.materials)
    //        {
    //            Color c = material.GetColor("_BaseColor");
    //            if (enable) c.a = 0.25f;
    //            else c.a = 1f;
    //            material.SetColor("_BaseColor", c);
    //        }
    //    }
    //}

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}