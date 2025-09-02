using UnityEngine;

/// <summary>
/// Handles building a character from preloaded prefabs and materials.
/// </summary>
public class CharacterTemplate : MonoBehaviour
{
    [Header("Slot Parents")]
    public Transform faceSlot;
    public Transform headSlot;
    public Transform torsoSlot;
    public Transform handsSlot;

    private GameObject currentFace;
    private GameObject currentHead;
    private GameObject currentTorso;
    private GameObject currentHands;

    /// <summary>
    /// Builds or rebuilds the entire character from CharacterData.
    /// </summary>
    public void ApplyAllParts(CharacterData data)
    {
        // Destroy previous instances
        if (currentFace != null) Destroy(currentFace);
        if (currentHead != null) Destroy(currentHead);
        if (currentTorso != null) Destroy(currentTorso);
        if (currentHands != null) Destroy(currentHands);

        // Apply new parts
        currentFace = ApplyPart(data.FaceID, faceSlot);
        currentHead = ApplyPart(data.HeadID, headSlot);
        currentTorso = ApplyPart(data.TorsoID, torsoSlot);
        //currentHands = ApplyPart(data.HandsID, handsSlot);
    }

    /// <summary>
    /// Swaps a single part by CharacterPartID.
    /// </summary>
    public void SwapPart(CharacterPartID partID)
    {
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
            case CharacterPartType.Hands:
                parent = handsSlot;
                oldInstance = currentHands;
                break;
        }

        if (oldInstance != null) Destroy(oldInstance);

        var instance = ApplyPart(partID, parent);

        // Update current reference
        switch (part.partType)
        {
            case CharacterPartType.Face: currentFace = instance; break;
            case CharacterPartType.Head: currentHead = instance; break;
            case CharacterPartType.Torso: currentTorso = instance; break;
            case CharacterPartType.Hands: currentHands = instance; break;
        }
    }

    /// <summary>
    /// Instantiates the prefab and applies the preloaded material from GameData caches.
    /// Also sets up FaceBlinkController if present.
    /// </summary>
    private GameObject ApplyPart(CharacterPartID id, Transform parent)
    {
        if (!GameData.PrefabCache.TryGetValue(id, out var prefab))
            return null;

        GameObject instance = GameObject.Instantiate(prefab, parent);

        // Apply preloaded material
        if (GameData.MaterialCache.TryGetValue(id, out var mat))
        {
            var rend = instance.GetComponentInChildren<Renderer>();
            if (rend != null)
                rend.material = mat;
        }

        // Auto-setup FaceBlinkController if this is a face
        var blink = instance.GetComponent<FaceBlinkController>();
        if (blink != null)
        {
            blink.Initialize(); // detects vertical frame count and starts blinking
        }

        return instance;
    }
}
