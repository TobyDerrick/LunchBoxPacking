using UnityEngine;
using System.Collections.Generic;

public class PartPanel : UIPanel
{
    [Header("Layout Groups")]
    [SerializeField] private DataLayoutGroup[] layoutGroups;

    private CharacterPartType partType;
    private List<CharacterPartScriptableObject> parts;
    private CharacterTemplate character;
    private CharacterBuilder characterBuilder;

    /// <summary>
    /// Setup panel with the relevant data
    /// </summary>
    public void Setup(CharacterPartType type, List<CharacterPartScriptableObject> parts, CharacterBuilder builder)
    {
        this.partType = type;
        this.parts = parts;
        this.characterBuilder = builder;
        this.character = characterBuilder.templateComponent;

        foreach (var group in layoutGroups)
        {
            if (group is PartGroup partGroup)
            {
                partGroup.SetParts(parts, builder);
            }

            if (group is ColourGroup colourGroup)
            {
                colourGroup.SetupColorGroup(builder, partType);
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        foreach (var group in layoutGroups)
        {
            group.Populate();
        }
    }

    public override void Deinitialize()
    {
        base.Deinitialize();

        foreach (var group in layoutGroups)
            group.Clear();
    }
}
