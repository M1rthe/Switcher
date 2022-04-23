#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public class TakeLevelPreviewPic : EditorWindow
{
    Vector3 newCamPos;
    Vector3 newCamRot;
    Vector2 cropOffset;
    float zoom = 80;
    string path = "Resources/LevelPreview/";
    string outputName;

    [MenuItem("CustomWindows/Take level preview pic")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TakeLevelPreviewPic));
    }

    void OnGUI()
    {
        GUILayout.Space(15);
        GUILayout.Label("Level Preview Pic:", EditorStyles.boldLabel);
        GUILayout.Space(10);

        //Cam pos
        newCamPos = EditorGUILayout.Vector3Field("Editor camera position: ", newCamPos);
        newCamRot = EditorGUILayout.Vector3Field("Editor camera rotation: ", newCamRot);
        GUILayout.Space(5);

        if (GUILayout.Button("Set this transform"))
        {
            newCamPos = SceneView.lastActiveSceneView.pivot;
            newCamRot = SceneView.lastActiveSceneView.rotation.eulerAngles;
        }
        GUILayout.Space(2);

        if (GUILayout.Button("Snap editor cam to position"))
        {
            SceneView.lastActiveSceneView.pivot = newCamPos;
            SceneView.lastActiveSceneView.rotation = Quaternion.Euler(newCamRot);
            SceneView.lastActiveSceneView.Repaint();
        }
        GUILayout.Space(20);

        //Export
        GUILayout.Label("Path: ");
        path = GUILayout.TextField(path);
        GUILayout.Space(5);

        GUILayout.Label("Output name: ");
        outputName = GUILayout.TextField(outputName);
        GUILayout.Space(5);

        cropOffset = EditorGUILayout.Vector2Field("Crop factor: ", cropOffset);

        zoom = EditorGUILayout.FloatField("Zoom: ", zoom);

        if (GUILayout.Button("Export"))
        {
            Debug.Log("Export cam at "+newCamPos+" to "+path+ " as '"+outputName+"'");

            byte[] screenshot = Screenshot();

            File.WriteAllBytes(Path.Combine(Application.dataPath + "/" + path, outputName + ".png"), screenshot);
            TextureImporter textureImporter = AssetImporter.GetAtPath("Assets/"+path + outputName + ".png") as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            AssetDatabase.Refresh();
        }
    }

    byte[] Screenshot()
    {
        //Get active EditorWindow
        SceneView sceneWindow = GetWindow<SceneView>();

        //sceneWindow.maximized = true;
        //sceneWindow.Repaint();

        //Get screen position and size
        var pos = sceneWindow.position.position;
        var sizeX = sceneWindow.position.width;
        var sizeY = sceneWindow.position.height;

        //Take Screenshot at given position sizes
        Color[] colors = InternalEditorUtility.ReadScreenPixel(pos, (int)sizeX, (int)sizeY);

        //Write result Color[] data into a temporal Texture2D
        Texture2D result = new Texture2D((int)sizeX, (int)sizeY);
        result.SetPixels(colors);

        //CROP
        int newWidth = (int)(sizeX * zoom * 0.01);
        int newHeight = (int)(sizeY * zoom * 0.01);
        Texture2D croppedTexture = new Texture2D(newHeight, newHeight);
        croppedTexture.SetPixels(result.GetPixels(
            (int)((newWidth - newHeight) * cropOffset.x),
            (int)((sizeY - newHeight) * cropOffset.y),
            (int)newHeight,
            (int)newHeight)
        );
        croppedTexture.Apply();
        result = croppedTexture;

        //sceneWindow.maximized = false;
        sceneWindow.Repaint();

        //Encode the Texture2D to a PNG
        byte[] bytes = result.EncodeToPNG();

        //Clean up
        DestroyImmediate(result);

        return bytes;
    }
}

#endif
