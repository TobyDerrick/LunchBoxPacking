using DG.Tweening;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FoodInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject buttonPrefab;
    

    [Header("Plate Setup")]
    [SerializeField] private Transform plateTransform;
    [SerializeField] private Transform plateOffscreenPos, plateOnscreenPos;

    [SerializeField] private float transitionDuration = 0.5f;

    private RectTransform inventoryPanel;
    private Vector2 inventoryOnScreenPos, inventoryOffScreenPos;

    private async void Start()
    {
        inventoryPanel = this.GetComponent<RectTransform>();
        inventoryOnScreenPos = inventoryPanel.anchoredPosition;
        inventoryOffScreenPos = inventoryOnScreenPos - new Vector2(2 * Screen.width, 0);

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
        AnimateInPlate();
        Debug.Log($"Spawn Food: {food.itemName}");
    }

    public void AnimateInPlate()
    {
        // Swipe out UI
        inventoryPanel.DOAnchorPos(inventoryOffScreenPos, transitionDuration).SetEase(Ease.InOutCubic);

        // Swipe in Plate
        plateTransform.DOMove(plateOnscreenPos.position, transitionDuration).SetEase(Ease.InBounce);
    }

    public void AnimateOutPlate()
    {
        // Swipe out Plate
        plateTransform.DOMove(plateOffscreenPos.position, transitionDuration).SetEase(Ease.Flash);

        // Swipe in UI
        inventoryPanel.DOAnchorPos(inventoryOnScreenPos, transitionDuration).SetEase(Ease.InOutCubic);
    }
}
