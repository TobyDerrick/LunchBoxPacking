using UnityEngine;
using UnityEngine.AddressableAssets;

public enum CharacterPartType
{
    Face,
    Head,
    Torso,
    Hands
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
