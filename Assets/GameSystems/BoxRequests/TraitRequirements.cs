using UnityEngine;

[System.Serializable]
public class TraitRequirement
{
    public FoodTrait trait;
    public float minValue;
    public float maxValue;

    public bool IsSatisfied(float value)
    {
        return (value >= minValue && value <= maxValue);
    }

    public TraitRequirement(FoodTrait trait, float min, float max)
    {
        this.trait = trait;
        this.minValue = min;
        this.maxValue = max;
    }
}

[System.Serializable]
public class TraitRequirements
{
    public FoodTraits minTraits;
    public FoodTraits maxTraits;

    public TraitRequirements(
        float sweetMin, float sweetMax,
        float savouryMin, float savouryMax,
        float cuteMin, float cuteMax,
        float spicyMin, float spicyMax)
    {
        minTraits = new FoodTraits(sweetMin, savouryMin, cuteMin, spicyMin);
        maxTraits = new FoodTraits(sweetMax, savouryMax, cuteMax, spicyMax);
    }
    public static TraitRequirements GenerateRandom(float minRange = 0f, float maxRange = 1f)
    {
        float sweetMin = Random.Range(minRange, maxRange);
        float sweetMax = Random.Range(sweetMin, maxRange);

        float savouryMin = Random.Range(minRange, maxRange);
        float savouryMax = Random.Range(savouryMin, maxRange);

        float cuteMin = Random.Range(minRange, maxRange);
        float cuteMax = Random.Range(cuteMin, maxRange);

        float spicyMin = Random.Range(minRange, maxRange);
        float spicyMax = Random.Range(spicyMin, maxRange);

        return new TraitRequirements(
            sweetMin, sweetMax,
            savouryMin, savouryMax,
            cuteMin, cuteMax,
            spicyMin, spicyMax
        );
    }
    public bool Validate(Lunchbox lunchbox)
    {
        return
            Check(FoodTrait.Sweet, lunchbox) &&
            Check(FoodTrait.Savoury, lunchbox) &&
            Check(FoodTrait.Cute, lunchbox) &&
            Check(FoodTrait.Spicy, lunchbox);
    }

    private bool Check(FoodTrait trait, Lunchbox lunchbox)
    {
        float value = lunchbox.TraitValues.GetTraitValue(trait);
        return value >= minTraits.GetTraitValue(trait) &&
               value <= maxTraits.GetTraitValue(trait);
    }

}
