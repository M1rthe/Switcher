using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] Object levelSelectionBoxPrefab;

    Transform levelParent;
    List<LevelSelectionBox> levelSelectionBoxes = new List<LevelSelectionBox>();

    string levelProgressJsonPath;

    PhotonView photonView;

    void Awake()
    {
        //Get things
        levelParent = transform.Find("LevelsBackground/Levels");
        levelProgressJsonPath = Application.streamingAssetsPath + "/LevelProgress.json";
        photonView = GetComponent<PhotonView>();
    }

    public void AddLevelUI()
    {
        //RESET
        while (levelParent.childCount > 0) DestroyImmediate(levelParent.GetChild(0).gameObject);
        foreach (LevelSelectionBox selectionBox in levelSelectionBoxes) selectionBox.button.onClick.RemoveAllListeners();
        levelSelectionBoxes.Clear();

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
            if (Client.hostJoin == Client.HostJoin.Join) Destroy(level.transform.GetComponent<UnityEngine.EventSystems.EventTrigger>());

            //Set LevelSelectionBox callback
            int dereferencedI = level.transform.GetSiblingIndex() + 1;
            level.button.onClick.AddListener(
                delegate
                {
                    if (Client.hostJoin == Client.HostJoin.Host)
                    {
                        photonView.RPC("GotoLevel", RpcTarget.All, dereferencedI);
                    }
                }
            );

            //Add to list
            levelSelectionBoxes.Add(level);
        }
    }

    [PunRPC]
    public void GotoLevel(int sceneIndex)
    {
        Client.location = Client.Location.Game;

        //Cursor
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;        
        
        //Load scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    [PunRPC]
    public void Select(bool select, int index)
    {
        levelSelectionBoxes[index].Select(select);
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