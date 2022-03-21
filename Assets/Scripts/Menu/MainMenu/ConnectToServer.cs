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

        Client.hostJoin = Client.HostJoin.Undefined;
    }

    //Lobby
    public override void OnJoinedLobby() { Client.lobbyStatus = Client.LobbyStatus.InLobby; }
    public override void OnLeftLobby() { Client.lobbyStatus = Client.LobbyStatus.OutLobby; }    
}
