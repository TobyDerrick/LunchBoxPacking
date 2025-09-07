using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCPreviewButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI nameText;
    public NPCData currentData;

    public Button deleteButton;
    [HideInInspector] public int slotIndex;
}
