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

        GameManager.hostJoin = GameManager.HostJoin.Undefined;
    }

    //Lobby
    public override void OnJoinedLobby() { GameManager.lobbyStatus = GameManager.LobbyStatus.InLobby; }
    public override void OnLeftLobby() { GameManager.lobbyStatus = GameManager.LobbyStatus.OutLobby; }    
}
