using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;

public class MissionCheck : MonoBehaviour, IOnPlayersSpawned
{
    [SerializeField] protected float timeLimit = 300;
    [SerializeField] protected int maxSwitches = 8;

    float timePassed = 0f;
    private static int timesSwithed = 0;

    WinScreen winScreen;

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
        LevelData.SaveLevelProgress(GameManager.GetCurrentScene() - 1, levelProgress);

        winScreen.gameObject.SetActive(true);
        winScreen.SetText("Time: "+ timePassed.ToString("F2") + "s / "+timeLimit+"s", "Times Switched: "+timesSwithed +"x / "+maxSwitches+"x");
        winScreen.SetLevelProgress(levelProgress);

        StartCoroutine(DisplayWinMessage(delegate{
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }));
    }

    IEnumerator DisplayWinMessage(UnityAction unityAction)
    {
        yield return new WaitForSeconds(5);

        winScreen.gameObject.SetActive(false);
        unityAction.Invoke();
    }

    public void OnPlayersSpawned()
    {
        winScreen = GameManager.FindObjectOfType<WinScreen>();
        winScreen.gameObject.SetActive(false);
    }
}
