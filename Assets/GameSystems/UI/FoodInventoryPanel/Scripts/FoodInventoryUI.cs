using DG.Tweening;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FoodInventoryUI : MonoBehaviour
{
    [SerializeField] private FoodSpawner foodSpawner;

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject buttonPrefab;
    

    [Header("Plate Setup")]
    [SerializeField] private Transform plateTransform;
    [SerializeField] private Transform plateOffscreenPos, plateOnscreenPos;

    [SerializeField] private float transitionDuration = 0.5f;

    private RectTransform inventoryPanel;
    private Vector2 inventoryOnScreenPos, inventoryOffScreenPos;

    private GameObject currentFood;

    private async void Start()
    {
        inventoryPanel = this.GetComponent<RectTransform>();
        inventoryOnScreenPos = inventoryPanel.anchoredPosition;

        plateTransform.position = plateOffscreenPos.position;

        await GameManager.EnsureInitialized();
        PopulateFoodButtons();
    }

    private void PopulateFoodButtons()
    {
        foreach(FoodScriptableObject food in GameData.FoodItems.GetAll())
        {
            GameObject buttonGO = Instantiate(buttonPrefab, contentParent);
            FoodButton foodButton = buttonGO.GetComponent<FoodButton>();
            foodButton.Setup(food, () => OnFoodButtonClicked(food));
        }
    }

    private void OnFoodButtonClicked(FoodScriptableObject food)
    {
        currentFood = foodSpawner.SpawnFood(food);
        AnimateInPlate();
        Debug.Log($"Spawn Food: {food.itemName}");
    }

    public void AnimateInPlate()
    {
        Rigidbody foodRb = currentFood.GetComponent<Rigidbody>();
        foodRb.constraints = RigidbodyConstraints.FreezeAll;


        inventoryOffScreenPos = inventoryOnScreenPos - new Vector2(Screen.width, 0);

        // Swipe out UI
        inventoryPanel.DOAnchorPos(inventoryOffScreenPos, transitionDuration).SetEase(Ease.InOutCubic);

        // Swipe in Plate
        plateTransform.DOMove(plateOnscreenPos.position, transitionDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                foodRb.constraints = RigidbodyConstraints.None;
            });
    }

    public void AnimateOutPlate()
    {
        Debug.Log("Animating out plate");
        inventoryOffScreenPos = inventoryOnScreenPos - new Vector2(Screen.width, 0);

        // Swipe out Plate
        plateTransform.DOMove(plateOffscreenPos.position, transitionDuration).SetEase(Ease.Flash);

        // Swipe in UI
        inventoryPanel.DOAnchorPos(inventoryOnScreenPos, transitionDuration).SetEase(Ease.InOutCubic);
    }
}
