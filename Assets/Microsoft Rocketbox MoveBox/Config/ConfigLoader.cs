using System;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConfigLoader))]
public class ConfigLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
    }
}

public class ConfigLoader : MonoBehaviour
{
    public static ConfigLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        LoadDemoSceneSetup();
    }

    // Name of scene config file.
    private const string gameDataFileName = "config.json";

    public Configs Configs { get; private set; } = new Configs();

    private void LoadDemoSceneSetup()
    {
        // Path.Combine combines strings into a file path.
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build.
        string filePath = Path.Combine(Application.dataPath+"/Microsoft Rocketbox MoveBox/Config/", gameDataFileName);
        if (File.Exists(filePath))
        {
            // Read the json from the file into a string.
            string dataAsJson = File.ReadAllText(filePath);

            // Pass the json to JsonUtility, and tell it to create a Configs object from it.
            Configs = JsonUtility.FromJson<Configs>(dataAsJson);

            UnityEngine.Debug.Log("Successfully loaded config file.");
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }
}
