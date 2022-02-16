using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timeline : MonoBehaviour
{
    MeshRenderer[] meshRenderers;
    LayerMask ignorePlayerMask;
    LayerMask defaultMask;

    void Start()
    {
        defaultMask = gameObject.layer;
        ignorePlayerMask = LayerMask.NameToLayer("IgnorePlayer");
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void Enable(bool enable)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = enable; //Visual

            //Physics
            if (enable) meshRenderer.gameObject.layer = defaultMask;
            else meshRenderer.gameObject.layer = ignorePlayerMask;
        }
    }

    public void TransparentMode(bool enable)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            //Visual
            if (meshRenderer.CompareTag("Addition"))
            {
                Color c = meshRenderer.material.color;
                c.a = 1f;

                if (enable) c.a = 0.5f;

                meshRenderer.material.color = c;

                meshRenderer.enabled = enable;
            }

            //Physics
            if (enable) meshRenderer.gameObject.layer = ignorePlayerMask;
            else meshRenderer.gameObject.layer = defaultMask;
        }
    }
}
