﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] Object playerPrefab;
    [SerializeField] Object otherPlayerPrefab;
    Transform spawnPositions;

    void Awake()
    {
        StartCoroutine("Wait");
    }

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();

        //Set spawnpoint
        spawnPositions = transform.Find("SpawnPositions").transform;
        int index = System.Convert.ToInt32(Client.hostJoin == Client.HostJoin.Host);
        Vector3 spawnPosition = spawnPositions.GetChild(index).position;

        // (past) | present | (future)
        Client.playerType = (Client.PlayerType)index;

        //Instantiate
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        //Color
        if (Client.hostJoin == Client.HostJoin.Host)
        {
            player.GetPhotonView().RPC("ChangeColor", RpcTarget.All);
        }

        //Invoke OnPlayerSpawned
        var onPlayersSpawnedInterfaces = FindObjectsOfType<MonoBehaviour>().OfType<IOnPlayersSpawned>();
        foreach (IOnPlayersSpawned pnPlayersSpawned in onPlayersSpawnedInterfaces)
        {
            pnPlayersSpawned.OnPlayersSpawned();
        }
    }
}
