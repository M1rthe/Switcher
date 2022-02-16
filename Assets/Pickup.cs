using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] float reachDistance = 4f;
    LayerMask hitMask;

    Item itemHolding = null;

    void Start()
    {
        hitMask = LayerMask.GetMask("Item");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, reachDistance, ~hitMask))
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
            if (itemHolding != null)
            {
                itemHolding.OnDrop(); //Event
                itemHolding.rb.isKinematic = false; //Rigidbody
                itemHolding.transform.SetParent(itemHolding.startParent); //Reset parent
                itemHolding = null; //Set null
            }
        }
    }
}
