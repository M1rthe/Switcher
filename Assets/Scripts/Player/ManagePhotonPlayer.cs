using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ManagePhotonPlayer : MonoBehaviour
{
    PhotonView photonView;
    [SerializeField] List<Behaviour> components = new List<Behaviour>();
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();

    MeshRenderer body;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        { 
            Client.photonPlayer = photonView.Owner;
            //Debug.LogError("PhotonPlayer: " + Client.photonPlayer.NickName);
        }
        else
        {
            //Debug.LogError("Other PhotonPlayer: " + photonView.Owner.NickName);
        }

        body = transform.Find("PlayerModel/Body").GetComponent<MeshRenderer>();
        foreach (Behaviour component in components) component.enabled = photonView.IsMine;
        foreach (GameObject gameObject in gameObjects) gameObject.SetActive(photonView.IsMine);

        /*foreach (Behaviour component in components) if (!photonView.IsMine) Destroy(component);
        foreach (GameObject gameObject in gameObjects) if (!photonView.IsMine) Destroy(gameObject);*/
    }

    [PunRPC]
    public void ChangeColor()
    {
        body.material.color = Color.red;
    }
}
