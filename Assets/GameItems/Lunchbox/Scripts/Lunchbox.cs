using System.Collections.Generic;
using UnityEngine;

public class Lunchbox : MonoBehaviour, ITraitValueContainer
{
    [SerializeField]
    private List<GameObject> foodInBox = new();

    [SerializeField]
    private FoodTraits foodTraitTotals;

    public FoodTraits TraitValues => foodTraitTotals;

    public void AddItemToBox(GameObject itemToAdd)
    {
        foodInBox.Add(itemToAdd);

        FoodScriptableObject itemSO = GameData.FoodItems.Get(itemToAdd.GetComponent<FoodItem>().id);
        foodTraitTotals.AddTraitValues(itemSO.traits);
    }

    public void RemoveItemFromBox(GameObject itemToRemove)
    {
        foodInBox.Remove(itemToRemove);

        FoodScriptableObject itemSO = GameData.FoodItems.Get(itemToRemove.GetComponent<FoodItem>().id);
        foodTraitTotals.RemoveTraitValues(itemSO.traits);

    }

    public void ClearLunchBox()
    {   
        foodInBox.Clear();
    }

}
