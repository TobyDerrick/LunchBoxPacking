using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spacing = 2f;
    [SerializeField] private GameObject npcPrefab;

    [SerializeField]
    private NPCQueue npcQueue;
    private List<GameObject> npcInstances = new();

    private void Awake()
    {
        npcQueue = ScriptableObject.CreateInstance<NPCQueue>();
        npcQueue.Initialize(50);
        Initialize(npcQueue);
        EventBus.OnRequestValidated += ShiftQueue;
    }

    private void Start()
    {
        Debug.Log("OWCHHH");
        EventBus.EmitNewNPC(npcQueue.npcs[0]);
    }


    public void Initialize(NPCQueue queue)
    {
        npcQueue = queue;

        for (int i = 0; i < npcQueue.npcs.Count; i++)
        {
            SpawnNPC(npcQueue.npcs[i], i);
        }
    }

    private void SpawnNPC(NPCData npcData, int Index)
    {
        Vector3 pos = spawnPoint.position + Vector3.left * Index * spacing;
        Debug.Log("Instantiating npc");
        GameObject go = Instantiate(npcPrefab, pos, Quaternion.identity, transform);
        go.name = npcData.name;
        go.GetComponent<MeshRenderer>().material.color = npcData.npcColor;
        npcInstances.Add(go);
    }

    public void ShiftQueue(float Score, Lunchbox box)
    {
        if (npcInstances.Count == 0) return;
        GameObject leaving = npcInstances[0];
        box.gameObject.transform.SetParent(leaving.transform);
        Sequence seq = DOTween.Sequence();

        npcInstances.RemoveAt(0);
        leaving.transform.DOMoveX(leaving.transform.position.x + spacing * 4f, 0.75f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                Destroy(leaving);
                SlideInNextNPCS();
            });

        npcQueue.Dequeue();
    }

    private void SlideInNextNPCS()
    {
        for (int i = 0; i < npcInstances.Count; i++)
        {
            Vector3 targetPos = spawnPoint.position + Vector3.left * i * spacing;
            npcInstances[i].transform.DOMove(targetPos, 0.5f)
                .SetEase(Ease.OutBounce);
        }

        EventBus.EmitNewNPC(npcQueue.npcs[0]);
    }
}
