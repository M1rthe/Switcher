using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] Object LevelUIPrefab;

    Transform levels;
    List<LevelUI> levelUIs = new List<LevelUI>();

    PhotonView photonView;

    string levelProgressJsonPath;

    void Awake()
    {
        levels = transform.Find("LevelsBackground/Levels");
        levelProgressJsonPath = Application.streamingAssetsPath + "/LevelProgress.json";
        photonView = GetComponent<PhotonView>();
    }

    public void AddLevelUI()
    {
        //RESET
        while (levels.childCount > 0) DestroyImmediate(levels.GetChild(0).gameObject);
        foreach (LevelUI levelUI in levelUIs) levelUI.button.onClick.RemoveAllListeners();
        levelUIs.Clear();

        //Save level progress
        AllLevelProgress allLevelProgress = new AllLevelProgress(
            new List<LevelProgress>() {
                new LevelProgress(true, true, true),
                new LevelProgress(true, false, false),
                new LevelProgress(false, false, false)
            }
        );

        SaveLevelProgress(allLevelProgress);

        //Load level progress
        List<LevelProgress> levelProgress = ReadLevelProgress();

        //Loop through levels
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        for (int i = 1; i < sceneCount; i++)
        {
            //Instantiate LevelUI
            GameObject levelGameObject = Instantiate(LevelUIPrefab, levels) as GameObject;
            LevelUI level = levelGameObject.GetComponent<LevelUI>();
            level.GetThings(photonView);

            //Set LevelUI number
            level.SetLevelNumber(i);

            //Set LevelUI preview
            Sprite preview = Resources.Load<Sprite>("LevelPreview/" + i);
            if (preview != null) level.SetPreviewImage(preview);

            //Set LevelUI stars
            level.SetLevelProgress(levelProgress[i - 1]);

            if (Client.hostJoin == Client.HostJoin.Join)
            {
                Destroy(level.transform.GetComponent<UnityEngine.EventSystems.EventTrigger>());
            }

            //Set LevelUI callback
            int dereferencedI = level.transform.GetSiblingIndex() + 1;
            level.button.onClick.AddListener(
                delegate
                {
                    if (Client.hostJoin == Client.HostJoin.Host)
                    {
                        photonView.RPC("GotoLevel", RpcTarget.AllBuffered, dereferencedI);
                    }
                }
            );

            levelUIs.Add(level);
        }
    }

    [PunRPC]
    public void GotoLevel(int sceneIndex)
    {
        Client.location = Client.Location.Game;

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);

        Cursor.lockState = CursorLockMode.Locked; //Cursor
        Cursor.visible = false;
    }

    [PunRPC]
    public void Select(bool select, int index)
    {
        levelUIs[index].Select(select);
    }

    void SaveLevelProgress(AllLevelProgress levelProgress)
    {
        string data = JsonUtility.ToJson(levelProgress);
        System.IO.File.WriteAllText(levelProgressJsonPath, data);
    }

    List<LevelProgress> ReadLevelProgress()
    {
        string data = System.IO.File.ReadAllText(levelProgressJsonPath);
        return JsonUtility.FromJson<AllLevelProgress>(data).levelProgressesList;
    }
}

public class AllLevelProgress
{
    public List<LevelProgress> levelProgressesList = new List<LevelProgress>();

    public AllLevelProgress(List<LevelProgress> levelProgressesList)
    {
        this.levelProgressesList = levelProgressesList;
    }
}

[System.Serializable]
public class LevelProgress
{
    public bool completedMission = false;
    public bool belowCertainTime = false;
    public bool underCertainSwitches = false;

    public LevelProgress(bool completedMission = false, bool belowCertainTime = false, bool underCertainSwitches = false)
    {
        this.completedMission = completedMission;
        this.belowCertainTime = belowCertainTime;
        this.underCertainSwitches = underCertainSwitches;
    }
}