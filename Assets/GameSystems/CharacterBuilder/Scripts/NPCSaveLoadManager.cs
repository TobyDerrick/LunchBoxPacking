using UnityEngine;
using System.Collections.Generic;

public class NPCSaveLoadManager : MonoBehaviour
{
    public int maxSlots = 12;
    private List<NPCData> savedNPCs = new List<NPCData>();

    private void Awake()
    {
        // Initialize the slot list with nulls (fixed size, accessible by index)
        savedNPCs = new List<NPCData>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
        {
            savedNPCs.Add(null); // all slots start empty
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
    }
}
