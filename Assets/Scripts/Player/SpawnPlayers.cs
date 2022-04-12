using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] Object playerPrefab;

    void Awake()
    {
        StartCoroutine("Wait");
    }

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();

        //Set spawnpoint
        int index = System.Convert.ToInt32(GameManager.hostJoin == GameManager.HostJoin.Host);
        GameManager.spawnPoint = transform.Find("SpawnPositions").transform.GetChild(index);

        // (past) | present | (future)
        GameManager.playerType = (GameManager.PlayerType)index;

        //Instantiate
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, GameManager.spawnPoint.position, GameManager.spawnPoint.rotation);

        //Color
        if (GameManager.hostJoin == GameManager.HostJoin.Host)
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
