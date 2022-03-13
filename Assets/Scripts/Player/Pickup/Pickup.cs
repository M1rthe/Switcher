using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] float reachDistance = 4f;
    public static Item itemHolding = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, reachDistance))
            {
                Item item = hit.transform.GetComponent<Item>();
                if (item != null)
                {
                    item.OnPickup(); //Event

                    item.rb.isKinematic = true; //Rigidbody

                    //Transform
                    item.transform.SetParent(transform); //Parent
                    item.transform.localPosition = item.positionOffset; //Position
                    item.transform.localEulerAngles = item.rotationOffset; //Rotation

                    itemHolding = item;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Drop();
        }
    }

    public static void Drop()
    {
        if (itemHolding == null) return; //No items to drop

        RaycastHit raycastHit;
        Vector3 raycastPosition = itemHolding.transform.parent.parent.position;
        raycastPosition += itemHolding.transform.parent.localPosition.x * itemHolding.transform.parent.right;
        Ray ray = new Ray(raycastPosition, itemHolding.transform.parent.forward);
        if (Physics.Raycast(ray, out raycastHit, 5f, ~LayerMask.GetMask("Held_Item_CurrentTL", "Held_Item_EveryTL", "Held_Item_OnlyCurrentTL")))
        {
            if (raycastHit.distance < itemHolding.transform.parent.localPosition.z)
            {
                //Cancel drop when item is in wall
                return;
            }
        }

        itemHolding.OnDrop(); //Event
        itemHolding.rb.isKinematic = false; //Rigidbody
        itemHolding.transform.SetParent(itemHolding.startParent); //Reset parent
        itemHolding = null; //Set null
    }
}
