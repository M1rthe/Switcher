using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] Image[] stars;
    [SerializeField] Text timeText;
    [SerializeField] Text switchedText;

    public void SetText(string time, string switched)
    {
        timeText.text = time;
        switchedText.text = switched;
    }

    public void SetLevelProgress(LevelProgress progress)
    {
        stars[0].color = progress.completedMission ? Color.yellow : Color.white;
        stars[1].color = progress.belowCertainTime ? Color.yellow : Color.white;
        stars[2].color = progress.underCertainSwitches ? Color.yellow : Color.white;
    }
}
