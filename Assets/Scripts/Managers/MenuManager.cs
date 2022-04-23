using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuManager : MonoBehaviour
{
    [HideInInspector] public PhotonView photonView;

    [SerializeField] GameObject mainMenu;
    [SerializeField] HostMenu hostMenu;
    [SerializeField] JoinMenu joinMenu;
    [SerializeField] LevelMenu levelMenu;

    [Space]

    public LoadWheel loadWheel;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (Time.frameCount < 10) 
        {
            Debug.LogError("GotoMainMenu");
            GotoMainMenu(); 
        }
        else if (GameManager.roomStatus == GameManager.RoomStatus.InRoom)
        {
            Debug.LogError("GotoLevelMenu");
            GotoLevelMenu();
        }
        else 
        {
            Debug.LogError("Escape");
            GotoLevelMenu();
            Escape();
        }

        //Cursor
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Escape();
        }
    }

    void Escape()
    {
        hostMenu.ClearError();
        joinMenu.ClearError();

        if (GameManager.roomStatus != GameManager.RoomStatus.OutRoom)
        {
            //Leave room
            if (GameManager.hostJoin != GameManager.HostJoin.Undefined)
            {
                //Make every client go back
                photonView.RPC("GoBack", RpcTarget.All, GameManager.hostJoin == GameManager.HostJoin.Host);
            }
        }
        else
        {
            //Go back when nobody joined yet
            GotoMainMenu();
        }
    }

    [PunRPC]
    public void GoBack(bool hostLeft)
    {
        StartCoroutine("Wait4GoBack", hostLeft);
    }

    IEnumerator Wait4GoBack(bool leave = true)
    {
        Debug.LogError("Wait4GoBack");
        GameManager.HostJoin hj = GameManager.hostJoin; //Because set to unidentified onLeft

        if (leave && hj == GameManager.HostJoin.Host) { hostMenu.LeaveRoom(); }
        if (hj == GameManager.HostJoin.Join) { joinMenu.LeaveRoom(); }

        while ((hj == GameManager.HostJoin.Host && leave && hj == GameManager.HostJoin.Join) && GameManager.roomStatus != GameManager.RoomStatus.OutRoom)
        {
            yield return null;
        }

        Debug.LogError("GotoHostServer/GotoJoinServer");

        if (levelMenu.gameObject.activeSelf ||
        hostMenu.gameObject.activeSelf ||
        joinMenu.gameObject.activeSelf) 
        {
            if (hj == GameManager.HostJoin.Host) { GotoHostServer(); }
            if (hj == GameManager.HostJoin.Join) { GotoJoinServer(); }
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
        loadWheel.Load = true;

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
        loadWheel.Load = true;

        while (GameManager.lobbyStatus != GameManager.LobbyStatus.InLobby)
        {
            yield return null;
        }

        loadWheel.Load = false;

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
        Cursor.visible = GameManager.hostJoin == GameManager.HostJoin.Host;

        mainMenu.SetActive(false);
        hostMenu.gameObject.SetActive(false);
        joinMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);
        levelMenu.AddLevelUI();
    }
}
