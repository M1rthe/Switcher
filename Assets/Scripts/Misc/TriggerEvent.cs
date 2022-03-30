using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    void OnTriggerEnter(Collider other)
    {
        if ((~layerMask & (1 << other.gameObject.layer)) != 0) return; //Collision with layermask

        onTriggerEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if ((~layerMask & (1 << other.gameObject.layer)) != 0) return; //Collision with layermask

        onTriggerExit.Invoke();
    }
}
