using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager Instance { get; private set; }

    //Photon Player
    public static Photon.Realtime.Player photonPlayer;

    //Scenes
    public static int GetCurrentScene() { return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex; }

    //Network Status
    public enum HostJoin { Host, Join, Undefined }
    public static HostJoin hostJoin = HostJoin.Undefined;
    public enum RoomStatus { JoiningRoom, LeavingRoom, InRoom, OutRoom }
    public static RoomStatus roomStatus = RoomStatus.OutRoom;
    public enum LobbyStatus { JoiningLobby, LeavingLobby, InLobby, OutLobby }
    public static LobbyStatus lobbyStatus = LobbyStatus.OutLobby;

    //Type Player
    public enum PlayerType { PastPresent, FuturePresent }
    public static PlayerType playerType;

    //SINGLETON
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
                if (Cursor.lockState == CursorLockMode.Confined) Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Screen.fullScreen = true;
                if (Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }
}
