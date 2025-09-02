using UnityEngine;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        TitleScreen,
        Packing,
        Paused,
        CharacterCreator,
    }

    public static GameManager Instance { get; private set; }
    public static bool IsInitialized { get; private set; } = false;
    private static Task initializationTask;

    public static Task EnsureInitialized()
    {
        if (IsInitialized)
            return Task.CompletedTask;
        if (initializationTask != null)
            return initializationTask;

        initializationTask = InitializeAsync();
        return initializationTask;
    }

    private static async Task InitializeAsync()
    {
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
            return;
        }

        StartPackingGame();
    }

    private void StartPackingGame()
    {
        // Game start logic goes here
    }
}
