using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Level2MissionCheck : MissionCheck
{
    void OnTriggerEnter(Collider other)
    {
        if (GameManager.hostJoin == GameManager.HostJoin.Host)
        {
            if (other.CompareTag("Player"))
            {
                base.photonView.RPC("Succes", RpcTarget.All, timePassed, timesSwitched);
            }
        }
    }

    [PunRPC]
    public override void Switched()
    {
        base.Switched();
    }
}
