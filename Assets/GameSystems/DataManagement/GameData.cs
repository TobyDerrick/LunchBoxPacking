using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class GameData
{
    public static readonly DataGroup<FoodScriptableObject, FoodItemID> FoodItems =
        new("FoodItems", item => item.id);

    public static readonly DataGroup<CharacterPartScriptableObject, string> CharacterParts =
        new("CharacterParts", item => item.id);


    public static readonly Dictionary<string, GameObject> PrefabCache = new();
    public static readonly Dictionary<string, Material> MaterialCache = new();

    private static Task loadTask;

    public static Task LoadAll()
    {
        if (loadTask != null)
            return loadTask;

        loadTask = LoadAllInternal();
        return loadTask;
    }

    private static async Task LoadAllInternal()
    {
        await FoodItems.Load();
        await CharacterParts.Load();

        foreach (var part in CharacterParts.GetAll())
        {
            if (part.prefab != null && part.prefab.RuntimeKeyIsValid() && !PrefabCache.ContainsKey(part.id))
                PrefabCache[part.id] = await part.prefab.LoadAssetAsync<GameObject>().Task;

            if (part.material != null && part.material.RuntimeKeyIsValid() && !MaterialCache.ContainsKey(part.id))
                MaterialCache[part.id] = await part.material.LoadAssetAsync<Material>().Task;
        }
    }

}
