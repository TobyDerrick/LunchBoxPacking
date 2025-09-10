using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class SaveFile
{
    public Dictionary<string, SaveData> saveData = new Dictionary<string, SaveData>();
}

public static class SaveManager
{
    private static string saveFileName = "Game_Save.json";
    private static string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);

    private static Dictionary<string, ISaveable> saveableObjects = new Dictionary<string, ISaveable>();

    public static void Register(string id, ISaveable saveable)
    {
        if (!saveableObjects.ContainsKey(id))
            saveableObjects.Add(id, saveable);
    }

    public static void Unregister(string id)
    {
        saveableObjects.Remove(id);
    }

    public static void SaveGame()
    {
        SaveFile file = new SaveFile();

        foreach (var kvp in saveableObjects)
        {
            file.saveData[kvp.Key] = kvp.Value.CaptureState();
        }

        string json = JsonConvert.SerializeObject(file, Formatting.Indented,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game saved to {saveFilePath}");
    }

    public static void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveFile file = JsonConvert.DeserializeObject<SaveFile>(json,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

        foreach (var kvp in saveableObjects)
        {
            if (file.saveData.TryGetValue(kvp.Key, out SaveData state))
            {
                kvp.Value.RestoreState(state);
            }
        }

        Debug.Log("Game loaded");
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted");
        }
    }
}
