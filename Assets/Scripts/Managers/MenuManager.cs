using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuManager : FullScreenManager
{
    public PhotonView photonView;
    [SerializeField] GameObject mainMenu;
    [SerializeField] HostServer hostServer;
    [SerializeField] JoinServer joinServer;
    [SerializeField] LevelMenu levelMenu;

    void Start()
    {
        if (Time.frameCount < 10) GotoMainMenu();
        else { GotoLevelMenu(); }

        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        ManageKeycodes();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hostServer.ClearError();
            joinServer.ClearError();

            if (Client.roomStatus != Client.RoomStatus.OutRoom)
            {
                //Leave room
                if (Client.hostJoin != Client.HostJoin.Undefined)
                {
                    //Make every client go back
                    photonView.RPC("GoBack", RpcTarget.All, Client.hostJoin == Client.HostJoin.Host);
                }
            }
            else
            {
                //Go back when nobody joined yet
                GotoMainMenu();
            }
        }
    }

    [PunRPC]
    public void GoBack(bool hostLeft)
    {
        StartCoroutine("Wait4GoBack", hostLeft);
    }

    IEnumerator Wait4GoBack(bool leave = true)
    {
        Client.HostJoin hj = Client.hostJoin; //Because set to unidentified onLeft

        if (leave && hj == Client.HostJoin.Host) { hostServer.LeaveRoom(); }
        if (hj == Client.HostJoin.Join) { joinServer.LeaveRoom(); }

        Debug.LogError("Go back");

        while ((hj == Client.HostJoin.Host && leave && hj == Client.HostJoin.Join) && Client.roomStatus != Client.RoomStatus.OutRoom)
        {
            yield return null;
        }

        if (levelMenu.gameObject.activeSelf ||
        hostServer.gameObject.activeSelf ||
        joinServer.gameObject.activeSelf) 
        {
            Debug.LogError("Go REALLY back");

            if (hj == Client.HostJoin.Host) { GotoHostServer(); }
            if (hj == Client.HostJoin.Join) { GotoJoinServer(); }
        }
    }

    public void GotoMainMenu()
    {
        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = true;

        mainMenu.SetActive(true);
        hostServer.gameObject.SetActive(false);
        joinServer.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
    }

    public void GotoHostServer()
    {
        StartCoroutine("Wait4GotoHostServer");
    }

    IEnumerator Wait4GotoHostServer()
    {
        while (Client.lobbyStatus != Client.LobbyStatus.InLobby)
        {
            yield return null;
        }

        mainMenu.SetActive(false);
        hostServer.gameObject.SetActive(true);
        joinServer.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
    }

    public void GotoJoinServer()
    {
        mainMenu.SetActive(false);
        hostServer.gameObject.SetActive(false);
        joinServer.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
    }

    [PunRPC]
    public void GotoLevelMenu(string test = "default")
    {
        Debug.LogError(test.ToString() + " GotoLevelMenu");

        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = Client.hostJoin == Client.HostJoin.Host;

        mainMenu.SetActive(false);
        hostServer.gameObject.SetActive(false);
        joinServer.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);

        levelMenu.AddLevelUI();
    }
}
