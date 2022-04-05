using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager instance;

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
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
