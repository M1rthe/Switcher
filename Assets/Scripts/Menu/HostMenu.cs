using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;

public class HostMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] MenuManager menuManager;
    [Space]
    [SerializeField] Text code;
    [SerializeField] Text error;
    [SerializeField] Button createServerButton;
    [SerializeField] Button playButton;
    [SerializeField] Text playButtonText;

    void Start()
    {
        playButtonText.text = "Play";
        ClearError();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        createServerButton.interactable = true;
        playButton.interactable = false;
        code.gameObject.SetActive(false);
    }

    void GiveError(string errorMsg)
    {
        error.gameObject.SetActive(true);
        error.text = " Error: " + errorMsg;
    }

    public void ClearError()
    {
        error.text = "";
        error.gameObject.SetActive(false);
    }

    public void CreateRoom(InputField username)
    {
        //When nothing is filled in, give error and return
        if (username.text.Length == 0) 
        {
            GiveError("Fill in all fields"); 
            return;
        }

        if (GameManager.roomStatus == GameManager.RoomStatus.OutRoom)
        {
            GameManager.roomStatus = GameManager.RoomStatus.JoiningRoom;

            //Create room
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2; //Max 2 platers
            PhotonNetwork.NickName = username.text; //Nickname
            string id = System.Guid.NewGuid().ToString("N"); //Generate code
            PhotonNetwork.CreateRoom(id, roomOptions); //Create Photon Room
            GUIUtility.systemCopyBuffer = id;

            //UI
            code.text = id;
            code.gameObject.SetActive(true);
        }
        else if (GameManager.roomStatus == GameManager.RoomStatus.InRoom) Debug.LogError("Already entered room");
        else if (GameManager.roomStatus == GameManager.RoomStatus.JoiningRoom) Debug.LogError("Already joining room");
        else Debug.LogError("Still leaving room");
    }

    public void Play()
    {
        //Make everyone go to level menu
        menuManager.photonView.RPC("GotoLevelMenu", RpcTarget.All);
    }

    public void LeaveRoom()
    {
        GameManager.roomStatus = GameManager.RoomStatus.LeavingRoom;
        PhotonNetwork.LeaveRoom();
        OnLeftRoom();
    }

    IEnumerator Wait4OtherPlayers()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount < 2 && GameManager.roomStatus == GameManager.RoomStatus.InRoom)
        {
            yield return null;
        }

        if (GameManager.roomStatus == GameManager.RoomStatus.InRoom)
        {
            playButton.interactable = true;
        }
    }

    /*########################################################*/
    /*## PHOTON CALLBACKS ####################################*/
    /*########################################################*/

    public override void OnCreatedRoom()
    {
        GameManager.roomStatus = GameManager.RoomStatus.InRoom;
        GameManager.hostJoin = GameManager.HostJoin.Host;

        createServerButton.interactable = false; //Cannot create server again

        //Change button text
        playButtonText.text = "Play (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2 players)";

        //Wait for other players
        StartCoroutine("Wait4OtherPlayers");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        Debug.LogError("OnCreateRoomFailed");
    }

    public override void OnLeftRoom()
    {
        GameManager.roomStatus = GameManager.RoomStatus.OutRoom;
        GameManager.hostJoin = GameManager.HostJoin.Undefined;

        createServerButton.interactable = true;
        playButtonText.text = "Play";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) { playButton.interactable = true; }

        //Update playercount on Play button
        playButtonText.text = "Play (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2 players)";
        UpdatePlayButtonInteractable();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogError(otherPlayer.NickName+" left room");
        //Update playercount on Play button
        playButtonText.text = "Play (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2 players)";
        UpdatePlayButtonInteractable();
    }

    void UpdatePlayButtonInteractable()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) playButton.interactable = true;
        else playButton.interactable = false;
    }
}
