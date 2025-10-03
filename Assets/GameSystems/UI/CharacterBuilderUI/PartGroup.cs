using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A layout group that populates buttons for character parts (faces, hair, torso, etc.)
/// </summary>
[RequireComponent(typeof(LayoutGroup))]
public class PartGroup : DataLayoutGroup
{
    [Header("Layout")]
    [SerializeField] private Transform contents;
    [SerializeField] private Button buttonTemplate;
    [SerializeField] private bool clearBeforePopulate = true;

    private List<CharacterPartScriptableObject> parts;
    private CharacterBuilder builder;

    private void Awake()
    {
        if (!contents) contents = transform;
    }

    /// <summary>
    /// Provide the parts list and builder reference
    /// </summary>
    public void SetParts(List<CharacterPartScriptableObject> parts, CharacterBuilder builder)
    {
        this.parts = parts;
        this.builder = builder;
    }

    public override void Clear()
    {
        if (!contents) return;
        for (int i = contents.childCount - 1; i >= 0; i--)
            Destroy(contents.GetChild(i).gameObject);
    }

    public override void Populate()
    {
        if (parts == null || builder == null || !buttonTemplate) return;
        if (clearBeforePopulate) Clear();

        foreach (var part in parts)
        {
            var btn = Instantiate(buttonTemplate, contents);
            var iconAnimator = btn.GetComponentInChildren<UIIconAnimator>();
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text) text.text = part.name;
            if (iconAnimator) iconAnimator.Initialize(hover: part.hoverFrames, pressed: part.pressedFrames, idle: part.idleFrames);

            var capturedPart = part;
            btn.onClick.AddListener(() => builder.templateComponent.SwapPart(capturedPart.id));
        }
    }
}
