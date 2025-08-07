using System;
using UnityEngine;
using System.Collections.Generic;

public enum FoodItemID
{
    Katsu,
    JamSamwich
}
public enum FoodTrait
{
    Sweet,
    Savoury,
    Cute,
    Spicy,
}

public enum FoodTag
{
    IsRed,
    IsVeg,
    IsFruit,
    IsCold,
}

[CreateAssetMenu(fileName = "FoodScriptableObject", menuName = "Scriptable Objects/FoodScriptableObject")]
public class FoodScriptableObject : ScriptableObject, IDataAsset
{
    public FoodItemID id;
    public string itemName;
    public Sprite icon;
    public string GetID() => itemName;

    public GameObject FoodPrefab;
    public FoodTraits traits;
    public List<FoodTag> tags = new();

    public bool HasTag(FoodTag tag) => tags.Contains(tag);

}

[Serializable]
public class FoodTraits
{
    [Range(0f, 1f)] public float sweet;
    [Range(0f, 1f)] public float savoury;
    [Range(0f, 1f)] public float cute;
    [Range(0f, 1f)] public float spicy;

    public float GetTraitValue(FoodTrait trait)
    {
        return trait switch
        {
            FoodTrait.Sweet => sweet,
            FoodTrait.Savoury => savoury,
            FoodTrait.Cute => cute,
            FoodTrait.Spicy => spicy,
            _ => 0f
        };
    }

    public void SetTraitValue(FoodTrait trait, float value)
    {
        MathHelpers.Clamp(value, 0f, 1f);
        switch (trait)
        {
            case FoodTrait.Sweet: sweet = value; break;
            case FoodTrait.Savoury: savoury = value; break;
            case FoodTrait.Cute: cute = value; break;
            case FoodTrait.Spicy: spicy = value; break;
        }
    }
}
