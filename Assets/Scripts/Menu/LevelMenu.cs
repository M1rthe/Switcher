using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] Object levelSelectionBoxPrefab;
    List<LevelSelectionBox> levelSelectionBoxes = new List<LevelSelectionBox>();

    PhotonView photonView;

    void OnEnable()
    {
        AddLevelUI();
    }

    void AddLevelUI()
    {
        //RESET
        Transform levelParent = transform.Find("LevelsBackground/Levels");
        photonView = GetComponent<PhotonView>();
        while (levelParent.childCount > 0) DestroyImmediate(levelParent.GetChild(0).gameObject);
        foreach (LevelSelectionBox selectionBox in levelSelectionBoxes) selectionBox.button.onClick.RemoveAllListeners();
        levelSelectionBoxes.Clear();

        //Load level progress
        List<LevelProgress> levelProgress = LevelData.ReadLevelProgresses();

        //Loop through levels
        int sceneCount = GameManager.GetSceneCount();
        for (int i = 1; i < sceneCount; i++)
        {
            //Instantiate LevelSelectionBox
            GameObject levelGameObject = Instantiate(levelSelectionBoxPrefab, levelParent) as GameObject;
            LevelSelectionBox level = levelGameObject.GetComponent<LevelSelectionBox>();
            level.GetThings(photonView);

            //Set LevelSelectionBox number
            level.SetLevelNumber(i);

            //Set LevelSelectionBox preview
            Sprite preview = Resources.Load<Sprite>("LevelPreview/" + i);
            if (preview != null) level.SetPreviewImage(preview);

            //Set LevelSelectionBox stars
            level.SetLevelProgress(levelProgress[i - 1]);

            //Destroy ability to show level selection, when joiner
            if (GameManager.Instance.hostJoin == GameManager.HostJoin.Join) Destroy(level.transform.GetComponent<UnityEngine.EventSystems.EventTrigger>());

            //Set LevelSelectionBox callback
            int dereferencedI = level.transform.GetSiblingIndex() + 1;
            level.button.onClick.AddListener(
                delegate
                {
                    if (GameManager.Instance.hostJoin == GameManager.HostJoin.Host)
                    {
                        GameManager.Instance.photonView.RPC("GoTo", RpcTarget.All, GameManager.Location.Level, dereferencedI);
                    }
                }
            );

            //Add to list
            levelSelectionBoxes.Add(level);
        }
    }

    [PunRPC]
    public void Select(bool select, int index)
    {
        levelSelectionBoxes[index].Select(select);
    }
}