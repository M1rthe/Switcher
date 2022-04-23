using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Level2MissionCheck : MissionCheck
{
    [SerializeField] Animation anim;
    [SerializeField] Renderer eyes;
    [SerializeField] Transform catStareArea;
    [SerializeField] Gradient colorGradient;

    Camera cam = null;
    float timeStaredAtCat = 0;

    float noEmmision = 0.95f;
    float fullEmmision = 0f;

    public override void Start()
    {
        base.Start();

        eyes.material.SetFloat("_EmissiveExposureWeight", noEmmision);
        //eyes.material.SetColor("_EmissiveColor", colorGradient.Evaluate(0));
    }

    void Update()
    {
        if (timeStaredAtCat < 0) return;

        if (cam == null)
        {
            if (GameManager.player != null)
            {
                cam = GameManager.player.GetComponentInChildren<Camera>();
            }

            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 2f, ~LayerMask.GetMask("IgnorePlayer"));
        bool hitsCat = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == catStareArea)
            {
                hitsCat = true;

                if (timeStaredAtCat == 0) anim.Play();

                timeStaredAtCat += Time.deltaTime;
                if (timeStaredAtCat < 5f)
                {
                    float timeStaredFactor = (timeStaredAtCat / 5);
                    float intensity = (1 - timeStaredFactor) * (noEmmision - fullEmmision) + fullEmmision;
                    eyes.material.SetFloat("_EmissiveExposureWeight", intensity);
                    //eyes.material.SetColor("_EmissiveColor", colorGradient.Evaluate(timeStaredFactor));
                }
                else
                {
                    base.photonView.RPC("Succes", RpcTarget.All, Time.timeSinceLevelLoad, timesSwitched);
                    timeStaredAtCat = -1;
                }
            }
        }

        if (!hitsCat)
        {
            timeStaredAtCat = 0;
            eyes.material.SetFloat("_EmissiveExposureWeight", noEmmision);
            //eyes.material.SetColor("_EmissiveColor", colorGradient.Evaluate(0));
        }
    }

    [PunRPC]
    public override void Switched()
    {
        base.Switched();
    }
}
