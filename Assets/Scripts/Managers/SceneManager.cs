using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public sealed class SceneManager : FullScreenManager
{
    PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            photonView.RPC("GotoMenu", RpcTarget.All);
        }

        ManageKeycodes();
    }

    [PunRPC]
    void GotoMenu()
    {
        Client.location = Client.Location.Menu;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
