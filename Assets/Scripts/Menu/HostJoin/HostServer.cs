using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;

public class HostServer : MonoBehaviourPunCallbacks
{
    [SerializeField] MenuManager menuManager;

    [SerializeField] Text error;
    [SerializeField] Button createServerButton;
    [SerializeField] Button playButton;
    [SerializeField] Text playButtonText;

    void Start()
    {
        playButtonText.text = "Play";

        ClearError();
    }

    public override void OnCreatedRoom() 
    {
        Client.roomStatus = Client.RoomStatus.InRoom;
        Client.hostJoin = Client.HostJoin.Host;

        createServerButton.interactable = false;
        playButtonText.text = "Play (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2 players)";

        StartCoroutine("Wait4OtherPlayers");
    }

    public override void OnLeftRoom() 
    {
        Client.roomStatus = Client.RoomStatus.OutRoom; 
        Client.hostJoin = Client.HostJoin.Undefined;

        createServerButton.interactable = true;
        playButtonText.text = "Play";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) { Debug.LogError("Enter room"); playButton.interactable = true; }
        playButtonText.text = "Play (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2 players)";
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        playButtonText.text = "Play (" + PhotonNetwork.CurrentRoom.PlayerCount + "/2 players)";
    }

    public override void OnEnable()
    {
        base.OnEnable();

        createServerButton.interactable = true;
        playButton.interactable = false;
    }

    public void ClearError()
    {
        error.text = "";
        error.gameObject.SetActive(false);
    }

    void GiveError(string errorMsg)
    {
        error.gameObject.SetActive(true);
        error.text = " Error: " + errorMsg;
    }

    public void CreateRoom(InputField roomName)
    {
        if (roomName.text.Length == 0) //Nothing is filled in
        {
            GiveError("Fill in all fields"); 
            return;
        }

        if (Client.roomStatus == Client.RoomStatus.OutRoom)
        {
            Client.roomStatus = Client.RoomStatus.JoiningRoom;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.NickName = roomName.text;
            string id = System.Guid.NewGuid().ToString("N");
            PhotonNetwork.CreateRoom(id, roomOptions);
            //Debug.LogError("ID: " + id);
            GUIUtility.systemCopyBuffer = id;
        }
        else if (Client.roomStatus == Client.RoomStatus.InRoom) Debug.LogError("Already entered room");
        else Debug.LogError("Still leaving room");
    }

    public void Play()
    {
        Debug.LogError("Play : GotoLevelMenu");

        menuManager.photonView.RPC("GotoLevelMenu", RpcTarget.All, Client.hostJoin.ToString());
        Client.hostJoin = Client.HostJoin.Host;
    }

    IEnumerator Wait4OtherPlayers()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount < 2 && Client.roomStatus == Client.RoomStatus.InRoom)
        {
            yield return null;
        }

        if (Client.roomStatus == Client.RoomStatus.InRoom)
        {
            playButton.interactable = true;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("OnCreateRoomFailed");
    }

    public void LeaveRoom()
    {
        Client.roomStatus = Client.RoomStatus.LeavingRoom;
        PhotonNetwork.LeaveRoom();
        OnLeftRoom();
    }
}
