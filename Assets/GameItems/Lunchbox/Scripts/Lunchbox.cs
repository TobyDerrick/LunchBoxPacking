using System.Collections.Generic;
using UnityEngine;

public class Lunchbox : MonoBehaviour, ITraitValueContainer
{
    public GameObject lid;
    public Transform lidTarget;

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

        itemToAdd.transform.SetParent(transform);
    }

    public void RemoveItemFromBox(GameObject itemToRemove)
    {
        foodInBox.Remove(itemToRemove);

        FoodScriptableObject itemSO = GameData.FoodItems.Get(itemToRemove.GetComponent<FoodItem>().id);
        foodTraitTotals.RemoveTraitValues(itemSO.traits);

        itemToRemove.transform.SetParent(null);

    }

    public void ClearLunchBox()
    {   
        foodInBox.Clear();
    }

}
