using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class CharacterCreatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterBuilder builder;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private NPCSaveLoadManager npcSaveLoadManager;
    [SerializeField] private TMP_InputField npcNameField;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button randomizeButton;

    [Header("Tabs")]
    [SerializeField] private Button faceTab;
    [SerializeField] private Button headTab;
    [SerializeField] private Button torsoTab;
    [SerializeField] private Button handsTab;

    [Header("Panels")]
    [SerializeField] private PartPanel facePanel;
    [SerializeField] private PartPanel headPanel;
    [SerializeField] private PartPanel torsoPanel;
    [SerializeField] private PartPanel handsPanel;
    [SerializeField] private GameObject CharacterCreator, CharacterSelector;

    [Header("Rotation Controls")]
    [SerializeField] private InputActionReference rotateLeftAction;
    [SerializeField] private InputActionReference rotateRightAction; 
    [SerializeField] private float rotationSpeed = 100f;

    private Dictionary<CharacterPartType, List<CharacterPartScriptableObject>> partLookup;
    private PartPanel currentPanel;

    private int rotateDirection; // -1 = left, 1 = right, 0 = none

    private async void Awake()
    {
        await GameManager.EnsureInitialized();

        SaveManager.LoadGame();

        // Build lookup table from GameData
        partLookup = new Dictionary<CharacterPartType, List<CharacterPartScriptableObject>>();
        foreach (var part in GameData.CharacterParts.GetAll())
        {
            if (!partLookup.ContainsKey(part.partType))
            {
                partLookup[part.partType] = new List<CharacterPartScriptableObject>();
            }

            partLookup[part.partType].Add(part);
        }

        // Spawn a random character
        GameObject go = builder.RandomizeCharacter(spawnPosition);

        // Setup tab clicks
        faceTab.onClick.AddListener(() => ShowParts(CharacterPartType.Face));
        headTab.onClick.AddListener(() => ShowParts(CharacterPartType.Head));
        torsoTab.onClick.AddListener(() => ShowParts(CharacterPartType.Torso));
        //handsTab.onClick.AddListener(() => ShowParts(CharacterPartType.Hands));

        saveButton.onClick.AddListener(OnSavePressed);
        loadButton.onClick.AddListener(OnLoadPressed);
        randomizeButton.onClick.AddListener(OnRandomizePressed);


        // Default tab
        ShowParts(CharacterPartType.Head);
    }

    private void OnEnable()
    {
        // Subscribe to rotation actions
        if (rotateLeftAction != null)
        {
            rotateLeftAction.action.performed += _ => rotateDirection = -1;
            rotateLeftAction.action.canceled += _ => StopRotation(-1);
            rotateLeftAction.action.Enable();
        }

        if (rotateRightAction != null)
        {
            rotateRightAction.action.performed += _ => rotateDirection = 1;
            rotateRightAction.action.canceled += _ => StopRotation(1);
            rotateRightAction.action.Enable();
        }

        if (npcNameField != null)
        {
            npcNameField.onValueChanged.AddListener(OnNameChanged);
        }

        EventBus.OnCharacterLoaded += OnCharacterSelected;
    }

    private void OnDisable()
    {
        if (rotateLeftAction != null)
        {
            rotateLeftAction.action.performed -= _ => rotateDirection = -1;
            rotateLeftAction.action.canceled -= _ => StopRotation(-1);
            rotateLeftAction.action.Disable();
        }

        if (rotateRightAction != null)
        {
            rotateRightAction.action.performed -= _ => rotateDirection = 1;
            rotateRightAction.action.canceled -= _ => StopRotation(1);
            rotateRightAction.action.Disable();
        }

        if (npcNameField != null)
        {
            npcNameField.onValueChanged.RemoveListener(OnNameChanged);
        }

        EventBus.OnCharacterLoaded -= OnCharacterSelected;
    }

    private void Update()
    {
        if (rotateDirection != 0)
            RotateCharacterPreview();
    }

    private void ShowParts(CharacterPartType type)
    {
        // Deinit current panel
        if (currentPanel != null)
        {
            currentPanel.Deinitialize();
        }

        // Choose the correct panel
        switch (type)
        {
            case CharacterPartType.Face:
                currentPanel = facePanel;
                break;
            case CharacterPartType.Head:
                currentPanel = headPanel;
                break;
            case CharacterPartType.Torso:
                currentPanel = torsoPanel;
                break;
            case CharacterPartType.Hands:
                currentPanel = handsPanel;
                break;
        }

        // Init panel and give it data
        if (currentPanel != null && partLookup.TryGetValue(type, out var parts))
        {
            currentPanel.Setup(type, parts, builder);
            currentPanel.Initialize();
        }
    }

    private void RotateCharacterPreview()
    {
        spawnPosition.Rotate(Vector3.up, rotateDirection * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void StopRotation(int dir)
    {
        if (rotateDirection == dir)
            rotateDirection = 0;
    }

    private void OnSavePressed()
    {
        CharacterCreator.SetActive(false);
        CharacterSelector.SetActive(true);

        CharacterSelector.GetComponent<CharacterSelectorController>().Initialize(npcSaveLoadManager.GetSavedNPCS(), CharacterSelectorMode.Save);
    }

    private void OnNameChanged(string newName)
    {
        builder.templateComponent.currentCharacter.NPCName = string.IsNullOrWhiteSpace(newName) ? "Unnamed" : newName;
        Debug.Log($"NPC Name set to: {newName}");
    }

    public string GetNpcName()
    {
        return builder.templateComponent.currentCharacter.NPCName;
    }

    private void OnLoadPressed()
    {
        CharacterCreator.SetActive(false);
        CharacterSelector.SetActive(true);

        CharacterSelector.GetComponent<CharacterSelectorController>().Initialize(npcSaveLoadManager.GetSavedNPCS(), CharacterSelectorMode.Load);
    }

    private void OnCharacterSelected(CharacterData data)
    {
        if(data != null)
        {
            npcNameField.text = data.NPCName;
        }
        CharacterCreator.SetActive(true);
        CharacterSelector.SetActive(false);
    }

    private void OnRandomizePressed()
    {
        npcNameField.text = string.Empty;
        builder.RandomizeCharacter(spawnPosition);
    }
}
