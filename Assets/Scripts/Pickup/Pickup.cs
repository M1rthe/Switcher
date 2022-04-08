using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviour
{
    [SerializeField] float reachDistance = 4f;
    [HideInInspector] public Item itemHolding = null;

    PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            //Pickup
            if (Input.GetMouseButtonDown(0))
            {
                //Raycast check
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, reachDistance))
                {
                    //Hit item component
                    Item item = hit.transform.GetComponent<Item>();
                    if (item != null)
                    {
                        //Let this player pickup this item for every player
                        photonView.RPC("PickUp", RpcTarget.All, GameManager.Instance.photonPlayer, item.PhotonView.ViewID);
                    }
                }
            }

            //Drop
            if (Input.GetMouseButtonDown(1))
            {
                if (itemHolding != null)
                {
                    if (!DropsInWall()) //WallCheck
                    {
                        //Let this player drop for all players
                        photonView.RPC("Drop", RpcTarget.All, GameManager.Instance.photonPlayer);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void PickUp(Photon.Realtime.Player owner, int id)
    {
        //Get item by id
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
        itemHolding.OnDrop(); //Event
        itemHolding.rb.isKinematic = false; //Rigidbody
        itemHolding.transform.SetParent(itemHolding.startParent); //Reset parent
        itemHolding = null; //Set null
    }

    bool DropsInWall()
    {
        //Send ray
        RaycastHit raycastHit;
        Vector3 raycastPosition = itemHolding.transform.parent.parent.position; //Raypos
        raycastPosition += itemHolding.transform.parent.localPosition.x * itemHolding.transform.parent.right; //Offset
        Ray ray = new Ray(raycastPosition, itemHolding.transform.parent.forward);
        if (Physics.Raycast(ray, out raycastHit, 5f, ~LayerMask.GetMask("Item", "ItemInvisibleToPlayer", "ItemInvisibleToOther", "ItemInvisibleToBothPlayers")))
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
