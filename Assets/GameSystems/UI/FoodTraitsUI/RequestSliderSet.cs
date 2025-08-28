using UnityEngine;
using UnityEngine.UI;

public class RequestSliderSet : TraitSliderSet
{
    [Header("Requirement Range Images")]
    [SerializeField] private Image sweetRangeImage;
    [SerializeField] private Image savouryRangeImage;
    [SerializeField] private Image cuteRangeImage;
    [SerializeField] private Image spicyRangeImage;

    [SerializeField]
    private TraitRequirements requirements;

    protected override void Awake()
    {

    }

    private new void OnEnable()
    {
        //base.OnEnable();
        EventBus.OnNewNPC += SetRequirements;
        EventBus.OnNewLunchbox += SetBox;
    }

    private new void OnDisable()
    {
        //base.OnDisable();
        EventBus.OnNewNPC -= SetRequirements;
        EventBus.OnNewLunchbox -= SetBox;

    }

    private void SetBox(Lunchbox box)
    {
        traitSource.monoBehaviourSource = box;
        traitsToSubscribe = traitSource.Container;
        traitsToSubscribe.TraitValues.TraitsChanged += UpdateSliderValues;
        UpdateSliderValues(new TraitChangedEventArgs(traitsToSubscribe.TraitValues.traitValues, traitsToSubscribe.TraitValues.normalizedTraitValues));
    }

    private void UpdateRangeVisuals()
    {
        SetRangeVisual(FoodTrait.Sweet, sweetRangeImage, sweetSlider, requirements.minTraits.traitValues.sweet, requirements.maxTraits.traitValues.sweet);
        SetRangeVisual(FoodTrait.Savoury, savouryRangeImage, savourySlider, requirements.minTraits.traitValues.savoury, requirements.maxTraits.traitValues.savoury);
        SetRangeVisual(FoodTrait.Cute, cuteRangeImage, cuteSlider, requirements.minTraits.traitValues.cute, requirements.maxTraits.traitValues.cute);
        SetRangeVisual(FoodTrait.Spicy, spicyRangeImage, spicySlider, requirements.minTraits.traitValues.spicy, requirements.maxTraits.traitValues.spicy);
    }

    private void SetRangeVisual(FoodTrait trait, Image rangeImage, Slider slider, float min, float max)
    {
        if (rangeImage == null || slider == null) return;

        RectTransform rt = rangeImage.rectTransform;

        float normalizedMin = Mathf.InverseLerp(slider.minValue, slider.maxValue, min);
        float normalizedMax = Mathf.InverseLerp(slider.minValue, slider.maxValue, max);

        rt.anchorMin = new Vector2(normalizedMin, 0f);
        rt.anchorMax = new Vector2(normalizedMax, 1f);

        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        //UpdateSliderValues(new TraitChangedEventArgs(traitsToSubscribe.TraitValues.traitValues, traitsToSubscribe.TraitValues.normalizedTraitValues));
    }

    public void SetRequirements(NPCData npcData)
    {
        requirements = npcData.request;
        UpdateRangeVisuals();
    }

}
