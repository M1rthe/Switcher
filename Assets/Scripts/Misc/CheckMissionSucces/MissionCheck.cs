using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;

public class MissionCheck : MonoBehaviour
{
    [SerializeField] protected float timeLimit = 300;
    [SerializeField] protected int maxSwitches = 8;

    protected static int timesSwitched;

    protected PhotonView photonView;

    public virtual void Start()
    {
        timesSwitched = 0;
        photonView = GetComponent<PhotonView>();   
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

        GameManager.hud.winScreen.gameObject.SetActive(true);
        GameManager.hud.winScreen.SetText("Time: "+ timePassed.ToString("F2") + "s / "+timeLimit+"s", "Times Switched: "+ timesSwitched + "x / "+maxSwitches+"x");
        GameManager.hud.winScreen.SetLevelProgress(levelProgress);

        StartCoroutine(DisplayWinMessage(delegate{
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }));
    }

    IEnumerator DisplayWinMessage(UnityAction unityAction)
    {
        yield return new WaitForSeconds(5);

        GameManager.hud.winScreen.gameObject.SetActive(false);
        unityAction.Invoke();
    }
}
