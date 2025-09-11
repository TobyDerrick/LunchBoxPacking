using UnityEngine;
using System.Collections.Generic;
using System;

public class NPCSaveLoadManager : MonoBehaviour, ISaveable
{
    public int maxSlots = 12;
    private List<NPCData> savedNPCs = new List<NPCData>();

    public string UniqueID => "NPCManager";

    private void OnEnable()
    {
        SaveManager.Register(UniqueID, this);
    }

    private void OnDisable()
    {
        SaveManager.Unregister(UniqueID);
    }

    private void Awake()
    {
        savedNPCs = new List<NPCData>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
        {
            savedNPCs.Add(null);
        }
    }

    public void SaveNPC(int slotIndex, string npcName, CharacterData characterData)
    {
        if(slotIndex < 0 || slotIndex >= savedNPCs.Count)
        {
            Debug.LogError($"Invalid slot index {slotIndex}");
            return;
        }


        NPCData npc = ScriptableObject.CreateInstance<NPCData>();
        npc.npcName = npcName;
        npc.name = npcName;
        npc.characterData = characterData;

        savedNPCs[slotIndex] = npc;


        SaveManager.SaveGame();
        Debug.Log($"Saved NPC '{npcName}' to slot {slotIndex}");
    }


    public NPCData LoadNPCFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= savedNPCs.Count)
        {
            Debug.LogError($"Invalid slot index {slotIndex}");
            return null;
        }

        return savedNPCs[slotIndex];
    }
    public List<NPCData> GetSavedNPCS()
    {
        return savedNPCs;
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= savedNPCs.Count)
        {
            Debug.LogError($"Invalid slot index {slotIndex}");
            return;
        }

        savedNPCs[slotIndex] = null;
        Debug.Log($"Cleared NPC slot {slotIndex}");

        SaveManager.SaveGame();
    }

    public SaveData CaptureState()
    {
        NPCSaveData saveData = new();

        foreach (NPCData npc in savedNPCs)
        {
            if (npc != null)
            {
                saveData.entries.Add(new NPCSaveEntry
                {
                    npcName = npc.npcName,
                    characterData = npc.characterData
                });
            }

            else
            {
                saveData.entries.Add(null);
            }
        }

        return saveData;
    }

    public void RestoreState(SaveData state)
    {
        NPCSaveData saveData = state as NPCSaveData;
        if (saveData == null) return;

        savedNPCs.Clear();
        foreach(NPCSaveEntry entry in saveData.entries)
        {
            if(entry == null)
            {
                savedNPCs.Add(null);
                continue;
            }

            NPCData npc = ScriptableObject.CreateInstance<NPCData>();
            npc.npcName = entry.npcName;
            npc.characterData = entry.characterData;

            savedNPCs.Add(npc);
        }
    }
}

[Serializable]
public class NPCSaveData : SaveData
{
    public List<NPCSaveEntry> entries = new List<NPCSaveEntry>();
}

[Serializable]
public class NPCSaveEntry
{
    public string npcName;
    public CharacterData characterData;
}