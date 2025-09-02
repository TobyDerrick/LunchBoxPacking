using System.Threading.Tasks;

public static class GameData
{
    public static readonly DataGroup<FoodScriptableObject, FoodItemID> FoodItems = new("FoodItems", item => item.id);
    public static readonly DataGroup<CharacterPartScriptableObject, CharacterPartID> CharacterParts = new("CharacterParts", item => item.id);

    public static async Task LoadAll()
    {
        await FoodItems.Load();
    }
}