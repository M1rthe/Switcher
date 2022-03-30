using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MissionCheck : MonoBehaviour
{
    [SerializeField] protected float timeLimit = 300;
    [SerializeField] protected int maxSwitches = 8;

    float timePassed = 0f;
    private static int timesSwithed = 0;

    protected PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();   
    }

    void Update()
    {
        timePassed += Time.deltaTime;
    }

    public virtual void Switched()
    {
        timesSwithed++;   
    }

    protected LevelProgress GetProgress()
    {
        LevelProgress levelProgress = new LevelProgress();
        levelProgress.completedMission = true;
        levelProgress.belowCertainTime = (timePassed < timeLimit);
        levelProgress.underCertainSwitches = (timesSwithed <= maxSwitches);
        Debug.LogError($"completedMission: {levelProgress.completedMission}, below {timeLimit} seconds: {levelProgress.belowCertainTime} ({timePassed}), under {maxSwitches} switches: {levelProgress.underCertainSwitches} ({timesSwithed})");
        return levelProgress;
    }

    [PunRPC]
    protected void Succes()
    {
        LevelProgress levelProgress = GetProgress();
        LevelData.SaveLevelProgress(Client.GetCurrentScene() - 1, levelProgress);
        Client.location = Client.Location.Menu;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
