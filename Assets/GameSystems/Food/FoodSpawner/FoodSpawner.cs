using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private Transform foodSpawnAnchor;
    private GameObject currentFood;

    public GameObject SpawnFood(FoodScriptableObject food)
    {
        // Clear previous food
        foreach (Transform child in foodSpawnAnchor)
            Destroy(child.gameObject);

        // Spawn new food prefab at anchor
        if (food.FoodPrefab != null)
        {
            GameObject spawned = Instantiate(
                food.FoodPrefab,
                foodSpawnAnchor.position,
                Quaternion.identity
            );

            spawned.transform.SetParent(foodSpawnAnchor, true);

            spawned.transform.localScale = Vector3.one;

            return spawned;
        }

        return null;
    }


    public void ClearFood()
    {
        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }
    }
}
