using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataGroup<TAsset, TKey> 
    where TAsset : ScriptableObject
{
    private Dictionary<TKey, TAsset> items = new();
    public bool IsLoaded { get; private set; } = false;

    private readonly string label;
    private readonly Func<TAsset, TKey> keySelector;

    public DataGroup(string label, Func<TAsset, TKey> keySelector)
    {
        this.label = label;
        this.keySelector = keySelector;
    }

    public async Task Load()
    {
        if (IsLoaded) return;

        var handle = Addressables.LoadAssetsAsync<TAsset>(label);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var item in handle.Result)
            {
                var key = keySelector(item);
                if (!items.ContainsKey(key))
                {
                    items[key] = item;
                }

                else
                {
                    Debug.LogWarning($"Duplicate key in {typeof(TAsset)}: {key}");
                }
            }

            IsLoaded = true;
        }

        else
        {
            Debug.LogError($"[DataGroup] Failed to load assets of type {typeof(TAsset).Name}");
        }
    }
    public TAsset Get(TKey key)
    {
        return items.TryGetValue(key, out var item) ? item : null;
    }
    public IEnumerable<TAsset> GetAll() => items.Values;
}
