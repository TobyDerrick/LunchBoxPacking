using UnityEngine;
using UnityEngine.AddressableAssets;

public enum CharacterPartType
{
    Skin,
    Head,
    Torso,
    Hands,
    None,
    Face,
}

public enum CharacterPartID
{
    FaceBigEye,
    FaceBigEyeNoLash,
    FaceGlasses,
    FaceGlassesFreckles,
    FaceLongEye,
    FaceSlanted,
    Hands,
    HairBraids,
    HairPigtails,
    HairPonytail,
    HairShortWavy,
    HairSidePart,
    HairLong,
    HairBun,
    HairSpaceBun,
    HairAfro,
    HairSidePony,
    HairBob,
    TorsoApple,
    TorsoBunny,
    TorsoFlower,
    TorsoStar,
    TorsoFish,
    TorsoStrawberry
}

[CreateAssetMenu(menuName = "Character/Part")]
public class CharacterPartScriptableObject : ScriptableObject
{
    public CharacterPartID id;
    public CharacterPartType partType;

    public AssetReferenceT<Material> material;
    public AssetReferenceT<GameObject> prefab;
}
