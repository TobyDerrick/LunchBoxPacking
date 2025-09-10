using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CharacterSelectorMode
{
    Save,
    Load
}

public class CharacterSelectorController : UIPanel
{
    [SerializeField] private GameObject buttonTemplate;
    [SerializeField] private Transform contents;

    [SerializeField] private CharacterBuilder characterBuilder;
    [SerializeField] private NPCSaveLoadManager npcSaveLoadManager;
    [SerializeField] private Button returnButton;

    private CharacterSelectorMode currentMode;
    private List<GameObject> npcButtons = new List<GameObject>();

    public void Initialize(List<NPCData> npcs, CharacterSelectorMode mode)
    {
        currentMode = mode;
        Deinitialize();

        for (int i = 0; i < npcs.Count; i++)
        {
            NPCData npcData = npcs[i];
            GameObject thisButton = Instantiate(buttonTemplate, contents);
            npcButtons.Add(thisButton);

            NPCPreviewButton npcPreview = thisButton.GetComponent<NPCPreviewButton>();
            npcPreview.slotIndex = i;

            if (npcData != null)
            {
                npcPreview.nameText.text = npcData.npcName;
                npcPreview.currentData = npcData;
            }
            else
            {
                npcPreview.nameText.text = "[Empty Slot]";
                npcPreview.currentData = null;
            }

            npcPreview.button.onClick.RemoveAllListeners();

            if (currentMode == CharacterSelectorMode.Load)
            {
                npcPreview.button.onClick.AddListener(() => LoadNPC(npcPreview.slotIndex));
            }
            else if (currentMode == CharacterSelectorMode.Save)
            {
                npcPreview.button.onClick.AddListener(() =>
                {
                    SaveNPC(npcPreview.slotIndex, characterBuilder.GetCurrentCharacterData().NPCName);
                });
            }

            if (npcPreview.deleteButton != null)
            {
                npcPreview.deleteButton.onClick.RemoveAllListeners();
                npcPreview.deleteButton.gameObject.SetActive(currentMode == CharacterSelectorMode.Save && npcData != null);
                npcPreview.deleteButton.onClick.AddListener(() => DeleteNPC(npcPreview.slotIndex));
            }
        }

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() => EventBus.EmitCharacterLoaded(null));
    }

    public override void Deinitialize()
    {
        foreach (GameObject thisButton in npcButtons)
            Destroy(thisButton);

        npcButtons.Clear();
    }

    public void LoadNPC(int slotIndex)
    {
        NPCData data = npcSaveLoadManager.LoadNPCFromSlot(slotIndex);
        if (data == null) return;

        characterBuilder.templateComponent.ApplyAllParts(data.characterData);
        EventBus.EmitCharacterLoaded(data.characterData);

        Deinitialize();
    }

    public void SaveNPC(int slotIndex, string npcName)
    {
        CharacterData current = characterBuilder.GetCurrentCharacterData();
        if (current == null) return;

        npcSaveLoadManager.SaveNPC(slotIndex, npcName, current);

        // Refresh the UI with the updated slots
        Initialize(npcSaveLoadManager.GetSavedNPCS(), CharacterSelectorMode.Save);
    }

    private void DeleteNPC(int slotIndex)
    {
        npcSaveLoadManager.ClearSlot(slotIndex);
        Initialize(npcSaveLoadManager.GetSavedNPCS(), currentMode);
    }
}
