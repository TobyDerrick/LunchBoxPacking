using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[Serializable]
public class CharacterData
{
    public string EyesID;
    public string HeadID;
    public string TorsoID;
    public string HandsID;

    // Hair (Head material)
    public Color HairBase = new Color(0.6f, 0.3f, 0.1f);
    public Color HairShadow = new Color(0.3f, 0.15f, 0.05f);
    public Color HairHighlight = new Color(0.9f, 0.6f, 0.3f);

    // Eyes (Face material)
    public Color EyeBase = Color.black;
    public Color EyeShadow = new Color(0.1f, 0.1f, 0.1f);
    public Color EyeHighlight = Color.white;

    // Skin (Face + Hands)
    public Color Skin = new Color(1f, 0.8f, 0.6f);

    // Shirt (Torso)
    public Color Shirt = Color.blue;

    /// <summary>
    /// Constructor: optionally randomize the character parts.
    /// </summary>
    /// <param name="randomize">If true, picks random parts for each slot.</param>
    public CharacterData(bool randomize = false, CharacterPalette palette = null)
    {
        if (randomize)
        {
            EyesID = GetRandomPartOfType(CharacterPartType.Face);
            HeadID = GetRandomPartOfType(CharacterPartType.Head);
            TorsoID = GetRandomPartOfType(CharacterPartType.Torso);
            HandsID = GetRandomPartOfType(CharacterPartType.Hands);

            if(palette != null)
            {
                RandomizeColors(palette);
            }
        }
    }

    private string GetRandomPartOfType(CharacterPartType type)
    {
        var parts = GameData.CharacterParts.GetAll()
            .Where(p => p.partType == type)
            .ToArray();

        if (parts.Length == 0)
            throw new Exception($"No parts found for type {type}");

        int index = UnityEngine.Random.Range(0, parts.Length);
        return parts[index].id;
    }

    public void RandomizeColors(CharacterPalette palette)
    {
        HairBase = palette.GetRandomHair();
        HairShadow = HairBase * 0.5f;
        HairHighlight = HairBase * 1.2f;

        EyeBase = palette.GetRandomEye();
        EyeShadow = EyeBase * 0.5f;
        EyeHighlight = EyeBase * 1.2f;

        Skin = palette.GetRandomSkin();
        Shirt = palette.GetRandomShirt();
    }
}

public class CharacterBuilder : MonoBehaviour
{
    [Header("Character Template")]
    [SerializeField] private GameObject templatePrefab;

    [Header("UI")]
    [SerializeField] private Button randomizeButton;

    public GameObject currentInstance;
    public CharacterTemplate templateComponent;
    public CharacterPalette characterPalette;
    private CharacterData currentCharacter;

    [SerializeField]
    private Transform spawnPosition;

    private void Awake()
    {
        if (randomizeButton != null)
            randomizeButton.onClick.AddListener(() => { _ = RandomizeCharacter(spawnPosition: spawnPosition); });
    }

    /// <summary>
    /// Creates a new random character with randomized parts and colors.
    /// </summary>
    public GameObject RandomizeCharacter(Transform spawnPosition = null)
    {
        if (templatePrefab == null) return null;

        // Destroy previous instance
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }

        currentCharacter = new CharacterData(randomize: true, palette: characterPalette);

        // Instantiate template and apply parts + colors
        currentInstance = Instantiate(templatePrefab);
        templateComponent = currentInstance.GetComponent<CharacterTemplate>();
        templateComponent.ApplyAllParts(currentCharacter);

        if(spawnPosition != null)
        {
            currentInstance.transform.SetParent(spawnPosition.transform);
        }

        if (templateComponent != null)
        {
            currentInstance.transform.position = spawnPosition.position;
        }

        EventBus.EmitNewCharacterBuilt(currentCharacter);
        return currentInstance;
    }

    /// <summary>
    /// Builds a character from existing CharacterData.
    /// </summary>
    public GameObject BuildCharacter(CharacterData data)
    {
        if (templatePrefab == null || data == null) return null;

        currentInstance = Instantiate(templatePrefab);
        templateComponent = currentInstance.GetComponent<CharacterTemplate>();
        templateComponent.ApplyAllParts(data);

        currentCharacter = data;
        return currentInstance;
    }

    /// <summary>
    /// Returns the current character data.
    /// </summary>
    public CharacterData GetCurrentCharacterData()
    {
        return currentCharacter;
    }
}
