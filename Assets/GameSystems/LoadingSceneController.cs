using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Scene Names")]
    [SerializeField] private string titleScreenSceneName = "TitleScreen";

    private void Start()
    {
        // Start the loading process
        _ = LoadGameAsync();
    }

    private async Task LoadGameAsync()
    {
        if (progressBar != null)
            progressBar.value = 0f;

        // Ensure GameManager is initialized
        await GameManager.EnsureInitialized();

        // Wait a short moment to let player see 100%
        await Task.Delay(200);

        // Load the title screen
        SceneManager.LoadScene(titleScreenSceneName);
    }

    /// <summary>
    /// Progress callback from GameManager / GameData
    /// </summary>
    /// <param name="progress">0 to 1</param>
    private void UpdateProgress(float progress)
    {
        if (progressBar != null)
            progressBar.value = progress;

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
    }
}
