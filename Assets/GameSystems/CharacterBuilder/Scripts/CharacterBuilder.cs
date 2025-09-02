using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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
    [SerializeField] private CharacterTemplate template;

    [Header("UI")]
    [SerializeField] private Button randomizeButton;

    private CharacterData currentCharacter;

    private void Awake()
    {
        if (randomizeButton != null)
            randomizeButton.onClick.AddListener(RandomizeCharacter);
    }

    /// <summary>
    /// Creates a new random character and applies it to the template.
    /// Safe for UI button use.
    /// </summary>
    public void RandomizeCharacter()
    {
        if (template == null) return;

        currentCharacter = new CharacterData(randomize: true);
        template.ApplyAllParts(currentCharacter);
    }

    /// <summary>
    /// Programmatically builds a character from existing CharacterData.
    /// </summary>
    public void BuildCharacter(CharacterData data)
    {
        if (template == null || data == null) return;

        currentCharacter = data;
        template.ApplyAllParts(data);
    }

    /// <summary>
    /// Returns the current character data for saving or other use.
    /// </summary>
    public CharacterData GetCurrentCharacterData()
    {
        return currentCharacter;
    }
}
