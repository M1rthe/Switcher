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

        if (GameManager.roomStatus == GameManager.RoomStatus.OutRoom)
        {
            GameManager.roomStatus = GameManager.RoomStatus.JoiningRoom;

            //Join room
            menuManager.loadWheel.Load = true;
            PhotonNetwork.JoinRoom(codeInputfield.text);
            PhotonNetwork.NickName = usernameInputfield.text; //Nickname
        }
        else if (GameManager.roomStatus == GameManager.RoomStatus.InRoom) Debug.LogError("Already entered room");
        else if (GameManager.roomStatus == GameManager.RoomStatus.JoiningRoom) Debug.LogError("Already joining room");
        else Debug.LogError("Still leaving room");
    }

    public void LeaveRoom()
    {
        GameManager.roomStatus = GameManager.RoomStatus.LeavingRoom;
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

        GameManager.hostJoin = GameManager.HostJoin.Join;
        GameManager.roomStatus = GameManager.RoomStatus.InRoom;

        menuManager.loadWheel.Load = false;

        ClearError();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GameManager.roomStatus = GameManager.RoomStatus.OutRoom;
        menuManager.loadWheel.Load = false;

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

        GameManager.roomStatus = GameManager.RoomStatus.OutRoom;
        GameManager.hostJoin = GameManager.HostJoin.Undefined;
    }
}