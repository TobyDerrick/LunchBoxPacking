using DG.Tweening;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class PanUpButton : MonoBehaviour
{
    [SerializeField] private CinemachineCamera boxCamera, npcCamera;
    [SerializeField] private GameObject panUpButton, panDownButton;
    public void PanUpCamera()
    {
        npcCamera.Prioritize();
        panUpButton.gameObject.SetActive(false);
        panDownButton.gameObject.SetActive(true);
    }

    public void PanDownCamera()
    {
        boxCamera.Prioritize();
        panDownButton.gameObject.SetActive(false);
        panUpButton.gameObject.SetActive(true);
    }

}
