using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

namespace Photon.Pun
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        //Instance
        public static GameManager Instance { get; private set; }

        //Managers
        public MenuManager menuManager = null;

        //Photon
        //public PhotonView view;
        [HideInInspector] public Realtime.Player photonPlayer;

        //Network Status
        public enum HostJoin { Host, Join, Undefined }
        public HostJoin hostJoin = HostJoin.Undefined;
        public enum RoomStatus { JoiningRoom, LeavingRoom, InRoom, OutRoom }
        public RoomStatus roomStatus = RoomStatus.OutRoom;
        public enum LobbyStatus { JoiningLobby, LeavingLobby, InLobby, OutLobby }
        public LobbyStatus lobbyStatus = LobbyStatus.OutLobby;

        //Type Player
        public enum PlayerType { PastPresent, FuturePresent }
        public PlayerType playerType;

        //Menu
        public enum Location { MainMenu, HostJoinMenu, LevelMenu, Level }
        public Location location;

        void Start()
        {
            GoTo(Location.MainMenu, 0);
        }

        [PunRPC]
        private void Destroy()
        {
            Destroy(this.gameObject);
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("Destroy duplicate: " + gameObject.name);
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                Debug.LogError("Instantiate " + gameObject.name);
                DontDestroyOnLoad(gameObject);
            }
        }

        public static int GetCurrentScene() { return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex; }

        public static int GetSceneCount() { return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; }

        [PunRPC]
        public void GoTo(Location l, int sceneIndex = 0)
        {
            location = l;
            Debug.LogError("Go To "+l.ToString());

            if (sceneIndex != GetCurrentScene())
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            }

            if (sceneIndex == 0)
            {
                StartCoroutine(OnSceneLoaded(sceneIndex, delegate
                {
                    Debug.LogError("Loaded scene " + sceneIndex + ", goto: "+ location.ToString());

                    menuManager = FindObjectOfType<MenuManager>();

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    switch (location)
                    {
                        case Location.MainMenu:
                            menuManager.GotoMainMenu();
                            break;
                        case Location.HostJoinMenu:
                            if (hostJoin == HostJoin.Host) menuManager.GotoHostServer();
                            if (hostJoin == HostJoin.Join) menuManager.GotoJoinServer();
                            break;
                        case Location.LevelMenu:
                            menuManager.GoToLevelMenu();
                            break;
                        default:
                            break;
                    }
                }));
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        IEnumerator OnSceneLoaded(int sceneIndex, UnityAction unityAction)
        {
            while (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded == false)
            {
                yield return null;
            }

            unityAction.Invoke();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (location == Location.Level)
                {
                    Debug.LogError("Go to level menu!!!!!");
                    photonView.RPC("GoTo", RpcTarget.All, Location.LevelMenu, 0);
                    photonView.RPC("Destroy", RpcTarget.All);
                }
                else //Menu
                {
                    if (roomStatus != RoomStatus.OutRoom)
                    {
                        //Leave room
                        if (hostJoin != HostJoin.Undefined)
                        {
                            //Make every client go back
                            menuManager.photonView.RPC("LeaveAndGoBack", RpcTarget.All, hostJoin == HostJoin.Host);
                        }
                    }
                    else
                    {
                        //Go back when nobody joined yet
                        menuManager.GotoMainMenu();
                    }
                }
            }

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

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            GoTo(Location.HostJoinMenu, 0);
        }
    }
}
