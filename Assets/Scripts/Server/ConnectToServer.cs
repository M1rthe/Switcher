using Photon.Pun;
using UnityEngine;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        GameManager.Instance.hostJoin = GameManager.HostJoin.Undefined;
    }

    //Lobby
    public override void OnJoinedLobby() { GameManager.Instance.lobbyStatus = GameManager.LobbyStatus.InLobby; }
    public override void OnLeftLobby() { GameManager.Instance.lobbyStatus = GameManager.LobbyStatus.OutLobby; }    
}
