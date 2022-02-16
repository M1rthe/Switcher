using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName = "";

    public Vector3 positionOffset = new Vector3(0, 0, 0);
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform startParent;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Item"); //Set layer
        startParent = transform.parent; //Get Parent
        GetComponent<MeshRenderer>().shadowCastingMode = 0; //Cast Shadows OFF
                                                          
        rb = GetComponent<Rigidbody>(); //Get rigidbody
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>(); //Add rigidbody
    }

    public virtual void OnPickup()
    {

    }

    public virtual void OnDrop()
    {

    }
}
