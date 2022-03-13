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
        else GotoLevelMenu();

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
                    photonView.RPC("GoBack", RpcTarget.AllBuffered, Client.hostJoin == Client.HostJoin.Host);
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
        if (Client.hostJoin == Client.HostJoin.Host && hostLeft)
        {
            StartCoroutine("Wait4GoBack");
            hostServer.LeaveRoom();
        }
        if (Client.hostJoin == Client.HostJoin.Join)
        {
            StartCoroutine("Wait4GoBack");
            joinServer.LeaveRoom();
        }
    }

    IEnumerator Wait4GoBack()
    {
        while (Client.roomStatus != Client.RoomStatus.OutRoom)
        {
            yield return null;
        }

        if (levelMenu.gameObject.activeSelf ||
        hostServer.gameObject.activeSelf ||
        joinServer.gameObject.activeSelf) 
        { 
            if (Client.hostJoin == Client.HostJoin.Host) GotoHostServer(); 
            if (Client.hostJoin == Client.HostJoin.Join) GotoJoinServer(); 
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
    public void GotoLevelMenu()
    {
        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = Client.hostJoin == Client.HostJoin.Host;

        mainMenu.SetActive(false);
        hostServer.gameObject.SetActive(false);
        joinServer.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);

        levelMenu.AddLevelUI();
    }
}
