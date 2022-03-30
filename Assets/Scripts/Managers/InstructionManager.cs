using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour
{
    Text msg;
    Animation anim;

    void Start()
    {
        msg = GetComponentInChildren<Text>();
        anim = GetComponent<Animation>();
    }

    public void GiveInstruction(string msg, float duration = -1f)
    {
        this.msg.text = msg; //Set text

        anim.Play("FlyIn"); //Play Animation rewinded

        if (duration > 0) StartCoroutine(Wait(duration, delegate { PullInstructionAway(); })); //Wait  
    }

    public void PullInstructionAway()
    {
        anim.Play("FlyOut");
        StartCoroutine(OnAnimationFinish(delegate { msg.text = ""; })); //Reset text
    }

    IEnumerator Wait(float duration, System.Action action)
    {
        yield return new WaitForSeconds(duration);

        action.Invoke();
    }

    IEnumerator OnAnimationFinish(System.Action action)
    {
        while (anim.IsPlaying("FlyIn") || anim.IsPlaying("FlyOut")) 
        { 
            yield return null; 
        }

        action.Invoke();
    }
}
