using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPCRequestBubbles : MonoBehaviour
{
    [SerializeField] private GameObject bubbleTemplate;
    [SerializeField] private CircularLayoutGroup layoutGroup;



    [Header("Animation")]
    public float tweenDuration = 0.5f;
    public Ease tweenEase = Ease.OutBack;

    private List<string> bubbleTexts = new();
    private List<GameObject> spawnedBubbles = new();

    private float maxRadius;

    private void Awake()
    {
        EventBus.OnNewRequest += InitializeRequestBubbles;
        layoutGroup = GetComponent<CircularLayoutGroup>();
        maxRadius = layoutGroup.radius;
    }

    public void InitializeRequestBubbles(TraitRequirements requirements)
    {
        bubbleTexts = ConvertRequirementsToText(requirements);
    }
    public void SpawnBubbles()
    {
        layoutGroup.radius = maxRadius;
        ClearExistingBubbles();

        if (bubbleTemplate == null)
        {
            Debug.LogError("Bubble template not assigned");
            return;
        }

        foreach (string text in bubbleTexts)
        {
            GameObject bubble = Instantiate(bubbleTemplate, layoutGroup.transform);
            TextMeshProUGUI tmp = bubble.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = text;

            spawnedBubbles.Add(bubble);
        }

        layoutGroup.RefreshLayout();
    }

    private List<string> ConvertRequirementsToText(TraitRequirements requirements)
    {
        List<string> texts = new();

        foreach(FoodTrait trait in System.Enum.GetValues(typeof(FoodTrait)))
        {
            float min = requirements.minTraits.GetTraitValue(trait);
            float max = requirements.maxTraits.GetTraitValue(trait);
            float range = max - min;

            string text = "";

            if (range > 0.5f) text += "Try to be";
            else if (range < 0.15) text += "Make sure it's";
            else text += "I'd like it";


            if (min <= 0.3f) text += $" not too {trait.ToString().ToLower()}";
            else if (min >= 0.8f) text += $" very {trait.ToString().ToLower()}";
            else if (min >= 0.3f && min <= 0.8f) text += $" average {trait.ToString().ToLower()}";
            else
            {
                text += "AAA";
            }
            ;

            if (!string.IsNullOrEmpty(text))
                texts.Add(text);
        }

        return texts;
    }
    public void AnimateInBubbles()
    {
        SpawnBubbles();
        if (layoutGroup == null) return;
        layoutGroup.radius = 0f;
        layoutGroup.RefreshLayout();
        layoutGroup.AnimateRadius(maxRadius, tweenDuration, tweenEase);
    }

    public void AnimateOutBubbles()
    {
        if (layoutGroup == null) return;
        layoutGroup.AnimateRadius(0f, tweenDuration, tweenEase).OnComplete(ClearExistingBubbles);
    }
    public void ClearExistingBubbles()
    {
        foreach (GameObject bubble in spawnedBubbles)
        {
            if (bubble != null)
            {
                Destroy(bubble);
            }
        }
        spawnedBubbles.Clear();
    }
}
