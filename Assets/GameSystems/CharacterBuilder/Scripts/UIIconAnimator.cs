using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIIconAnimator : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector]
    public CharacterPartScriptableObject iconData;

    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private bool loopHover = true; // controls hover looping
    public Image image;

    private Coroutine animRoutine;
    private bool isPointerOver = false;
    private bool pendingIdle = false;

    public void Initialize(CharacterPartScriptableObject data)
    {
        iconData = data;

        if (image == null)
            image = GetComponent<Image>();

        if (image == null)
        {
            Debug.LogError($"UIIconAnimator requires an Image component on {gameObject.name}");
            return;
        }

        if (iconData != null && iconData.idleFrames != null)
            PlayAnimation(iconData.idleFrames, loop: true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        pendingIdle = false;

        if (iconData != null)
        {
            bool loop = loopHover;
            PlayAnimation(iconData.hoverFrames, loop: loop, onComplete: () =>
            {
                // If hover wasn't looping or pointer has exited, revert to idle
                if (!loop || pendingIdle)
                {
                    pendingIdle = false;
                    PlayAnimation(iconData.idleFrames, loop: true);
                }
            });
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        pendingIdle = true; // flag to transition to idle after current animation
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (iconData == null) return;

        PlayAnimation(iconData.pressedFrames, loop: false, onComplete: () =>
        {
            if (iconData.partType == CharacterPartType.Face)
            {
                // For eyes/faces, always go to idle after press
                PlayAnimation(iconData.idleFrames, loop: true);
            }
            else
            {
                // For other parts, revert to hover if pointer is over, else idle
                if (isPointerOver)
                    PlayAnimation(iconData.hoverFrames, loop: loopHover);
                else
                    PlayAnimation(iconData.idleFrames, loop: true);
            }
        });
    }

    private void PlayAnimation(Sprite[] frames, bool loop, System.Action onComplete = null)
    {
        if (frames == null || frames.Length == 0) return;

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
                    i = 0;
                else
                    break;

                if (pendingIdle)
                    break;
                
            }

            yield return new WaitForSeconds(frameRate);
        } while (loop || i < frames.Length);

        onComplete?.Invoke();

        if (pendingIdle && !isPointerOver && iconData != null)
        {
            pendingIdle = false;
            PlayAnimation(iconData.idleFrames, loop: true);
        }
    }
}
