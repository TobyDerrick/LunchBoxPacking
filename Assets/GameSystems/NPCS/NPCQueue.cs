using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCQueue", menuName = "Scriptable Objects/NPCQueue")]
public class NPCQueue : ScriptableObject
{
    public List<NPCData> npcs;

    public void Initialize(int totalCount)
    {
        npcs = new List<NPCData>();
        for (int i = 0; i < totalCount; i++)
        {
            npcs.Add(CreateInstance<NPCData>());
        }
    }

    public NPCData Dequeue()
    {
        if (npcs.Count == 0) return null;

        NPCData leaving = npcs[0];
        npcs.RemoveAt(0);
        return leaving;
    }
}
