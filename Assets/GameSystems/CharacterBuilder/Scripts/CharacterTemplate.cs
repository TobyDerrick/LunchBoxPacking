using System.Runtime.CompilerServices;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Handles building a character from preloaded prefabs and materials,
/// and applies per-part colors using a palette swap shader.
/// </summary>
public class CharacterTemplate : MonoBehaviour
{
    [Header("Slot Parents")]
    public Transform faceSlot;
    public Transform headSlot;
    public Transform torsoSlot;
    public Transform leftHand, rightHand;

    public GameObject currentFace;
    public GameObject currentHead;
    public GameObject currentTorso;
    public GameObject faceObject;

    public CharacterData currentCharacter;

    /// <summary>
    /// Builds or rebuilds the entire character from CharacterData.
    /// </summary>
    public void ApplyAllParts(CharacterData data)
    {
        currentCharacter = data;

        // Destroy previous instances
        if (currentFace != null) Destroy(currentFace);
        if (currentHead != null) Destroy(currentHead);
        if (currentTorso != null) Destroy(currentTorso);

        // Apply new parts
        currentFace = ApplyPart(data.EyesID, faceSlot, data);
        currentHead = ApplyPart(data.HeadID, headSlot, data);
        currentTorso = ApplyPart(data.TorsoID, torsoSlot, data);

        ApplyColors(leftHand.GetComponent<Renderer>(), CharacterPartType.Hands, data);
        ApplyColors(rightHand.GetComponent<Renderer>(), CharacterPartType.Hands, data);

    }

    /// <summary>
    /// Swaps a single part by CharacterPartID while keeping colors.
    /// </summary>
    public void SwapPart(string partID)
    {
        if (currentCharacter == null)
        {
            Debug.LogWarning("No character data stored. Call ApplyAllParts first.");
            return;
        }

        var part = GameData.CharacterParts.Get(partID);
        if (part == null) return;

        Transform parent = null;
        GameObject oldInstance = null;

        switch (part.partType)
        {
            case CharacterPartType.Face:
                parent = faceSlot;
                oldInstance = currentFace;
                break;
            case CharacterPartType.Head:
                parent = headSlot;
                oldInstance = currentHead;
                break;
            case CharacterPartType.Torso:
                parent = torsoSlot;
                oldInstance = currentTorso;
                break;

        }

        if (oldInstance != null) Destroy(oldInstance);

        var instance = ApplyPart(partID, parent, currentCharacter);

        // Update current reference
        switch (part.partType)
        {
            case CharacterPartType.Face: currentFace = instance; break;
            case CharacterPartType.Head: currentHead = instance; break;
            case CharacterPartType.Torso: currentTorso = instance; break;
        }
    }

    /// <summary>
    /// Instantiates the prefab and applies preloaded material and colors.
    /// Also sets up FaceBlinkController if present.
    /// </summary>
    private GameObject ApplyPart(string id, Transform parent, CharacterData data)
    {
        if (!GameData.PrefabCache.TryGetValue(id, out var prefab))
            return null;

        GameObject instance = GameObject.Instantiate(prefab, parent);

        // Apply preloaded material and palette colors
        if (GameData.MaterialCache.TryGetValue(id, out var mat))
        {
            var rend = instance.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                rend.material = mat;
                var part = GameData.CharacterParts.Get(id);
                ApplyColors(rend, part.partType, data);
            }
        }

        // Auto-setup FaceBlinkController if this is a face
        var blink = instance.GetComponent<FaceBlinkController>();
        if (blink != null)
        {
            blink.Initialize();
        }

        return instance;
    }

    public void SetPartColour(CharacterPartType partType,  Color newColour)
    {
        GameObject partObject = null;
        newColour.a = 1;

        switch (partType)
        {
            case CharacterPartType.Skin:
                currentCharacter.Skin = newColour;
                partObject = currentHead;
                break;
            case CharacterPartType.Head:
                currentCharacter.HairBase = newColour;
                currentCharacter.HairShadow = new Color(newColour.r * 0.5f, newColour.g * 0.5f, newColour.b * 0.5f, 1);
                partObject = currentHead;
                break;
            case CharacterPartType.Torso:
                currentCharacter.Shirt = newColour;
                partObject = currentTorso;
                break;
            case CharacterPartType.Face:
                currentCharacter.EyeBase = newColour;
                currentCharacter.EyeShadow = new Color(newColour.r * 0.5f, newColour.g * 0.5f, newColour.b * 0.5f, 1);
                partObject = currentFace;
                break;
        }

        ApplyColors(partObject.GetComponent<Renderer>(), partType, currentCharacter);
        ApplyColors(leftHand.GetComponent<Renderer>(), CharacterPartType.Hands, currentCharacter);
        ApplyColors(rightHand.GetComponent<Renderer>(), CharacterPartType.Hands, currentCharacter);
    }

    /// <summary>
    /// Sets up MaterialPropertyBlock colors for a specific renderer and part type.
    /// </summary>
    public void ApplyColors(Renderer rend, CharacterPartType type, CharacterData data)
    {
        var mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        switch (type)
        {
            case CharacterPartType.Head:
                mpb.SetColor("_BaseColor", data.HairBase);
                mpb.SetColor("_ShadowColor", data.HairShadow);
                mpb.SetColor("_HighlightColor", data.HairHighlight);
                mpb.SetColor("_SkinColor", data.Skin);
                break;

            case CharacterPartType.Skin:
                //mpb.SetColor("_BaseColor", data.EyeBase);
                //mpb.SetColor("_ShadowColor", data.EyeShadow);
                //mpb.SetColor("_HighlightColor", data.EyeHighlight);
                mpb.SetColor("_SkinColor", data.Skin);
                break;

            case CharacterPartType.Torso:
                mpb.SetColor("_BaseColor", data.Shirt);
                break;

            case CharacterPartType.Hands:
                mpb.SetColor("_SkinColor", data.Skin);
                break;
            case CharacterPartType.Face:
                mpb.SetColor("_BaseColor", data.EyeBase);
                mpb.SetColor("_ShadowColor", data.EyeShadow);
                mpb.SetColor("_HighlightColor", data.EyeHighlight);
                break;
        }

        rend.SetPropertyBlock(mpb);
    }
}
