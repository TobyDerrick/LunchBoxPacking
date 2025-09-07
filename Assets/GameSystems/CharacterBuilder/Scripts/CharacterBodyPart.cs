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

[CreateAssetMenu(menuName = "Character/Part")]
public class CharacterPartScriptableObject : ScriptableObject
{
    public string id;
    public CharacterPartType partType;

    public AssetReferenceT<Material> material;
    public AssetReferenceT<GameObject> prefab;
}
