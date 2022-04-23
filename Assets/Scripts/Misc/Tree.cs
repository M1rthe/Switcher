using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tree : MonoBehaviour
{
    public enum Stage { S, M, L }
    public Stage stage;

    [HideInInspector] public TreeManager treeManager;

    private void Awake()
    {
        treeManager = GetComponentInParent<TreeManager>();
    }
}
