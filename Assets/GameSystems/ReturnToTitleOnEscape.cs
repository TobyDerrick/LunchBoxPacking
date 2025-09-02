using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ReturnToTitleOnEscape : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput; 

    private void Awake()
    {
        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
            playerInput.actions["Pause"].performed += OnEscapePressed;
    }

    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.actions["Pause"].performed -= OnEscapePressed;
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
