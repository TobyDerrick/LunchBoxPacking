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
public class FoodScriptableObject : ScriptableObject, IDataAsset, ITraitValueContainer
{
    public FoodItemID id;
    public string itemName;
    public Sprite icon;
    public string GetID() => itemName;
    [HideInInspector]
    public FoodTraits TraitValues => traits;

    public GameObject FoodPrefab;
    public FoodTraits traits;
    public List<FoodTag> tags = new();

    public bool HasTag(FoodTag tag) => tags.Contains(tag);

}

[Serializable]
public class FoodTraits
{
    [Serializable]
    public struct TraitValues
    {
        public float sweet;
        public float savoury;
        public float cute;
        public float spicy;
    }

    public TraitValues traitValues;
    public event Action<TraitChangedEventArgs> TraitsChanged;
    public float GetTraitValue(FoodTrait trait)
    {
        return trait switch
        {
            FoodTrait.Sweet => traitValues.sweet,
            FoodTrait.Savoury => traitValues.savoury,
            FoodTrait.Cute => traitValues.cute,
            FoodTrait.Spicy => traitValues.spicy,
            _ => 0f
        };
    }

    public void SetTraitValue(FoodTrait trait, float value)
    {
        //MathHelpers.Clamp(value, 0f, 1f);
        switch (trait)
        {
            case FoodTrait.Sweet: traitValues.sweet = value; break;
            case FoodTrait.Savoury: traitValues.savoury = value; break;
            case FoodTrait.Cute: traitValues.cute = value; break;
            case FoodTrait.Spicy: traitValues.spicy = value; break;
        }
        
        TraitsChanged?.Invoke(new TraitChangedEventArgs(traitValues));
    }

    public void AddTraitValues(FoodTraits foodTraits)
    {
        traitValues.sweet += foodTraits.traitValues.sweet;
        traitValues.savoury += foodTraits.traitValues.savoury;
        traitValues.cute += foodTraits.traitValues.cute;
        traitValues.spicy += foodTraits.traitValues.spicy;

        TraitsChanged?.Invoke(new TraitChangedEventArgs(traitValues));
    }

    public void RemoveTraitValues(FoodTraits foodTraits)
    {
        traitValues.sweet -= foodTraits.traitValues.sweet;
        traitValues.savoury -= foodTraits.traitValues.savoury;
        traitValues.cute -= foodTraits.traitValues.cute;
        traitValues.spicy -= foodTraits.traitValues.spicy;

        TraitsChanged?.Invoke(new TraitChangedEventArgs(traitValues));
    }
}

public class TraitChangedEventArgs : EventArgs
{
    public FoodTraits.TraitValues FoodTraits { get; }

    public TraitChangedEventArgs(FoodTraits.TraitValues traitValues)
    {
        FoodTraits = traitValues;
    }
}