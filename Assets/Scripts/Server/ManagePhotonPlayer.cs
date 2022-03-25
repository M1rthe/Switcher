using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ManagePhotonPlayer : MonoBehaviour
{
    public PhotonView PhotonView { get; set; }
    [SerializeField] List<Behaviour> components = new List<Behaviour>();
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();

    MeshRenderer body;

    void Awake()
    {
        PhotonView = GetComponent<PhotonView>();

        Camera cam = GetComponentInChildren<Camera>();

        if (PhotonView.IsMine)
        { 
            Client.photonPlayer = PhotonView.Owner;
            gameObject.layer = LayerMask.NameToLayer("Player");
            cam.cullingMask |= (1 << LayerMask.NameToLayer("ItemInvisibleToOther"));
            //Debug.LogError("PhotonPlayer: " + Client.photonPlayer.NickName);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("OtherPlayer");
            cam.cullingMask |= (1 << LayerMask.NameToLayer("ItemInvisibleToPlayer"));
            //Debug.LogError("Other PhotonPlayer: " + photonView.Owner.NickName);
        }

        body = transform.Find("PlayerModel/Body").GetComponent<MeshRenderer>();
        foreach (Behaviour component in components) component.enabled = PhotonView.IsMine;
        foreach (GameObject gameObject in gameObjects) gameObject.SetActive(PhotonView.IsMine);

        /*foreach (Behaviour component in components) if (!photonView.IsMine) Destroy(component);
        foreach (GameObject gameObject in gameObjects) if (!photonView.IsMine) Destroy(gameObject);*/
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
        if (enable) c.a = 0.5f;
        else c.a = 1f;
        body.material.SetColor("_BaseColor", c);
    }
}
