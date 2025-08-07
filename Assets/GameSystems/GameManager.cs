using UnityEngine;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool IsInitialized { get; private set; } = false;

    public static async Task EnsureInitialized()
    {
        if (IsInitialized) return;
        if (Instance == null)
        {
            var go = new GameObject("~GameManager");
            Instance = go.AddComponent<GameManager>();
            DontDestroyOnLoad(go);
        }

        Debug.Log("[GameManager] Loading GameData...");
        await GameData.LoadAll();
        IsInitialized = true;
        Debug.Log("[GameManager] GameData Loaded.");
    }

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await EnsureInitialized();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
