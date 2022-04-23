using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shovel : Item
{
    TreeManager holdingTree;

    public override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0))
        {
            if (inHand)
            {
                if (transform.GetChild(0).childCount == 0)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 2f, ~LayerMask.GetMask("IgnorePlayer")))
                    {
                        Tree tree = hit.transform.GetComponent<Tree>();

                        if (tree != null)
                        {
                            TreeManager treeManager = tree.treeManager;

                            if (treeManager != null)
                            {
                                if (tree.stage == Tree.Stage.S)
                                {
                                    treeManager.transform.SetParent(transform.GetChild(0));
                                    treeManager.transform.localPosition = new Vector3(0, 0, 0);
                                    treeManager.transform.localEulerAngles = new Vector3(0, 0, 0);

                                    holdingTree = treeManager;
                                }

                            }
                        }
                    }
                }
                else
                {
                    if (holdingTree != null)
                    {
                        RaycastHit hit;
                        Vector3 pos = holdingTree.transform.position;
                        pos.y = 100;
                        if (Physics.Raycast(pos, Vector3.down, out hit, 200f, LayerMask.GetMask("Terrain")))
                        {
                            holdingTree.transform.SetParent(holdingTree.startParent);
                            holdingTree.view.RPC("PutInGround", RpcTarget.All, hit.point);
                            holdingTree = null;
                        }
                    }
                }
            }
        }
    }
}
