using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class ManagePhotonPlayer : MonoBehaviour
{
    public PhotonView photonView { get; private set; }
    Camera cam;

    [SerializeField] List<Behaviour> components = new List<Behaviour>();
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();

    LayerMask playerLayer;
    LayerMask otherPlayerLayer;
    LayerMask ghostPlayerLayer;
    LayerMask otherGhostPlayerLayer;

    MeshRenderer body;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        cam = GetComponentInChildren<Camera>();
        body = transform.Find("PlayerModel/Body").GetComponent<MeshRenderer>();

        playerLayer = LayerMask.NameToLayer("Player");
        otherPlayerLayer = LayerMask.NameToLayer("OtherPlayer");
        ghostPlayerLayer = LayerMask.NameToLayer("GhostPlayer");
        otherGhostPlayerLayer = LayerMask.NameToLayer("OtherGhostPlayer");

        ConvertToOtherPlayer(!photonView.IsMine);
    }

    void ConvertToOtherPlayer(bool isOtherPlayer)
    {

        //Turn off the extra components
        foreach (Behaviour component in components) component.enabled = !isOtherPlayer;
        //Turn off the extra gameobjects
        foreach (GameObject gameObject in gameObjects) gameObject.SetActive(!isOtherPlayer);

        if (isOtherPlayer)
        {
            //Other player
            gameObject.name = "OtherPlayer"; //GameObject name
            SetLayerRecursively(gameObject, otherPlayerLayer); //Layer
        }
        else
        {
            //Current player
            GameManager.photonPlayer = photonView.Owner;
            GameManager.player = gameObject;

            gameObject.name = "Player"; //GameObject name
            SetLayerRecursively(gameObject, playerLayer); //Layer
            GameManager.hud = transform.GetComponentInChildren<HUD>(); //HUD

            //Invoke OnPlayerSpawned
            StartCoroutine(PlayersReceivedOthersCurrentTimeline(delegate {
                var onPlayersSpawnedInterfaces = FindObjectsOfType<MonoBehaviour>().OfType<IOnPlayersSpawned>();
                foreach (IOnPlayersSpawned onPlayersSpawned in onPlayersSpawnedInterfaces)
                {
                    onPlayersSpawned.OnPlayersSpawned();
                }

                photonView.RPC("SetColorTransparent", RpcTarget.All, TimelineManager.CurrentTimeline != TimelineManager.CurrentTimelineOtherPlayer);
            }));
        }
    }

    [PunRPC]
    public void ChangeColor()
    {
        body.material.SetColor("_BaseColor", Color.red);
    }

    [PunRPC]
    public void SetColorTransparent(bool enable)
    {
        Color c = body.material.GetColor("_BaseColor");
        if (enable)
        {
            c.a = 0.5f;
            if (gameObject.layer == playerLayer) SetLayerRecursively(gameObject, ghostPlayerLayer);
            if (gameObject.layer == otherPlayerLayer) SetLayerRecursively(gameObject, otherGhostPlayerLayer);
        }
        else
        {
            c.a = 1f;
            if (gameObject.layer == ghostPlayerLayer) SetLayerRecursively(gameObject, playerLayer);
            if (gameObject.layer == otherGhostPlayerLayer) SetLayerRecursively(gameObject, otherPlayerLayer);
        }

        body.material.SetColor("_BaseColor", c);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if ((LayerMask.GetMask("ItemPlayer", "ItemOtherPlayer", "ItemBothPlayers", "ItemNeitherPlayers") & (1 << obj.layer)) != 0) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }

    IEnumerator PlayersReceivedOthersCurrentTimeline(UnityEngine.Events.UnityAction action)
    {
        while (TimelineManager.CurrentTimeline == -1 || TimelineManager.CurrentTimelineOtherPlayer == -1)
        {
            yield return null;
        }

        action.Invoke();
    }
}
