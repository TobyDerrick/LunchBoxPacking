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
    TorsoApple,
    TorsoBunny,
    TorsoFlower,
    TorsoStar,
    TorsoStrawberry
}

[CreateAssetMenu(menuName = "Character/Part")]
public class CharacterPartScriptableObject : ScriptableObject
{
    public CharacterPartID id; // unique string ID, JSON save will use this
    public CharacterPartType partType;

    public AssetReferenceT<Material> material;
    public AssetReferenceT<GameObject> prefab;
}
