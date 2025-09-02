using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ReturnToTitleOnEscape : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseAction;
    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed += OnEscapePressed;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnEscapePressed;
            pauseAction.action.Disable();
        }
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
