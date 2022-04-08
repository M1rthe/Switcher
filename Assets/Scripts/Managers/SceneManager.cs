using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public sealed class SceneManager : MonoBehaviourPunCallbacks
{
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GameManager.Instance.view.RPC("GoToMenu", RpcTarget.All, GameManager.Location.HostJoinMenu, 0);
    }
}
