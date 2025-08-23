using DG.Tweening;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class PanUpButton : MonoBehaviour
{
    [SerializeField] private CinemachineCamera boxCamera, npcCamera;
    [SerializeField] private GameObject panUpButton, panDownButton;
    [SerializeField] private FoodInventoryUI foodInventoryUI;
    public void PanUpCamera()
    {
        npcCamera.Priority = 10;
        boxCamera.Priority = 0;
        panUpButton.gameObject.SetActive(false);
        panDownButton.gameObject.SetActive(true);
        foodInventoryUI.AnimateInPlate();
    }

    public void PanDownCamera()
    {
        npcCamera.Priority = 0;
        boxCamera.Priority = 10;

        boxCamera.Prioritize();
        panDownButton.gameObject.SetActive(false);
        panUpButton.gameObject.SetActive(true);
        foodInventoryUI.AnimateOutUI();
    }

}
