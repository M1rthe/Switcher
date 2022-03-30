using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuManager : InputManager
{
    [HideInInspector] public PhotonView photonView;

    [SerializeField] GameObject mainMenu;
    [SerializeField] HostMenu hostMenu;
    [SerializeField] JoinMenu joinMenu;
    [SerializeField] LevelMenu levelMenu;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (Time.frameCount < 10) { GotoMainMenu(); }
        else { GotoLevelMenu(); }

        //Cursor
        Cursor.lockState = CursorLockMode.None;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hostMenu.ClearError();
            joinMenu.ClearError();

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

        if (leave && hj == Client.HostJoin.Host) { hostMenu.LeaveRoom(); }
        if (hj == Client.HostJoin.Join) { joinMenu.LeaveRoom(); }

        while ((hj == Client.HostJoin.Host && leave && hj == Client.HostJoin.Join) && Client.roomStatus != Client.RoomStatus.OutRoom)
        {
            yield return null;
        }

        if (levelMenu.gameObject.activeSelf ||
        hostMenu.gameObject.activeSelf ||
        joinMenu.gameObject.activeSelf) 
        {
            if (hj == Client.HostJoin.Host) { GotoHostServer(); }
            if (hj == Client.HostJoin.Join) { GotoJoinServer(); }
        }
    }

    public void GotoMainMenu()
    {
        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = true;

        mainMenu.SetActive(true);
        hostMenu.gameObject.SetActive(false);
        joinMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
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
        hostMenu.gameObject.SetActive(true);
        joinMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
    }

    public void GotoJoinServer()
    {
        mainMenu.SetActive(false);
        hostMenu.gameObject.SetActive(false);
        joinMenu.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
    }

    [PunRPC]
    public void GotoLevelMenu()
    {
        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = Client.hostJoin == Client.HostJoin.Host;

        mainMenu.SetActive(false);
        hostMenu.gameObject.SetActive(false);
        joinMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);
        levelMenu.AddLevelUI();
    }
}
