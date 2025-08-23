using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum CursorState
{
    Default,
    Grab,
    Hover,
}

public class CursorController : MonoBehaviour
{
    [Header("Cursor UI")]
    public Image cursorImage;
    public Sprite defaultSprite;
    public Sprite grabSprite;
    public Sprite hoverSprite;

    [SerializeField] private CursorState currentState = CursorState.Default;

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeStrength = 5f;
    public int shakeVibrato = 10;

    [Header("Input System")]
    public InputActionReference pointerPositionAction;

    private Vector3 pointerPosition;
    private Vector3 shakeOffset;
    private Tween currentShakeTween;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        pointerPositionAction.action.Enable();
        pointerPositionAction.action.performed += OnPointerMoved;
    }

    void OnDisable()
    {
        pointerPositionAction.action.performed -= OnPointerMoved;
        pointerPositionAction.action.Disable();
    }

    private void OnPointerMoved(InputAction.CallbackContext context)
    {
        pointerPosition = context.ReadValue<Vector2>();
    }

    void Update()
    {
        cursorImage.transform.position = pointerPosition + shakeOffset;
    }

    public void SetCursorState(CursorState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        UpdateCursorSprite();

        if (currentState == CursorState.Grab)
        {
            StartShake();
        }
        else
        {
            StopShake();
        }
    }

    private void UpdateCursorSprite()
    {
        switch (currentState)
        {
            case CursorState.Default:
                cursorImage.sprite = defaultSprite;
                break;
            case CursorState.Grab:
                cursorImage.sprite = grabSprite;
                break;
            case CursorState.Hover:
                cursorImage.sprite = hoverSprite;
                break;
        }
    }

    private void StartShake()
    {
        StopShake();
        currentShakeTween = DOTween.To(
            () => shakeOffset,
            x => shakeOffset = x,
            new Vector3(0, 0, 0),
            shakeDuration
        )
        .SetLoops(-1, LoopType.Yoyo)
        .OnUpdate(() =>
        {
            shakeOffset = Random.insideUnitCircle * shakeStrength;
        });
    }

    private void StopShake()
    {
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
            currentShakeTween = null;
        }
        shakeOffset = Vector3.zero;
    }
}
