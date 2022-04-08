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

    protected float timePassed;
    protected static int timesSwitched;

    WinScreen winScreen;

    protected PhotonView photonView;

    void Start()
    {
        timePassed = 0;
        timesSwitched = 0;
        photonView = GetComponent<PhotonView>();   
    }

    void Update()
    {
        timePassed += Time.deltaTime;
    }

    public virtual void Switched()
    {
        timesSwitched++;   
    }

    [PunRPC]
    protected void Succes(float timePassed, int timesSwitched)
    {
        LevelProgress levelProgress = new LevelProgress 
        {
            completedMission = true,
            belowCertainTime = (timePassed < timeLimit),
            underCertainSwitches = (timesSwitched <= maxSwitches)
        }; 
        LevelData.SaveLevelProgress(GameManager.GetCurrentScene() - 1, levelProgress);

        winScreen.gameObject.SetActive(true);
        winScreen.SetText("Time: "+ timePassed.ToString("F2") + "s / "+timeLimit+"s", "Times Switched: "+ timesSwitched + "x / "+maxSwitches+"x");
        winScreen.SetLevelProgress(levelProgress);

        StartCoroutine(DisplayWinMessage(delegate{
            GameManager.Instance.GoTo(GameManager.Location.LevelMenu, 0);
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
