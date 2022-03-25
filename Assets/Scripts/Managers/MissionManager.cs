using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    [SerializeField] GameObject missionPanel;
    Text storyText;
    Text missionText;
    bool firstTimeShown;

    void Start()
    {
        storyText = missionPanel.transform.Find("Story").GetChild(1).GetComponent<Text>();
        missionText = missionPanel.transform.Find("Mission").GetChild(1).GetComponent<Text>();

        string data = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/LevelMissions.json");
        List<LevelInfo> levelInfos = JsonUtility.FromJson<AllLevelInfo>(data).levelMissions;
        int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;
        storyText.text = levelInfos[currentScene].story;
        missionText.text = levelInfos[currentScene].mission;
    }

    void Awake()
    {
        firstTimeShown = true;
        missionPanel.SetActive(true);
    }

    void Update()
    {
        if (firstTimeShown)
        {
            if (Input.anyKeyDown)
            {
                missionPanel.SetActive(false);
                firstTimeShown = false;
            }
        } else if (Input.GetKeyDown(KeyCode.M))
        {
            missionPanel.SetActive(!missionPanel.activeSelf);
        }
    }
}
public class AllLevelInfo
{
    public List<LevelInfo> levelMissions = new List<LevelInfo>();
}
    
[System.Serializable]
public class LevelInfo
{
    public string story;
    public string mission;
}
