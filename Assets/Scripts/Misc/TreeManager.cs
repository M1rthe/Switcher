using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TreeManager : MonoBehaviour, IOnPlayersSpawned
{
    [SerializeField] bool isDead;
    public bool IsDead {
        get { return isDead; }
        set
        {
            view.RPC("SetIsDead", RpcTarget.All, value);
        }
    }

    [SerializeField] List<TreeManager> futureTrees = new List<TreeManager>();

    [PunRPC]
    void SetIsDead(bool value) 
    {
        isDead = value;
        ShowTree(System.Convert.ToInt32(isDead));
        foreach (TreeManager futureTree in futureTrees) { futureTree.IsDead = value; }
    }

    [HideInInspector] public int absorbedChops = 0;
    bool cutDown = false;
    public bool CutDown 
    {
        get { return cutDown; }
        set 
        {
            cutDown = value;
            IsCutDown(value);
        }
    }

    [HideInInspector] public Transform startParent;
    [HideInInspector] public Vector3 startRotation;

    AudioSource cutDownAudio;

    [HideInInspector] public PhotonView view;

    void Start()
    {
        view = gameObject.GetComponent<PhotonView>();

        startParent = transform.parent;
        startRotation = transform.eulerAngles;

        cutDownAudio = GetComponent<AudioSource>();
    }

    public void OnPlayersSpawned()
    {
        IsDead = IsDead;
    }

    [PunRPC]
    public void Chop()
    {
        absorbedChops++;
    }

    [PunRPC]
    public void IsCutDown(bool value)
    {
        if (value) cutDownAudio.Play();
    }

    [PunRPC]
    public void PutInGround(Vector3 pos)
    {
        foreach (TreeManager futureTree in futureTrees)
        {
            futureTree.PutInGround(pos);
        }

        transform.position = pos;
        transform.eulerAngles = startRotation;
    }

    void ShowTree(int index)
    {
        foreach (Transform tree in transform)
        {
            tree.gameObject.SetActive(false);
        }

        transform.GetChild(index).gameObject.SetActive(true);
    }
}
