﻿using Photon.Pun;

public class Client : MonoBehaviourPunCallbacks
{
    public static Photon.Realtime.Player photonPlayer;

    public enum HostJoin { Host, Join, Undefined }
    public static HostJoin hostJoin = HostJoin.Undefined;

    public enum RoomStatus { JoiningRoom, LeavingRoom, InRoom, OutRoom }
    public static RoomStatus roomStatus = RoomStatus.OutRoom;

    public enum LobbyStatus { JoiningLobby, LeavingLobby, InLobby, OutLobby }
    public static LobbyStatus lobbyStatus = LobbyStatus.OutLobby;

    public enum Location { Menu, Game }
    public static Location location = Location.Menu;
}
