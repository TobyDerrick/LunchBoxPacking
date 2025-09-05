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

        // Build lookup table from GameData
        partLookup = new Dictionary<CharacterPartType, List<CharacterPartScriptableObject>>();
        foreach (var part in GameData.CharacterParts.GetAll())
        {
            if (!partLookup.ContainsKey(part.partType))
                partLookup[part.partType] = new List<CharacterPartScriptableObject>();
            partLookup[part.partType].Add(part);
        }

        // Spawn a random character
        GameObject go = builder.RandomizeCharacter(spawnPosition);

        // Setup tab clicks
        faceTab.onClick.AddListener(() => ShowParts(CharacterPartType.Face));
        headTab.onClick.AddListener(() => ShowParts(CharacterPartType.Head));
        torsoTab.onClick.AddListener(() => ShowParts(CharacterPartType.Torso));
        //handsTab.onClick.AddListener(() => ShowParts(CharacterPartType.Hands));

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
    }

    private void OnDisable()
    {
        // Unsubscribe from rotation actions
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
            currentPanel.Deinitialize();

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
}
