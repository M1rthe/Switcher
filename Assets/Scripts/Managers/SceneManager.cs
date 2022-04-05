using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public sealed class SceneManager : InputManager
{
    //PhotonView photonView;

    //void Start()
    //{
    //    photonView = GetComponent<PhotonView>();
    //}

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
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
        photonView.RPC("GotoMenu", RpcTarget.All);
    }
}
