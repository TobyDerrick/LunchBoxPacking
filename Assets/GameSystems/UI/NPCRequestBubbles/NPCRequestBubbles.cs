using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        EventBus.OnNewNPC += InitializeRequestBubbles;
        EventBus.OnRequestValidated += AnimateOutBubbles;
        layoutGroup = GetComponent<CircularLayoutGroup>();
        maxRadius = layoutGroup.radius;
    }

    public void InitializeRequestBubbles(NPCData npcData)
    {
        TraitRequirements requirements = npcData.request;

        bubbleTexts = ConvertRequirementsToText(requirements);
        //If current cam = NPC cam, spawn bubbles
        if(PanUpButton.currentCam == CameraAngle.NPCcamera)
        {
            SpawnBubbles();
        }
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
            float midpoint = max + min / 2;

            string text = "";

            if (range > 0.5f) text += "Try to be";
            else if (range < 0.15) text += "Make sure it's";
            else text += "I'd like it";


            if (midpoint <= 0.3f) text += $" not too {trait.ToString().ToLower()}";
            else if (midpoint >= 0.7f) text += $" very {trait.ToString().ToLower()}";
            else if (midpoint >= 0.3f && midpoint <= 0.8f) text += $" average {trait.ToString().ToLower()}";
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

    public void AnimateOutBubbles(float score = 0, Lunchbox box = null)
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
