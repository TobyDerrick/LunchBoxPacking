using System.Collections.Generic;
using UnityEngine;

public class Lunchbox : MonoBehaviour, ITraitValueContainer
{
    public GameObject lid;
    public Transform lidTarget;

    public List<GameObject> foodInBox = new();

    [SerializeField]
    private FoodTraits foodTraitTotals;

    public FoodTraits TraitValues => foodTraitTotals;

    public void AddItemToBox(GameObject itemToAdd)
    {
        if (foodInBox.Contains(itemToAdd.GetComponentInParent<Rigidbody>().gameObject))
        {
            return;
        }

        itemToAdd = itemToAdd.GetComponentInParent<Rigidbody>().gameObject;
        foodInBox.Add(itemToAdd);

        FoodScriptableObject itemSO = GameData.FoodItems.Get(itemToAdd.GetComponent<FoodItem>().id);
        foodTraitTotals.AddTraitValues(itemSO.traits);

        itemToAdd.transform.SetParent(transform);
    }

    public void RemoveItemFromBox(GameObject itemToRemove)
    {
        if (!foodInBox.Contains(itemToRemove.GetComponentInParent<Rigidbody>().gameObject))
        {
            return;
        }
        itemToRemove = itemToRemove.GetComponentInParent<Rigidbody>().gameObject;
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
