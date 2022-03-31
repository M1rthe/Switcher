//#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimelineDisplayEditor : EditorWindow
{
    GameObject past = null;
    GameObject present = null;
    GameObject future = null;

    [MenuItem("Window/Timeline Switcher")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TimelineDisplayEditor));
    }

    void OnGUI()
    {
        GUILayout.Space(15);

        GUILayout.Label("Switch Timeline:", EditorStyles.boldLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Everything (build)"))
        {
            CheckNULL();

            if (past != null && present != null && future != null)
            {
                Enable(past, true);
                Enable(present, true);
                Enable(future, true);
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Past"))
        {
            CheckNULL();

            if (past != null && present != null && future != null)
            {
                Enable(past, true);
                Enable(present, false);
                Enable(future, false);
            }
        }

        GUILayout.Space(3);

        if (GUILayout.Button("Present"))
        {
            CheckNULL();

            if (past != null && present != null && future != null)
            {
                Enable(past, false);
                Enable(present, true);
                Enable(future, false);
            }
        }

        GUILayout.Space(3);

        if (GUILayout.Button("Future"))
        {
            CheckNULL();

            if (past != null && present != null && future != null)
            {
                Enable(past, false);
                Enable(present, false);
                Enable(future, true);
            } 
        }
    }

    void Enable(GameObject gameObject, bool enable)
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        Canvas[] canvasses = gameObject.GetComponentsInChildren<Canvas>();
        Light[] lights = gameObject.GetComponentsInChildren<Light>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = enable;

            if (enable) meshRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
            else meshRenderer.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
        }

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (enable) particleSystem.Play();
            else particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        foreach (Canvas canvas in canvasses)
        {
            canvas.enabled = enable;
        }

        foreach (Light light in lights)
        {
            light.enabled = enable;
        }
    }

    void CheckNULL()
    {
        if (past == null || present == null || future == null)
        {
            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();

            foreach (Transform transform in transforms)
            {
                if (transform.tag == "Past") past = transform.gameObject;
                if (transform.tag == "Present") present = transform.gameObject;
                if (transform.tag == "Future") future = transform.gameObject;
            }
        }
    }
}

//#endif