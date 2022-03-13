using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class JoinServer : MonoBehaviourPunCallbacks
{
    [SerializeField] MenuManager menuManager;

    [SerializeField] Text error;
    [SerializeField] Button joinButton;
    [SerializeField] Text joinButtonText;

    void Start()
    {
        ClearError();
    }

    public override void OnJoinedRoom() 
    {
        joinButton.interactable = false;
        joinButtonText.text = "Joined";

        Client.hostJoin = Client.HostJoin.Join;
        Client.roomStatus = Client.RoomStatus.InRoom;
        ClearError();
    }
    public override void OnLeftRoom() 
    {
        joinButton.interactable = true;
        joinButtonText.text = "Join";

        Client.roomStatus = Client.RoomStatus.OutRoom;
        Client.hostJoin = Client.HostJoin.Undefined;
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

    public void JoinRoom(JoinForm form)
    {
        if (form.username.text.Length == 0 || form.code.text.Length == 0)
        {
            GiveError("Fill in all fields");
            return;
        }

        if (Client.roomStatus == Client.RoomStatus.OutRoom)
        {
            Client.roomStatus = Client.RoomStatus.JoiningRoom;

            PhotonNetwork.JoinRoom(form.code.text);
            PhotonNetwork.NickName = form.username.text;
        }
        else Debug.LogError("Already entered room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Client.roomStatus = Client.RoomStatus.OutRoom;

        switch (returnCode)
        {
            case 32765:
                GiveError($"Room is full");
            break;

            default:
                GiveError($"Incorrect code");
                break;
        }
    }

    public void LeaveRoom()
    {
        Client.roomStatus = Client.RoomStatus.LeavingRoom;
        PhotonNetwork.LeaveRoom();
    }
}