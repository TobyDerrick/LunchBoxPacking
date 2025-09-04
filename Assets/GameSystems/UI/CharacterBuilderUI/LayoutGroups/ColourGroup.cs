using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourGroup : DataLayoutGroup
{
    [Header("Layout")]
    [SerializeField] private Transform contents;
    [SerializeField] private Button buttonTemplate;
    [SerializeField] private bool clearBeforePopulate = true;

    private CharacterTemplate template;
    private CharacterBuilder builder;
    private CharacterPartType partType;
    private Color[] colors;
    public override void Clear()
    {
        if (!contents) return;

        for (int i = contents.childCount - 1; i >= 0; i--)
            Destroy(contents.GetChild(i).gameObject);
    }

    public void SetupColorGroup(CharacterBuilder builder, CharacterPartType partType)
    {
        this.builder = builder;
        this.template = builder.templateComponent;
        this.partType = partType;
        this.colors = builder.characterPalette.GetColors(partType);

    }
    public override void Populate()
    {
        if (colors == null || template == null || !buttonTemplate) return;
        if (clearBeforePopulate) Clear();

        foreach (var color in colors)
        {
            var btn = Instantiate(buttonTemplate, contents);
            var img = btn.GetComponent<Image>();
            if (img) img.color = new Color(color.r,color.g,color.b, 1);

            var capturedColor = color;
            btn.onClick.AddListener(() =>
            {
                builder.templateComponent.SetPartColour(partType, color);
            });
        }
    }
}
