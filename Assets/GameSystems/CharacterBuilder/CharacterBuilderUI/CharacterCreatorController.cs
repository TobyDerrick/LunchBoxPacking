using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class CharacterCreatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterBuilder builder;
    [SerializeField] private Transform scrollContent; // Content of ScrollView
    [SerializeField] private Button partButtonPrefab; // Prefab for part buttons
    [SerializeField] private Transform spawnPosition;

    [Header("Tabs")]
    [SerializeField] private Button faceTab;
    [SerializeField] private Button headTab;
    [SerializeField] private Button torsoTab;
    [SerializeField] private Button handsTab;

    private Dictionary<CharacterPartType, List<CharacterPartScriptableObject>> partLookup;

    private async void Awake()
    {

        await GameManager.EnsureInitialized();
        // Setup tab clicks
        faceTab.onClick.AddListener(() => ShowParts(CharacterPartType.Face));
        headTab.onClick.AddListener(() => ShowParts(CharacterPartType.Head));
        torsoTab.onClick.AddListener(() => ShowParts(CharacterPartType.Torso));
        //handsTab.onClick.AddListener(() => ShowParts(CharacterPartType.Hands));

        // Build lookup table from GameData
        partLookup = new Dictionary<CharacterPartType, List<CharacterPartScriptableObject>>();
        foreach (var part in GameData.CharacterParts.GetAll())
        {
            if (!partLookup.ContainsKey(part.partType))
                partLookup[part.partType] = new List<CharacterPartScriptableObject>();
            partLookup[part.partType].Add(part);
        }


        GameObject go = builder.RandomizeCharacter();
        go.transform.position = spawnPosition.position;
        

        // Default to Face tab
        ShowParts(CharacterPartType.Face);
    }

    private void ShowParts(CharacterPartType type)
    {
        // Clear existing buttons
        foreach (Transform child in scrollContent)
            Destroy(child.gameObject);

        if (!partLookup.TryGetValue(type, out var parts))
            return;

        // Create button for each part
        foreach (CharacterPartScriptableObject part in parts)
        {
            Button btn = Instantiate(partButtonPrefab, scrollContent);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = part.name;

            // Capture part in a local variable for the closure
            var capturedPart = part;

            btn.onClick.AddListener(() =>
            {
                builder.templateComponent.SwapPart(capturedPart.id);
            });
        }
    }
}
