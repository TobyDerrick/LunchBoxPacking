using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public Color npcColor;
    public GameObject npcModel;
    public TraitRequirements request;
    public GameObject BoxDivider;
    public CharacterData characterData;

    private void OnEnable()
    {
        npcName = GenerateRandomName();
        characterData = new CharacterData(randomize: true);
        npcColor = new Color(Random.value, Random.value, Random.value);
        request = TraitRequirements.GenerateRandom(0f, 1f);
    }

    private string GenerateRandomName()
    {
        string[] names = { "Alex", "Jamie", "Sam", "Taylor", "Morgan", "Riley", "Jordan", "Casey" };
        return names[Random.Range(0, names.Length)];
    }
}