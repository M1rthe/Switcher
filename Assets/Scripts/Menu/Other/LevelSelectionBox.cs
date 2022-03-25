using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LevelSelectionBox : MonoBehaviour
{
    Text levelNumber;
    Image preview;
    Image outline;
    Image[] stars = new Image[3];
    [HideInInspector] public Button button;

    PhotonView photonView;

    public void GetThings(PhotonView photonView)
    {
        levelNumber = GetComponentInChildren<Text>();
        preview = transform.Find("Preview").GetComponent<Image>();
        outline = GetComponent<Image>();
        button = GetComponent<Button>();
        Transform starsTransform = transform.Find("Stars");
        for (int i = 0; i < starsTransform.childCount; i++) stars[i] = starsTransform.GetChild(i).GetChild(0).GetComponent<Image>();

        this.photonView = photonView;
    }

    public void Select(bool select)
    {
        //Change color outline on selection
        if (select) outline.color = Color.grey;
        else outline.color = Color.white;

        if (Client.hostJoin == Client.HostJoin.Host)
        {
            //If host, also select the boxes for the other players
            photonView.RPC("Select", RpcTarget.Others, select, transform.GetSiblingIndex());
        }
    }

    public void SetPreviewImage(Sprite s)
    {
        preview.sprite = s;
    }

    public void SetLevelNumber(int num)
    {
        levelNumber.text = num.ToString();
    }

    public void SetLevelProgress(LevelProgress progress)
    {
        stars[0].color = progress.completedMission ? Color.yellow : Color.white;
        stars[1].color = progress.belowCertainTime ? Color.yellow : Color.white;
        stars[2].color = progress.underCertainSwitches ? Color.yellow : Color.white;
    }
}
