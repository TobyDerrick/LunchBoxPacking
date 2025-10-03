using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIIconAnimator : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float frameRate = 0.1f;

    [Tooltip("Should hover animation loop while pointer is over?")]
    [SerializeField] private bool loopHover = true;

    [Tooltip("After pressing, should the icon always return to Idle?")]
    [SerializeField] private bool returnToIdleAfterPress = false;

    [Tooltip("If true, after press the icon will return to Hover instead of Idle (if pointer is still over).")]
    [SerializeField] private bool returnToHoverAfterPress = true;

    [Tooltip("If true, any animation that finishes while pointer exits will auto-return to Idle.")]
    [SerializeField] private bool autoReturnToIdle = true;

    [Header("Frames")]
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] hoverFrames;
    [SerializeField] private Sprite[] pressedFrames;

    [Header("Target")]
    public Image image;

    private Coroutine animRoutine;
    private bool isPointerOver = false;
    private bool pendingIdle = false;

    private void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    public void Initialize(Sprite[] idle, Sprite[] hover, Sprite[] pressed)
    {
        idleFrames = idle;
        hoverFrames = hover;
        pressedFrames = pressed;

        if (image == null)
            image = GetComponent<Image>();

        if (idleFrames != null && idleFrames.Length > 0)
            PlayAnimation(idleFrames, loop: true);
    }


    private void Start()
    {
        if (idleFrames != null && idleFrames.Length > 0)
            PlayAnimation(idleFrames, loop: true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        pendingIdle = false;

        if (hoverFrames != null && hoverFrames.Length > 0)
        {
            bool loop = loopHover;
            PlayAnimation(hoverFrames, loop, onComplete: () =>
            {
                if (!loop || (pendingIdle && autoReturnToIdle))
                {
                    pendingIdle = false;
                    PlayAnimation(idleFrames, loop: true);
                }
            });
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        pendingIdle = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressedFrames == null || pressedFrames.Length == 0) return;

        PlayAnimation(pressedFrames, loop: false, onComplete: () =>
        {
            if (returnToIdleAfterPress)
            {
                PlayAnimation(idleFrames, loop: true);
            }
            else if (returnToHoverAfterPress && isPointerOver)
            {
                PlayAnimation(hoverFrames, loop: loopHover);
            }
            else if (autoReturnToIdle)
            {
                PlayAnimation(idleFrames, loop: true);
            }
        });
    }

    private void PlayAnimation(Sprite[] frames, bool loop, System.Action onComplete = null)
    {
        if (frames == null || frames.Length == 0 || image == null) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(Animate(frames, loop, onComplete));
    }

    private IEnumerator Animate(Sprite[] frames, bool loop, System.Action onComplete)
    {
        int i = 0;
        do
        {
            image.sprite = frames[i];
            i++;

            if (i >= frames.Length)
            {
                if (loop)
                {
                    i = 0;
                }
                else
                {
                    break;
                }

                if (pendingIdle)
                    break;
            }

            yield return new WaitForSeconds(frameRate);

        } while (loop || i < frames.Length);

        onComplete?.Invoke();

        if (pendingIdle && !isPointerOver && autoReturnToIdle)
        {
            pendingIdle = false;
            PlayAnimation(idleFrames, loop: true);
        }
    }


}
