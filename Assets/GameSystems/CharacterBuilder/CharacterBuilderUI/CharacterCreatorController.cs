using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CharacterCreatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterBuilder builder;
    [SerializeField] private Transform spawnPosition;

    [Header("Tabs")]
    [SerializeField] private Button faceTab;
    [SerializeField] private Button headTab;
    [SerializeField] private Button torsoTab;
    [SerializeField] private Button handsTab;

    [Header("Panels")]
    [SerializeField] private PartPanel facePanel;
    [SerializeField] private PartPanel headPanel;
    [SerializeField] private PartPanel torsoPanel;
    [SerializeField] private PartPanel handsPanel;

    private Dictionary<CharacterPartType, List<CharacterPartScriptableObject>> partLookup;
    private PartPanel currentPanel;

    private async void Awake()
    {
        await GameManager.EnsureInitialized();

        // Build lookup table from GameData
        partLookup = new Dictionary<CharacterPartType, List<CharacterPartScriptableObject>>();
        foreach (var part in GameData.CharacterParts.GetAll())
        {
            if (!partLookup.ContainsKey(part.partType))
                partLookup[part.partType] = new List<CharacterPartScriptableObject>();
            partLookup[part.partType].Add(part);
        }

        // Spawn a random character
        GameObject go = builder.RandomizeCharacter(spawnPosition);

        // Setup tab clicks
        faceTab.onClick.AddListener(() => ShowParts(CharacterPartType.Face));
        headTab.onClick.AddListener(() => ShowParts(CharacterPartType.Head));
        torsoTab.onClick.AddListener(() => ShowParts(CharacterPartType.Torso));
        //handsTab.onClick.AddListener(() => ShowParts(CharacterPartType.Hands));

        // Default tab
        ShowParts(CharacterPartType.Head);
    }

    private void ShowParts(CharacterPartType type)
    {
        // Deinit current panel
        if (currentPanel != null)
            currentPanel.Deinitialize();

        // Choose the correct panel
        switch (type)
        {
            case CharacterPartType.Face:
                currentPanel = facePanel;
                break;
            case CharacterPartType.Head:
                currentPanel = headPanel;
                break;
            case CharacterPartType.Torso:
                currentPanel = torsoPanel;
                break;
            case CharacterPartType.Hands:
                currentPanel = handsPanel;
                break;
        }

        // Init panel and give it data
        if (currentPanel != null && partLookup.TryGetValue(type, out var parts))
        {
            currentPanel.Setup(type, parts, builder);
            currentPanel.Initialize();
        }
    }
}
