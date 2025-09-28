using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum CharacterPartType
{
    Skin,
    Head,
    Torso,
    Hands,
    None,
    Face,
}

[CreateAssetMenu(menuName = "Character/Part")]
public class CharacterPartScriptableObject : ScriptableObject
{
    public string id;
    public CharacterPartType partType;

    public AssetReferenceT<Material> material;
    public AssetReferenceT<GameObject> prefab;

    public Texture2D spriteSheet;

    public Sprite[] idleFrames;
    public Sprite[] hoverFrames;
    public Sprite[] pressedFrames;

    [Tooltip("Number of frames for each state, in order: Click, Hover, Idle")]
    public int[] frameCounts;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (spriteSheet != null)
        {
            // Auto-set frameCounts based on partType
            switch (partType)
            {
                case CharacterPartType.Face:
                    frameCounts = new int[] { 5, 4, 0 };
                    break;
                default:
                    frameCounts = new int[] { 6, 5, 1 };
                    break;
            }

            SliceFrames();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif

    private void SliceFrames()
    {
        string path = UnityEditor.AssetDatabase.GetAssetPath(spriteSheet);
        var allAssets = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

        // Collect only sprites
        var spriteList = new System.Collections.Generic.List<Sprite>();
        foreach (var asset in allAssets)
        {
            if (asset is Sprite sprite)
                spriteList.Add(sprite);
        }

        if (spriteList.Count == 0)
        {
            Debug.LogError($"CharacterPartScriptableObject: No sprites found in {spriteSheet.name}");
            return;
        }

        var allSprites = spriteList.ToArray();

        int totalFrames = frameCounts[0] + frameCounts[1] + frameCounts[2];
        if (allSprites.Length < totalFrames)
        {
            Debug.LogError($"CharacterPartScriptableObject: Not enough sprites in {spriteSheet.name}. Found {allSprites.Length}, expected {totalFrames}.");
            return;
        }

        int index = 0;
        pressedFrames = SubArray(allSprites, index, frameCounts[0]); index += frameCounts[0];
        hoverFrames = SubArray(allSprites, index, frameCounts[1]); index += frameCounts[1];

        if (partType == CharacterPartType.Face)
        {
            idleFrames = new Sprite[] { allSprites[4] };
        }
        else
        {
            idleFrames = SubArray(allSprites, index, frameCounts[2]);
        }
    }

    private Sprite[] SubArray(Sprite[] data, int index, int length)
    {
        Sprite[] result = new Sprite[length];
        for (int i = 0; i < length; i++)
            result[i] = data[index + i];
        return result;
    }
}

