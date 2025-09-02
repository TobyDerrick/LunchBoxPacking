using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Threading.Tasks;

[Serializable]
public class CharacterData
{
    public CharacterPartID FaceID;
    public CharacterPartID HeadID;
    public CharacterPartID TorsoID;
    public CharacterPartID HandsID;

    /// <summary>
    /// Constructor: optionally randomize the character parts.
    /// </summary>
    /// <param name="randomize">If true, picks random parts for each slot.</param>
    public CharacterData(bool randomize = false)
    {
        if (randomize)
        {
            FaceID = GetRandomPartOfType(CharacterPartType.Face);
            HeadID = GetRandomPartOfType(CharacterPartType.Head);
            TorsoID = GetRandomPartOfType(CharacterPartType.Torso);
            HandsID = GetRandomPartOfType(CharacterPartType.Hands);
        }
    }

    private CharacterPartID GetRandomPartOfType(CharacterPartType type)
    {
        var parts = GameData.CharacterParts.GetAll()
            .Where(p => p.partType == type)
            .ToArray();

        if (parts.Length == 0)
            throw new Exception($"No parts found for type {type}");

        int index = UnityEngine.Random.Range(0, parts.Length);
        return parts[index].id;
    }
}

public class CharacterBuilder : MonoBehaviour
{
    [Header("Character Template")]
    [SerializeField] private GameObject templatePrefab;

    [Header("UI")]
    [SerializeField] private Button randomizeButton;

    private CharacterData currentCharacter;
    public GameObject currentInstance;
    public CharacterTemplate templateComponent;

    private void Awake()
    {
        if (randomizeButton != null)
            randomizeButton.onClick.AddListener(() => { _ = RandomizeCharacter(); });


        //await GameManager.EnsureInitialized();
        //RandomizeCharacter();
    }

    /// <summary>
    /// Creates a new random character and applies it to the template.
    /// Safe for UI button use.
    /// </summary>
    public GameObject RandomizeCharacter()
    {
        if (templatePrefab == null)
            return null;

        // Destroy previous character if exists
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }

        currentCharacter = new CharacterData(randomize: true);

        // Instantiate new character
        currentInstance = GameObject.Instantiate(templatePrefab);
        templateComponent = currentInstance.GetComponent<CharacterTemplate>();
        templateComponent.ApplyAllParts(currentCharacter);

        return currentInstance;
    }

    /// <summary>
    /// Programmatically builds a character from existing CharacterData.
    /// </summary>
    public GameObject BuildCharacter(CharacterData data)
    {
        if (templatePrefab == null || data == null)
            return null;

        GameObject templateInstance = GameObject.Instantiate(templatePrefab);
        templateComponent = templateInstance.GetComponent<CharacterTemplate>();
        templateComponent.ApplyAllParts(data);

        currentInstance = templateInstance;
        currentCharacter = data;

        return templateInstance;
    }

    /// <summary>
    /// Returns the current character data for saving or other use.
    /// </summary>
    public CharacterData GetCurrentCharacterData()
    {
        return currentCharacter;
    }
}
