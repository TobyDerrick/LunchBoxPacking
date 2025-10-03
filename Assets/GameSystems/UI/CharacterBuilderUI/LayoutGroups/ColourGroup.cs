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

    public CharacterPartType characterPartTypeOverride = CharacterPartType.None;

    private CharacterTemplate template;
    private CharacterBuilder builder;
    private CharacterPartType partType;

    [SerializeField]
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

        if(characterPartTypeOverride == CharacterPartType.None)
        {
            this.partType = partType;
        }

        else
        {
            this.partType = characterPartTypeOverride;
        }
        
        this.colors = builder.characterPalette.GetColors(this.partType);

    }
    public override void Populate()
    {
        if (colors == null || template == null || !buttonTemplate) return;
        if (clearBeforePopulate) Clear();

        foreach (var color in colors)
        {
            var btn = Instantiate(buttonTemplate, contents);
            var img = btn.GetComponent<Image>();

            if (img != null)
            {
                Material matInstance = Instantiate(img.material);
                img.material = matInstance;
                if (matInstance.HasProperty("_BaseColor"))
                {
                    matInstance.SetColor("_BaseColor", color);
                }
                else if (matInstance.HasProperty("_Color"))
                {
                    matInstance.SetColor("_Color", color);
                }
            }

            btn.onClick.AddListener(() =>
            {
                builder.templateComponent.SetPartColour(partType, color);
            });
        }
    }
}
