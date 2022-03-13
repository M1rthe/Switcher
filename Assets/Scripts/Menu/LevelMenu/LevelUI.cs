using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LevelUI : MonoBehaviour
{
    Image background;
    Image preview;
    Text levelNumber;
    Image[] stars = new Image[3];
    [HideInInspector] public Button button;

    PhotonView photonView;

    public void GetThings(PhotonView photonView)
    {
        background = GetComponent<Image>();
        preview = transform.Find("Preview").GetComponent<Image>();
        levelNumber = GetComponentInChildren<Text>();
        button = GetComponent<Button>();

        this.photonView = photonView;
    }

    public void Select(bool select)
    {
        if (select) background.color = Color.grey;
        else background.color = Color.white;

        if (Client.hostJoin == Client.HostJoin.Host)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("Select", RpcTarget.OthersBuffered, select, transform.GetSiblingIndex());
            }
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
        Transform starsTransform = transform.Find("Stars");
        for (int i = 0; i < starsTransform.childCount; i++)
        {
            stars[i] = starsTransform.GetChild(i).GetChild(0).GetComponent<Image>();
        }

        stars[0].color = progress.completedMission ? Color.yellow : Color.white;
        stars[1].color = progress.belowCertainTime ? Color.yellow : Color.white;
        stars[2].color = progress.underCertainSwitches ? Color.yellow : Color.white;
    }
}
