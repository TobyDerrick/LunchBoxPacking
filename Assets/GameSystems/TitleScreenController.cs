using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button boxPackingButton;
    [SerializeField] private Button characterCreatorButton;

    private async void Awake()
    {
        // Ensure GameManager and GameData are ready before showing UI
        await GameManager.EnsureInitialized();

        // Hook up buttons
        if (boxPackingButton != null)
            boxPackingButton.onClick.AddListener(() => LoadScene("PackingScene"));

        if (characterCreatorButton != null)
            characterCreatorButton.onClick.AddListener(() => LoadScene("CharacterBuilder"));
    }

    private void LoadScene(string sceneName)
    {
        // Simple scene load; could add async/transition effects later
        SceneManager.LoadScene(sceneName);
    }
}
