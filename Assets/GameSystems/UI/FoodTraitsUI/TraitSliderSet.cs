using UnityEngine;
using UnityEngine.UI;

public class TraitSliderSet : MonoBehaviour
{
    [SerializeField]
    protected Slider sweetSlider, savourySlider, cuteSlider, spicySlider;

    [SerializeField] private TraitValueSource traitSource;
    private ITraitValueContainer traitsToSubscribe;

    [SerializeField]
    private bool displayAbsoluteValues = false;

    protected virtual void Awake()
    {
        traitsToSubscribe = traitSource.Container;

        if (traitsToSubscribe == null)
        {
            Debug.LogError("Assigned source does not implement ITraitValueContainer!");
        }

    }

    public void OnEnable()
    {
        if (traitsToSubscribe != null)
        {
            traitsToSubscribe.TraitValues.TraitsChanged += UpdateSliderValues;
        }
    }

    public void OnDisable()
    {
        if (traitsToSubscribe != null)
        {
            traitsToSubscribe.TraitValues.TraitsChanged -= UpdateSliderValues;
        }
    }

    public void UpdateSliderValues(TraitChangedEventArgs args)
    {
        Debug.Log("Updating Trait Values");
        if(displayAbsoluteValues)
        {
            //TODO: Also update the min and max of the slider. (default to 0-1)
            sweetSlider.value = args.FoodTraits.sweet;
            savourySlider.value = args.FoodTraits.savoury;
            cuteSlider.value = args.FoodTraits.cute;
            spicySlider.value = args.FoodTraits.spicy;
        }

        else
        {
            sweetSlider.value = args.NormalizedValues.sweet;
            savourySlider.value = args.NormalizedValues.savoury;
            cuteSlider.value = args.NormalizedValues.cute;
            spicySlider.value = args.NormalizedValues.spicy;
        }

    }
}

[System.Serializable]
public class TraitValueSource
{
    [SerializeField] private MonoBehaviour monoBehaviourSource;
    [SerializeField] private FoodScriptableObject scriptableObjectSource;

    public ITraitValueContainer Container
    {
        get
        {
            if (monoBehaviourSource is ITraitValueContainer mb) return mb;
            if (scriptableObjectSource is ITraitValueContainer so) return so;
            return null;
        }
    }
}
