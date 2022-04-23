using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WateringCan : Item
{
    [SerializeField] GameObject water;
    
    [Space]

    [SerializeField] float fillUpTime = 5f;
    float timeInRain = 0f;

    [Space]

    [SerializeField] float emptyY = 0f;
    [SerializeField] float fullY = 1f;

    bool isFull = false;

    bool inRainyTimeline = false;

    public override void Start()
    {
        base.Start();

        Vector3 waterPos = water.transform.localPosition;
        waterPos.y = emptyY;
        water.transform.localPosition = waterPos;
    }

    public override void Update()
    {
        base.Update();

        if (inRainyTimeline)
        {
            if (timeInRain < fillUpTime)
            {
                timeInRain += Time.deltaTime;

                Vector3 waterPos = water.transform.localPosition;
                waterPos.y = timeInRain / fillUpTime * (fullY - emptyY) + emptyY;
                water.transform.localPosition = waterPos;

                isFull = true;
            }
        }
    }

    public override void Use(GameObject go)
    {
        if (isFull)
        {
            Tree tree = go.transform.GetComponent<Tree>();
            if (tree != null)
            {
                TreeManager treeManager = tree.treeManager;

                if (treeManager != null)
                {
                    if (tree.stage == Tree.Stage.S)
                    {
                        PhotonView.RPC("Pour", RpcTarget.All, treeManager.view.ViewID);
                    }
                }
            }
        }
    }

    [PunRPC]
    public override void StartTimelineSwitched()
    {
        inRainyTimeline = TimelineManager.GetTimelineOf(GameObject.FindGameObjectWithTag("Rain")) == StartTimeline;

        PhotonView.RPC("StartTimelineSwitched", RpcTarget.Others);
    }

    [PunRPC]
    void Pour(int treeManagerID)
    {
        StartCoroutine(PourEmpty());
        isFull = false;
        PhotonNetwork.GetPhotonView(treeManagerID).GetComponent<TreeManager>().IsDead = false;
    }

    IEnumerator PourEmpty()
    {
        while (water.transform.localPosition.y > emptyY)
        {
            Vector3 waterPos = water.transform.localPosition;
            waterPos.y -= Time.deltaTime * 0.3f;
            water.transform.localPosition = waterPos;

            yield return null;
        }

        timeInRain = 0;

        yield return null;
    }
}
