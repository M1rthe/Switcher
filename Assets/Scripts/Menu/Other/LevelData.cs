using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    static string GetPath() { return Application.streamingAssetsPath + "/LevelData.json"; }
    static string ReadPath() { return System.IO.File.ReadAllText(GetPath()); }

    public static LevelDataLists ReadLevelData() { return JsonUtility.FromJson<LevelDataLists>(ReadPath()); }
    public static List<LevelProgress> ReadLevelProgresses() { return ReadLevelData().levelProgressesList; }
    public static List<LevelObjective> ReadLevelObjectives() { return ReadLevelData().levelObjectivesList; }
    public static List<StartTimeline> ReadStartTimeline() { return ReadLevelData().startTimelinesList; }

    public static void SaveLevelData(LevelDataLists levelData)
    {
        string data = JsonUtility.ToJson(levelData);
        System.IO.File.WriteAllText(GetPath(), data);
    }
    public static void SaveLevelProgress(List<LevelProgress> levelProgress)
    {
        LevelDataLists levelData = ReadLevelData(); //Get data
        levelData.levelProgressesList = levelProgress; //Change only level progress
        SaveLevelData(levelData); //Save level data
    }
    public static void SaveLevelProgress(int index, LevelProgress levelProgress)
    {
        List<LevelProgress> levelProgresses = ReadLevelProgresses();
        levelProgresses[index] = levelProgress;
        SaveLevelProgress(levelProgresses);
    }
    public static void SaveLevelObjectives(List<LevelObjective> levelObjectives)
    {
        LevelDataLists levelData = ReadLevelData(); //Get data
        levelData.levelObjectivesList = levelObjectives; //Change only level progress
        SaveLevelData(levelData); //Save level data
    }
    public static void SaveStartTimeline(List<StartTimeline> startTimelines)
    {
        LevelDataLists levelData = ReadLevelData(); //Get data
        levelData.startTimelinesList = startTimelines; //Change only level progress
        SaveLevelData(levelData); //Save level data
    }
}

[System.Serializable]
public class LevelDataLists
{
    public List<LevelProgress> levelProgressesList = new List<LevelProgress>();
    public List<LevelObjective> levelObjectivesList = new List<LevelObjective>();
    public List<StartTimeline> startTimelinesList = new List<StartTimeline>();
}

[System.Serializable]
public class LevelProgress
{
    public bool completedMission = false;
    public bool belowCertainTime = false;
    public bool underCertainSwitches = false;
}

[System.Serializable]
public class LevelObjective
{
    public string story = "";
    public string mission = "";
}

[System.Serializable]
public class StartTimeline
{
    public int p0 = 1;
    public int p1 = 1;
}
