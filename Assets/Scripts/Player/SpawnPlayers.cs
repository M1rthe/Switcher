using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] Object playerPrefab;
    Transform spawnPositions;

    void Awake()
    {
        StartCoroutine("Wait");
    }

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();

        spawnPositions = transform.Find("SpawnPositions").transform;
        int index = System.Convert.ToInt32(Client.hostJoin == Client.HostJoin.Host);
        Vector3 spawnPosition = spawnPositions.GetChild(index).position;
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        if (Client.hostJoin == Client.HostJoin.Host) player.GetPhotonView().RPC("ChangeColor", RpcTarget.AllBuffered);
    }
}
