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

    void Start()
    {
        photonView = GetComponent<PhotonView>();
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

    public void GotoHostServer()
    {
        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = true;

        mainMenu.SetActive(false);
        hostMenu.gameObject.SetActive(true);
        joinMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
    }

    public void GotoJoinServer()
    {
        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = true;

        mainMenu.SetActive(false);
        hostMenu.gameObject.SetActive(false);
        joinMenu.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
    }

    [PunRPC]
    public void GoToLevelMenu()
    {
        Cursor.lockState = CursorLockMode.None; //Cursor
        Cursor.visible = GameManager.Instance.hostJoin == GameManager.HostJoin.Host;

        mainMenu.SetActive(false);
        hostMenu.gameObject.SetActive(false);
        joinMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);
        levelMenu.AddLevelUI();
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    [PunRPC]
    public void LeaveAndGoBack(bool hostLeft)
    {
        hostMenu.ClearError();
        joinMenu.ClearError();

        StartCoroutine("LeaveWaitAndGoBack", hostLeft);
    }

    IEnumerator LeaveWaitAndGoBack(bool leave = true)
    {
        GameManager.HostJoin hj = GameManager.Instance.hostJoin; //Because set to unidentified onLeft

        //Leave ROOM
        if (leave && hj == GameManager.HostJoin.Host) { hostMenu.LeaveRoom(); }
        if (hj == GameManager.HostJoin.Join) { joinMenu.LeaveRoom(); }

        while ((hj == GameManager.HostJoin.Host && leave && hj == GameManager.HostJoin.Join) && GameManager.Instance.roomStatus != GameManager.RoomStatus.OutRoom)
        {
            //Still in room
            yield return null;
        }

        //Leave visually
        if (levelMenu.gameObject.activeSelf ||
        hostMenu.gameObject.activeSelf ||
        joinMenu.gameObject.activeSelf)
        {
            if (hj == GameManager.HostJoin.Host) { GotoHostServer(); }
            if (hj == GameManager.HostJoin.Join) { GotoJoinServer(); }
        }
    }
}
