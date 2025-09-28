using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIIconAnimator : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public CharacterPartScriptableObject iconData;
    [SerializeField] private float frameRate = 0.1f;

    private Image image;
    private Coroutine animRoutine;

    private void Awake()
    {
        image = GetComponent<Image>();
        PlayAnimation(iconData.idleFrames);
    }

    public void OnPointerEnter(PointerEventData eventData) => PlayAnimation(iconData.hoverFrames);
    public void OnPointerExit(PointerEventData eventData) => PlayAnimation(iconData.idleFrames);
    public void OnPointerDown(PointerEventData eventData) => PlayAnimation(iconData.pressedFrames);
    public void OnPointerUp(PointerEventData eventData) => PlayAnimation(iconData.hoverFrames);

    private void PlayAnimation(Sprite[] frames)
    {
        if (frames == null || frames.Length == 0) return;

        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(Animate(frames));
    }

    private IEnumerator Animate(Sprite[] frames)
    {
        int i = 0;
        while (true)
        {
            image.sprite = frames[i];
            i = (i + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }
}
