using UnityEngine;
using UnityEngine.UI;

public class ScrollRectButton : MonoBehaviour
{
    [Header("ScrollRect Settings")]
    public ScrollRect scrollRect;
    [Tooltip("Scrolling speed in normalized units per second")]
    public float scrollSpeed = 1f;

    [Header("Buttons")]
    public Button upButton;
    public Button downButton;

    private bool scrollingUp = false;
    private bool scrollingDown = false;

    void Start()
    {
        if (upButton != null)
        {
            upButton.onClick.AddListener(() => { });
            AddHoldEvents(upButton, () => scrollingUp = true, () => scrollingUp = false);
        }

        if (downButton != null)
        {
            downButton.onClick.AddListener(() => { });
            AddHoldEvents(downButton, () => scrollingDown = true, () => scrollingDown = false);
        }
    }

    void Update()
    {
        bool canScroll = scrollRect.content.rect.height > scrollRect.viewport.rect.height;
        if (upButton != null) upButton.gameObject.SetActive(canScroll);
        if (downButton != null) downButton.gameObject.SetActive(canScroll);

        if (scrollingUp)
        {
            scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;
            if (scrollRect.verticalNormalizedPosition > 1f) scrollRect.verticalNormalizedPosition = 1f;
        }
        else if (scrollingDown)
        {
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;
            if (scrollRect.verticalNormalizedPosition < 0f) scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void AddHoldEvents(Button button, System.Action onPress, System.Action onRelease)
    {
        var trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        var entryDown = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
        entryDown.callback.AddListener((_) => onPress());
        trigger.triggers.Add(entryDown);

        var entryUp = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        entryUp.callback.AddListener((_) => onRelease());
        trigger.triggers.Add(entryUp);

        var entryExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        entryExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        entryExit.callback.AddListener((_) => onRelease());
        trigger.triggers.Add(entryExit);
    }
}
