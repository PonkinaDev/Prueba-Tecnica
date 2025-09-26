using UnityEngine;
using System.IO;

// Base class for handling JSON data persistence.

public abstract class JsonHandler : MonoBehaviour
{
    [Header("Nombre del archivo JSON (sin extensión) en Resources")]
    [SerializeField] protected bool eraseDataOnStart = false;
    [SerializeField] private string jsonFileName;
    protected string persistentPath;

// Initializes persistent path and loads default JSON if needed.

    protected virtual void Awake()
    {
        persistentPath = Path.Combine(Application.persistentDataPath, jsonFileName + ".json");

        // If file exists and eraseDataOnStart is false, skip loading default data.
        if (File.Exists(persistentPath) && !eraseDataOnStart)
            return;

        // Load default JSON from Resources and write to persistent path.
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile != null)
        {
            File.WriteAllText(persistentPath, jsonFile.text);
        }
        else
        {
            // Disable component if default JSON is missing.
            enabled = false;
        }
    }


// Loads data from persistent JSON file.
// <typeparam name="T">Type of data to load.</typeparam>
// <returns>Deserialized data or null if not found/invalid.</returns>
    protected T LoadJsonToGame<T>() where T : class
    {
        if (!File.Exists(persistentPath))
            return null;

        string json = File.ReadAllText(persistentPath);
        T data = JsonUtility.FromJson<T>(json);

        // Allow child class to validate or modify loaded data.
        if (!OnAfterLoad(data))
            return null;

        return data;
    }


// Saves data to persistent JSON file.

// <typeparam name="T">Type of data to save.</typeparam>
// <param name="data">Data to serialize and save.</param>
    protected void SaveDataToJson<T>(T data) where T : class
    {
        if (data == null)
            return;

        // Allow child class to validate or modify data before saving.
        if (!OnBeforeSave(data))
            return;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(persistentPath, json);
    }


// Hook for child classes after loading data.

// <typeparam name="T">Type of loaded data.</typeparam>
// <param name="data">Loaded data.</param>
// <returns>True if data is valid, false otherwise.</returns>
    protected abstract bool OnAfterLoad<T>(T data) where T : class;


// Hook for child classes before saving data.
// <typeparam name="T">Type of data to save.</typeparam>
// <param name="data">Data to save.</param>
// <returns>True if data can be saved, false otherwise.</returns>
    protected abstract bool OnBeforeSave<T>(T data) where T : class;
}
