using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager Instance { get; private set; }

    public AnimationCurve lightFlareCurve;

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

    public static GameObject player;
    public static HUD hud;
    public static Transform spawnPoint;

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

    public static Component GetComponentInParentRecursively(GameObject go, System.Type type)
    {
        Component component = go.GetComponent(type);
        if (component != null)
        {
            return component;
        }
        else
        {
            if (go.transform.parent != null)
            {
                return GetComponentInParentRecursively(go.transform.parent.gameObject, type);
            }
            else
            {
                return null;
            }
        }
    }

    public static GameObject FindChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.gameObject;
            }
        }

        return null;
    }
}
