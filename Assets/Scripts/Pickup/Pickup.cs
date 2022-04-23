using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviour
{
    [SerializeField] float reachDistance = 4f;
    [HideInInspector] public Item itemHolding = null;

    [HideInInspector] public PhotonView photonView;

    Camera cam;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        cam = GetComponentInParent<Camera>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            //Pickup
            if (Input.GetMouseButtonDown(0))
            {
                if (itemHolding == null)
                {
                    //Raycast check
                    RaycastHit hit;
                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, reachDistance))
                    {
                        //Hit item component
                        Item item = hit.transform.GetComponent<Item>();
                        if (item != null)
                        {
                            //Let this player pickup this item for every player
                            photonView.RPC("PickUp", RpcTarget.All, GameManager.photonPlayer, item.PhotonView.ViewID);
                        }
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
                        photonView.RPC("Drop", RpcTarget.All, GameManager.photonPlayer, itemHolding.transform.position, itemHolding.transform.eulerAngles);
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

        item.RequestOwnership(owner); //Ownership

        item.rb.isKinematic = true; //Rigidbody

        //Transform
        item.transform.SetParent(transform); //Parent
        item.transform.localPosition = item.positionOffset; //Position
        item.transform.localEulerAngles = item.rotationOffset; //Rotation

        item.OnPickup(); //Event

        itemHolding = item;
    }

    [PunRPC]
    public void Drop(Photon.Realtime.Player owner, Vector3 pos, Vector3 rot)
    {
        itemHolding.transform.position = pos;
        itemHolding.transform.eulerAngles = rot;

        itemHolding.rb.isKinematic = false; //Rigidbody
        itemHolding.transform.SetParent(itemHolding.startParent); //Reset parent

        itemHolding.OnDrop(); //Event

        itemHolding = null; //Set null
    }

    bool DropsInWall()
    {
        //Send ray
        Vector3 raycastPosition = itemHolding.transform.parent.parent.position; //Raypos
        raycastPosition += itemHolding.transform.parent.localPosition.x * itemHolding.transform.parent.right; //Offset
        Ray ray = new Ray(raycastPosition, itemHolding.transform.parent.forward);
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 5f, ~LayerMask.GetMask("ItemPlayer", "ItemOtherPlayer", "ItemBothPlayers", "ItemNeitherPlayers"));
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.distance < itemHolding.transform.parent.localPosition.z && hit.collider.isTrigger == false)
            {
                //Cancel drop when item is in wall
                return true;
            }
        }

        return false;
    }
}
