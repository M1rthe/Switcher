using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Level1MissionCheck : MissionCheck
{
    [SerializeField] GameObject cat;

    void OnTriggerEnter(Collider other)
    {
        if (Client.hostJoin == Client.HostJoin.Host)
        {
            if (other.transform.gameObject == cat)
            {
                Debug.LogError("YOU WON!!!!!");
                base.photonView.RPC("Succes", Photon.Pun.RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public override void Switched()
    {
        base.Switched();
    }
}
