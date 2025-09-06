using UnityEngine;
using System.Collections.Generic;

public class NPCSaveLoadManager : MonoBehaviour
{
    private List<NPCData> savedNPCs = new List<NPCData>();

    public void SaveNPC(string npcName, CharacterData characterData)
    {
        NPCData newNPC = ScriptableObject.CreateInstance<NPCData>();
        newNPC.name = npcName;
        newNPC.characterData = characterData;

        savedNPCs.Add(newNPC);


        Debug.Log($"Saved NPC: {npcName}");
    }

    public IEnumerable<NPCData> GetSavedNPCS()
    {
        return savedNPCs;
    }
}
