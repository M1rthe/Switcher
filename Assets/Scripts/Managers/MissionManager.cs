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
        //Get text components
        storyText = missionPanel.transform.Find("Story").GetChild(1).GetComponent<Text>();
        missionText = missionPanel.transform.Find("Mission").GetChild(1).GetComponent<Text>();

        //Cast to list
        List<LevelObjective> levelInfos = LevelData.ReadLevelObjectives();

        //Get level info of current level, set text 
        LevelObjective currentLevel = levelInfos[UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1];
        storyText.text = currentLevel.story;
        missionText.text = currentLevel.mission;
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
            //Click away with any key
            if (Input.anyKeyDown)
            {
                missionPanel.SetActive(false);
                firstTimeShown = false;
            }
        } 
        else if (Input.GetKeyDown(KeyCode.M))
        {
            //Toggle with 'M'
            missionPanel.SetActive(!missionPanel.activeSelf);
        }
    }
}