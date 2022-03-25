using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class JoinMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] MenuManager menuManager;
    [Space]
    [SerializeField] InputField usernameInputfield;
    [SerializeField] InputField codeInputfield;
    [SerializeField] Text error;
    [SerializeField] Button joinButton;
    [SerializeField] Text joinButtonText;

    void Start()
    {
        ClearError();
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

    public void JoinRoom()
    {
        //Fill in all fields
        if (usernameInputfield.text.Length == 0 || codeInputfield.text.Length == 0)
        {
            GiveError("Fill in all fields");
            return;
        }

        if (Client.roomStatus == Client.RoomStatus.OutRoom)
        {
            Client.roomStatus = Client.RoomStatus.JoiningRoom;

            //Join room
            PhotonNetwork.JoinRoom(codeInputfield.text);
            PhotonNetwork.NickName = usernameInputfield.text; //Nickname
        }
        else if (Client.roomStatus == Client.RoomStatus.InRoom) Debug.LogError("Already entered room");
        else if (Client.roomStatus == Client.RoomStatus.JoiningRoom) Debug.LogError("Already joining room");
        else Debug.LogError("Still leaving room");
    }

    public void LeaveRoom()
    {
        Client.roomStatus = Client.RoomStatus.LeavingRoom;
        PhotonNetwork.LeaveRoom();
        //OnLeftRoom();
    }

    /*########################################################*/
    /*## PHOTON CALLBACKS ####################################*/
    /*########################################################*/

    public override void OnJoinedRoom()
    {
        joinButton.interactable = false;
        joinButtonText.text = "Joined";

        Client.hostJoin = Client.HostJoin.Join;
        Client.roomStatus = Client.RoomStatus.InRoom;

        ClearError();
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

    public override void OnLeftRoom()
    {
        joinButton.interactable = true;
        joinButtonText.text = "Join";

        Client.roomStatus = Client.RoomStatus.OutRoom;
        Client.hostJoin = Client.HostJoin.Undefined;
    }
}