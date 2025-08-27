using System;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    [SerializeField] private Lunchbox lunchbox;

    public TraitRequirements currentRequest { get; private set; }

    private void Awake()
    {
        GenerateNewRequest();
    }

    public void GenerateNewRequest()
    {
        currentRequest = TraitRequirements.GenerateRandom(0f, 1f);
        EventBus.EmitNewRequest(currentRequest);
    }

    public void ValidateRequest()
    {
        if (lunchbox == null || currentRequest == null)
        {
            Debug.LogWarning("No lunchbox or request available for validation.");
            return;
        }

        float score = CalculateBoxScore(lunchbox, currentRequest);
        EventBus.EmitRequestValidated(score);

        Debug.Log($"Request validated with score: {score}");

        GenerateNewRequest();
    }

    private float CalculateBoxScore(Lunchbox lunchbox, TraitRequirements request)
    {
        float totalDiff = 0f;
        int traitCount = 0;

        foreach (FoodTrait trait in Enum.GetValues(typeof(FoodTrait)))
        {
            float value = lunchbox.TraitValues.GetTraitValue(trait);
            float min = request.minTraits.GetTraitValue(trait);
            float max = request.maxTraits.GetTraitValue(trait);

            float diff = 0f;

            //its good enough to just be in the bar
            if (value < min) diff = min - value;
            else if (value > max) diff = value - max;

            totalDiff += diff;
            traitCount++;
        }

        float avgDiff = totalDiff / traitCount; // should try to match each trait, rather than just 1 hugely
        float score = Mathf.Clamp01(1f - avgDiff); // 1 = perfect match

        return score * 100;
    }
}
