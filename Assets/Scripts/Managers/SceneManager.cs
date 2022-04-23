using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public sealed class SceneManager : MonoBehaviourPunCallbacks
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeSinceLevelLoad > 0.5)
        {
            photonView.RPC("GotoMenu", RpcTarget.All);
        }
    }

    [PunRPC]
    void GotoMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //FIND A WAY TO GO TO HOST/JOIN MENU AFTER SWITCHING SCENES
        photonView.RPC("GotoMenu", RpcTarget.All);
        GameManager.roomStatus = GameManager.RoomStatus.LeavingRoom;
    }
}
