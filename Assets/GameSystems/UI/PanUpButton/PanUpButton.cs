using Unity.Cinemachine;
using UnityEngine;

public enum CameraAngle
{
    boxCamera,
    NPCcamera,
}

public class PanUpButton : MonoBehaviour
{
    [SerializeField] private CinemachineCamera boxCamera, npcCamera;
    [SerializeField] private GameObject panUpButton, panDownButton;
    [SerializeField] private FoodInventoryUI foodInventoryUI;
    [SerializeField] private NPCRequestBubbles npcRequestButtons;

    public static CameraAngle currentCam;

    public void PanUpCamera()
    {
        npcCamera.Priority = 10;
        boxCamera.Priority = 0;
        panUpButton.gameObject.SetActive(false);
        panDownButton.gameObject.SetActive(true);
        foodInventoryUI.AnimateOutUI();
        npcRequestButtons.SpawnBubbles();
        currentCam = CameraAngle.NPCcamera;
    }

    public void PanDownCamera()
    {
        npcCamera.Priority = 0;
        boxCamera.Priority = 10;

        boxCamera.Prioritize();
        panDownButton.gameObject.SetActive(false);
        panUpButton.gameObject.SetActive(true);
        foodInventoryUI.AnimateOutPlate();
        npcRequestButtons.AnimateOutBubbles();
        currentCam = CameraAngle.boxCamera;
    }

}
