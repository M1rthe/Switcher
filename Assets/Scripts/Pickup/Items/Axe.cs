using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Item
{
    AudioSource chop;

    public override void Start()
    {
        base.Start();

        chop = GetComponent<AudioSource>();
    }

    public override void Use(GameObject go)
    {
        Tree tree = go.GetComponent<Tree>();
        if (tree != null)
        {
            if (tree.treeManager.CutDown == false)
            {
                chop.Play();
                tree.treeManager.view.RPC("Chop", Photon.Pun.RpcTarget.All);

                if (tree.treeManager.absorbedChops >= 5)
                {
                    tree.treeManager.CutDown = true;

                    Animation anim = tree.GetComponentInChildren<Animation>();
                    if (anim != null)
                    {
                        anim.Play();
                    }
                }
            }
        }
    }
}
