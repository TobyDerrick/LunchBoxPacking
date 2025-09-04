using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterPalette", menuName = "Character/Palette")]
public class CharacterPalette : ScriptableObject
{
    public string PaletteName;

    public Color[] HairColors;
    public Color[] EyeColors;
    public Color[] SkinColors;
    public Color[] ShirtColors;

    public Color[] GetColors(CharacterPartType partType)
    {
        switch (partType)
        {
            case CharacterPartType.Face:
                return SkinColors;
            case CharacterPartType.Head:
                return HairColors;
            case CharacterPartType.Torso:
                return ShirtColors;
            case CharacterPartType.Hands:
                return SkinColors;
        }
        return null;
    }

    // Get a random color from the array, fallback to white
    public Color GetRandomHair() => HairColors != null && HairColors.Length > 0 ? HairColors[UnityEngine.Random.Range(0, HairColors.Length)] : Color.white;
    public Color GetRandomEye() => EyeColors != null && EyeColors.Length > 0 ? EyeColors[UnityEngine.Random.Range(0, EyeColors.Length)] : Color.white;
    public Color GetRandomSkin() => SkinColors != null && SkinColors.Length > 0 ? SkinColors[UnityEngine.Random.Range(0, SkinColors.Length)] : Color.white;
    public Color GetRandomShirt() => ShirtColors != null && ShirtColors.Length > 0 ? ShirtColors[UnityEngine.Random.Range(0, ShirtColors.Length)] : Color.white;
}
