using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Pickup : MonoBehaviour
{
    [SerializeField] float reachDistance = 4f;
    public static Item itemHolding = null;
    PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, reachDistance))
                {
                    Item item = hit.transform.GetComponent<Item>();
                    if (item != null)
                    {
                        photonView.RPC("PickUp", RpcTarget.AllBuffered, Client.photonPlayer, item.PhotonView.ViewID);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (!DropsInWall())
                {
                    Debug.LogError("Make every player called " + Client.photonPlayer.NickName + " drop their item");
                    photonView.RPC("Drop", RpcTarget.AllBuffered, Client.photonPlayer);
                }
            }
        }
    }

    [PunRPC]
    public void PickUp(Photon.Realtime.Player owner, int id)
    {
        Item item = PhotonNetwork.GetPhotonView(id).GetComponent<Item>();

        item.OnPickup(); //Event

        item.RequestOwnership(owner); //Ownership

        item.rb.isKinematic = true; //Rigidbody

        //Transform
        item.transform.SetParent(transform); //Parent
        item.transform.localPosition = item.positionOffset; //Position
        item.transform.localEulerAngles = item.rotationOffset; //Rotation

        itemHolding = item; 
    }

    [PunRPC]
    public void Drop(Photon.Realtime.Player owner)
    {
        if (itemHolding == null) return; //No items to drop

        itemHolding.OnDrop(); //Event
        itemHolding.rb.isKinematic = false; //Rigidbody
        itemHolding.transform.SetParent(itemHolding.startParent); //Reset parent
        itemHolding = null; //Set null
    }

    bool DropsInWall()
    {
        RaycastHit raycastHit;
        Vector3 raycastPosition = itemHolding.transform.parent.parent.position;
        raycastPosition += itemHolding.transform.parent.localPosition.x * itemHolding.transform.parent.right;
        Ray ray = new Ray(raycastPosition, itemHolding.transform.parent.forward);
        if (Physics.Raycast(ray, out raycastHit, 5f, ~LayerMask.GetMask("Held_Item_CurrentTL", "Held_Item_EveryTL", "Held_Item_OnlyCurrentTL")))
        {
            if (raycastHit.distance < itemHolding.transform.parent.localPosition.z)
            {
                //Cancel drop when item is in wall
                return true;
            }
        }

        return false;
    }
}
