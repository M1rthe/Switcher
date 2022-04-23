#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class TimelineDisplayEditor : EditorWindow
{
    bool useEditor = false;
    GameObject past = null;
    GameObject present = null;
    GameObject future = null;

    [MenuItem("CustomWindows/Timeline Switcher")]

    public static void ShowWindow()
    {
        GetWindow(typeof(TimelineDisplayEditor));
    }

    void OnGUI()
    {
        GUILayout.Space(15);

        GUILayout.Label("Switch Timeline:", EditorStyles.boldLabel);
        useEditor = GUILayout.Toggle(useEditor, "Use editor: ");

        GUILayout.Space(10);

        if (useEditor)
        {
            if (GUILayout.Button("Everything (build)"))
            {
                GetTimelineGO();

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
                GetTimelineGO();

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
                GetTimelineGO();

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
                GetTimelineGO();

                if (past != null && present != null && future != null)
                {
                    Enable(past, false);
                    Enable(present, false);
                    Enable(future, true);
                }
            }
        }
    }

    void Enable(GameObject gameObject, bool enable)
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        Canvas[] canvasses = gameObject.GetComponentsInChildren<Canvas>();
        Light[] lights = gameObject.GetComponentsInChildren<Light>();
        Volume[] volumes = gameObject.GetComponentsInChildren<Volume>();

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

        foreach (Volume volume in volumes)
        {
            volume.enabled = enable;
        }
    }

    void GetTimelineGO()
    {
        past = GameObject.FindGameObjectWithTag("Past");
        present = GameObject.FindGameObjectWithTag("Present");
        future = GameObject.FindGameObjectWithTag("Future");
    }
}

#endif