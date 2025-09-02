using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class GameData
{
    public static readonly DataGroup<FoodScriptableObject, FoodItemID> FoodItems =
        new("FoodItems", item => item.id);

    public static readonly DataGroup<CharacterPartScriptableObject, CharacterPartID> CharacterParts =
        new("CharacterParts", item => item.id);

    // Preloaded dictionaries
    public static readonly Dictionary<CharacterPartID, GameObject> PrefabCache = new();
    public static readonly Dictionary<CharacterPartID, Material> MaterialCache = new();

    public static async Task LoadAll()
    {
        await FoodItems.Load();
        await CharacterParts.Load();

        // Preload all character part prefabs and materials
        foreach (var part in CharacterParts.GetAll())
        {
            if (part.prefab != null && part.prefab.RuntimeKeyIsValid())
            {
                var prefab = await part.prefab.LoadAssetAsync<GameObject>().Task;
                PrefabCache[part.id] = prefab;
            }

            if (part.material != null && part.material.RuntimeKeyIsValid())
            {
                var mat = await part.material.LoadAssetAsync<Material>().Task;
                MaterialCache[part.id] = mat;
            }
        }
    }
}
